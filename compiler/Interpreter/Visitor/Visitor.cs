using compiler.Nodes;
using compiler.Nodes.ExpressionNodes;
using compiler.Nodes.ExpressionNodes.SimpleExpressionNodes;
using compiler.Nodes.InstructionNodes;
using compiler.Nodes.Interfaces;
using compiler.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Interpreter.Visitor
{
    public class Visitor : IVisitor
    {
        //metody odwiedzające konkretne node'y 
        public Stack<FunctionCallContext> functionCallContexts;
        public List<FunctionNode> functionDefinitions;

        List<ILastExpressionValue> lastExpressionValues;
        bool IsFunctionReturned;
        ILastExpressionValue returnedValue;
        
        public Visitor() 
        {
            functionCallContexts = new Stack<FunctionCallContext>();
            functionDefinitions = new List<FunctionNode>();
            lastExpressionValues = new List<ILastExpressionValue>();
            IsFunctionReturned = false;
        }
        public void Visit(ProgramNode programNode)
        {
            FunctionNode main = null;
            foreach (FunctionNode functionNode in programNode.functionNodes) 
            {
                if(functionNode == null) 
                {
                    //wyjatek - jakiś node z def funkcji jest pusty
                    
                }
                if(functionNode.identifier == "main") 
                {
                    if(main != null) 
                    {
                        //wyjatek - there is not 1 main function
                    }
                    main = functionNode;
                    
                    
                }
                functionDefinitions.Add(functionNode);
            }
            main.Accept(this);
            if (IsFunctionReturned) 
            {
                if (GetLastExpressionType() == ValueType.DOUBLE)
                {
                    Console.WriteLine("Main returned with double value " + GetLastExpressionValue().ToString());
                }
                else if (GetLastExpressionType() == ValueType.INT)
                {
                    Console.WriteLine("Main returned with int value " + ((int)GetLastExpressionValue()).ToString());
                }
            }
            else 
            {
                // wyjatek - nie ma return w mainuie 
            }
        }

        public void Visit(FunctionNode functionNode)
        {
            functionCallContexts.Push(new FunctionCallContext(functionNode.identifier));
            FunctionCallContext functionCallContext = functionCallContexts.Peek();
            functionCallContext.functionScopes.Push(new FunctionScope());
            functionNode.instructionsBlockNode.Accept(this);
            functionCallContexts.Pop();
        }

        public void Visit(InstructionsBlockNode instructionsBlockNode)
        {
            if(instructionsBlockNode == null)
            {
                // wyjatek instructionsBlockNode = null
                Console.WriteLine("instructionsBlockNode = null");
            }
            instructionsBlockNode.instructionsList.Accept(this);
        }

        public void Visit(InstructionsListNode instructionsListNode)
        {
            if (instructionsListNode == null)
            {
                // wyjatek instructionsListNode = null
                Console.WriteLine("instructionsListNode = null");
            }
            foreach(IInstructionNode instruction in instructionsListNode.instructionNodes) 
            {
                instruction.Accept(this);
                if (IsFunctionReturned)
                {
                    return;
                }
            }
        }

        public void Visit(VariableDefinitionNode variableDefinitionNode)
        {
            FunctionScope currentScope = functionCallContexts.Peek().functionScopes.Peek();
            if (VariableExistsInCurrentFCC(variableDefinitionNode.identifier)) 
            {
                //wyjatek zmienna juz istnieje a sprobojemy zdefionowac ją ponownie
                Console.WriteLine("variable with this name already exists - " + variableDefinitionNode.identifier);
            }
            if(variableDefinitionNode.variableType == TokenType.INT) 
            {
                currentScope.IntVariables.Add(variableDefinitionNode.identifier, 0);
                return;
            }
            else if(variableDefinitionNode.variableType == TokenType.DOUBLE) 
            {
                currentScope.DoubleVariables.Add(variableDefinitionNode.identifier, 0);
                return;
            }

        }
        

        public void Visit(IfNode ifNode)
        {
            if(ifNode == null) 
            {
                //wyjatek ifNode jest pusty
            }
            lastExpressionValues.Add(new LastExpressionValueInt(1));
            ifNode.expressionNode.Accept(this);
            bool ifVisited = false;
            double value=GetLastExpressionValue();
            ConsumeLastExpressionValue();
            if(value > 0) 
            {
                ifVisited = true;
                functionCallContexts.Peek().functionScopes.Push(new FunctionScope());
                ifNode.instructionsBlockNode.Accept(this);
                if (IsFunctionReturned) return;
                functionCallContexts.Peek().functionScopes.Pop();
            }
            if(!ifVisited && ifNode.elseNode != null) 
            {
                ifNode.elseNode.Accept(this);
            }
        }

        public void Visit(ElseNode elseNode)
        {
            functionCallContexts.Peek().functionScopes.Push(new FunctionScope());
            elseNode.InstructionsBlockNode.Accept(this);
            if (IsFunctionReturned)
            {
                return;
            }
            functionCallContexts.Peek().functionScopes.Pop();
        }

        public void Visit(WhileNode whileNode)
        {
            if(whileNode == null)
            {
                //wyjatek
            }
            lastExpressionValues.Add(new LastExpressionValueInt(1));
            whileNode.expressionNode.Accept(this);
            double value = GetLastExpressionValue();
            ConsumeLastExpressionValue();
            while(value > 0) 
            {
                functionCallContexts.Peek().functionScopes.Push(new FunctionScope());
                whileNode.instructionsBlockNode.Accept(this);
                if (IsFunctionReturned)
                {
                    return;
                }
                functionCallContexts.Peek().functionScopes.Pop();
                whileNode.expressionNode.Accept(this);
            }
        }

        public void Visit(IdentifierAssignmentOrInvocationNode identifierAssignmentOrInvocationNode) 
        {
            //przypisanie zmiennej 
            if(identifierAssignmentOrInvocationNode.varAssignmentOrFuncInvocationNode.expression != null) 
            {
                string varName = identifierAssignmentOrInvocationNode.identifier;
                identifierAssignmentOrInvocationNode.varAssignmentOrFuncInvocationNode.Accept(this);
                if(!TrySetValueOfVariable(varName,GetLastExpressionType(),GetLastExpressionValue())) 
                {
                    //wyjatek - nie ma zmiennej o nazwie varName albo sprobojemy przypisac do zmiennej wartosc innego typu
                }
                ConsumeLastExpressionValue();
            }
            //wywolanie funkcji
            else if(identifierAssignmentOrInvocationNode.varAssignmentOrFuncInvocationNode.parametersListNode != null) 
            {
                if (!FunctionDefinitionExists(identifierAssignmentOrInvocationNode.identifier)) 
                {
                    // wyjatek - nie ma definicji funkcji z taką nazwą 
                }
                functionCallContexts.Push(new FunctionCallContext(identifierAssignmentOrInvocationNode.identifier));
                functionCallContexts.Peek().functionScopes.Push(new FunctionScope());
                identifierAssignmentOrInvocationNode.varAssignmentOrFuncInvocationNode.Accept(this);
                functionCallContexts.Pop();
            }
        }

        public void Visit(VarAssignmentOrFuncInvocationNode varAssignmentOrFuncInvocationNode) 
        {
            varAssignmentOrFuncInvocationNode.expression?.Accept(this);
            varAssignmentOrFuncInvocationNode.parametersListNode?.Accept(this);
        }

        public void Visit(ParametersListNode parametersListNode) 
        {
            FunctionNode funcDef = GetFunctionDefinition();
            if (parametersListNode.expressionFunctionParameters.Count != funcDef.argumentsListNodes.variableDefinitionNodes.Count()) 
            {
                //wyjatek arguments count is not the same in definition and invocation
            }
            int ParameterIndex = 0;
            foreach(IExpressionNode expression in parametersListNode.expressionFunctionParameters) 
            {
                expression.Accept(this);
                double Argvalue = GetLastExpressionValue();
                ValueType ArgType = GetLastExpressionType();
                ConsumeLastExpressionValue();
                if(ParameterIndex == funcDef.argumentsListNodes.variableDefinitionNodes.Count) 
                {
                    //wyjatek definicja funkcji ma mniej parametrow niz w wywolaniu 
                }
                if (VariableExistsInCurrentFCC(funcDef.argumentsListNodes.variableDefinitionNodes[ParameterIndex].identifier))
                {
                    // wyjatek juz istnieje zmienna w FCC
                }
                if (funcDef.argumentsListNodes.variableDefinitionNodes[ParameterIndex].variableType == TokenType.INT && ArgType == ValueType.INT) 
                {
                    functionCallContexts.Peek().functionScopes.Peek().IntVariables.Add(funcDef.argumentsListNodes.variableDefinitionNodes[ParameterIndex].identifier, (int)Argvalue);
                }
                else if (funcDef.argumentsListNodes.variableDefinitionNodes[ParameterIndex].variableType == TokenType.DOUBLE && ArgType == ValueType.DOUBLE)
                {
                    functionCallContexts.Peek().functionScopes.Peek().DoubleVariables.Add(funcDef.argumentsListNodes.variableDefinitionNodes[ParameterIndex].identifier, Argvalue);
                }
                else 
                {
                    // wyjatek - typ argumentu w definicji funkcji się nie zgadza z type argumentu w wywolaniu
                }
                ParameterIndex++;
            }
            funcDef.instructionsBlockNode.Accept(this);
            if (IsFunctionReturned)
            {
                IsFunctionReturned = false;
                return;
            }
            else 
            {
                // wyjatek - funkcja nic nie zwraca
            }
        }

        public void Visit(ReturnNode returnNode) 
        {
            if(returnNode == null) 
            {
                //wyjatek returnNode jest pusty 
            }
            returnNode.expression.Accept(this);
            ValueType returnType = GetLastExpressionType();
            if (!IsFunctionReturnTypeCorrect(returnType)) 
            {
                // wyjatek - zwracany typ wartosci nie zgadza się ze zwracanym typem funkcji
            }
            IsFunctionReturned = true;
        }


        //Expressions

        public void Visit(SimpleIntNode simpleIntNode)
        {
            if(simpleIntNode == null) 
            {
                //wyjatek jest pusty 
            }
            ConsumeLastExpressionValue();
            lastExpressionValues.Add(new LastExpressionValueInt(simpleIntNode.value));
        }
        public void Visit(SimpleDoubleNode simpleDoubleNode) 
        {
            if(simpleDoubleNode == null) 
            {
                // jest pusty
            }
            ConsumeLastExpressionValue();
            lastExpressionValues.Add(new LastExpressionValueDouble(simpleDoubleNode.value));
        }
        public void Visit(SimpleIdentifierNode simpleIdentifierNode)
        {
            if(simpleIdentifierNode == null) 
            {
                //jest pusty
            }
            ConsumeLastExpressionValue();
            foreach (FunctionScope functionScope in functionCallContexts.Peek().functionScopes)
            {
                int IntValue;
                if(functionScope.IntVariables.TryGetValue(simpleIdentifierNode.value,out IntValue)) 
                {
                    lastExpressionValues.Add(new LastExpressionValueInt(IntValue));
                    return;
                }
                double ValueDouble;
                if (functionScope.DoubleVariables.TryGetValue(simpleIdentifierNode.value, out ValueDouble))
                {
                    lastExpressionValues.Add(new LastExpressionValueDouble(ValueDouble));
                    return;
                }
                else 
                {
                    // wyjatek - nie ma zmiennej o nazwie 
                }
            }
            
        }

        public void Visit(AndExpressionNode andExpressionNode)
        {
            if(andExpressionNode == null) 
            {
                // pusty Node
            }
            andExpressionNode.left.Accept(this);
            double leftValue = GetLastExpressionValue();
            ConsumeLastExpressionValue();
            andExpressionNode.right.Accept(this);
            double rightValue = GetLastExpressionValue();
            ConsumeLastExpressionValue();
            if(leftValue > 0 && rightValue > 0) 
            {
                lastExpressionValues.Add(new LastExpressionValueInt(1));
            }
            else 
            {
                lastExpressionValues.Add(new LastExpressionValueInt(0));
            }
        }

        public void Visit(OrExpressionNode orExpressionNode)
        {
            if (orExpressionNode == null)
            {
                // pusty Node
            }
            orExpressionNode.left.Accept(this);
            double leftValue = GetLastExpressionValue();
            ConsumeLastExpressionValue();
            orExpressionNode.right.Accept(this);
            double rightValue = GetLastExpressionValue();
            ConsumeLastExpressionValue();
            if (leftValue > 0 || rightValue > 0)
            {
                lastExpressionValues.Add(new LastExpressionValueInt(1));
            }
            else
            {
                lastExpressionValues.Add(new LastExpressionValueInt(0));
            }
        }

        public void Visit(LogicNegationExpressionNode logicNegationExpressionNode)
        {
            if (logicNegationExpressionNode == null)
            {
                // pusty Node
            }
            logicNegationExpressionNode.left.Accept(this);
            double leftValue = GetLastExpressionValue();
            ValueType valueType = GetLastExpressionType();
            ConsumeLastExpressionValue();
            if(valueType != ValueType.INT) 
            {
                //wyjatek - logic negations na double , powinno być int 0 lub 1
            }
            if(leftValue == 0) 
            {
                lastExpressionValues.Add(new LastExpressionValueInt(1));
            }
            else if(leftValue == 1) 
            {
                lastExpressionValues.Add(new LastExpressionValueInt(0));
            }
            else 
            {
                //wyjatek - negacja dla wartosci roznej od 0 lub 1
            }
        }

        public void Visit(ComparisonExpressionNode comparisonExpressionNode)
        {
            if (comparisonExpressionNode == null)
            {
                //wyjatek jest pusty 
            }
            comparisonExpressionNode.left.Accept(this);
            double leftValue = GetLastExpressionValue();
            ConsumeLastExpressionValue();
            comparisonExpressionNode.right.Accept(this);
            double rightValue = GetLastExpressionValue();
            ConsumeLastExpressionValue();
            switch (comparisonExpressionNode.relativeOperator.text)
            {
                case ">":
                    if (leftValue > rightValue)
                    {
                        lastExpressionValues.Add(new LastExpressionValueInt(1));
                    }
                    else
                    {
                        lastExpressionValues.Add(new LastExpressionValueInt(0));
                    }
                    break;
                case ">=":
                    if (leftValue >= rightValue)
                    {
                        lastExpressionValues.Add(new LastExpressionValueInt(1));
                    }
                    else
                    {
                        lastExpressionValues.Add(new LastExpressionValueInt(0));
                    }
                    break;
                case "<":
                    if (leftValue < rightValue)
                    {
                        lastExpressionValues.Add(new LastExpressionValueInt(1));
                    }
                    else
                    {
                        lastExpressionValues.Add(new LastExpressionValueInt(0));
                    }
                    break;
                case "<=":
                    if (leftValue <= rightValue)
                    {
                        lastExpressionValues.Add(new LastExpressionValueInt(1));
                    }
                    else
                    {
                        lastExpressionValues.Add(new LastExpressionValueInt(0));
                    }
                    break;
                case "==":
                    if (leftValue == rightValue)
                    {
                        lastExpressionValues.Add(new LastExpressionValueInt(1));
                    }
                    else
                    {
                        lastExpressionValues.Add(new LastExpressionValueInt(0));
                    }
                    break;
                case "!=":
                    if (leftValue != rightValue)
                    {
                        lastExpressionValues.Add(new LastExpressionValueInt(1));
                    }
                    else
                    {
                        lastExpressionValues.Add(new LastExpressionValueInt(0));
                    }
                    break;
            }

        }
        public void Visit(AddSubExpressionNode addSubExpressionNode)
        {
            if (addSubExpressionNode == null)
            {
                // pusty Node
            }
            addSubExpressionNode.left.Accept(this);
            double leftValue = GetLastExpressionValue();
            ValueType leftValueType = GetLastExpressionType();
            ConsumeLastExpressionValue();

            addSubExpressionNode.right.Accept(this);
            double rightValue = GetLastExpressionValue();
            ValueType rightValueType = GetLastExpressionType();
            ConsumeLastExpressionValue();
            if(leftValueType == ValueType.DOUBLE || rightValueType == ValueType.DOUBLE) 
            {
                if(addSubExpressionNode.operatorToken.text == "+") 
                {
                    lastExpressionValues.Add(new LastExpressionValueDouble(leftValue + rightValue));
                    return;
                }
                if (addSubExpressionNode.operatorToken.text == "-")
                {
                    lastExpressionValues.Add(new LastExpressionValueDouble(leftValue - rightValue));
                    return;
                }
            }
            else 
            {
                if (addSubExpressionNode.operatorToken.text == "+")
                {
                    lastExpressionValues.Add(new LastExpressionValueInt(((int)leftValue + (int)rightValue)));
                    return;
                }
                if (addSubExpressionNode.operatorToken.text == "-")
                {
                    lastExpressionValues.Add(new LastExpressionValueInt(((int)leftValue - (int)rightValue)));
                    return;
                }
            }
        }

        public void Visit(MulDivExpressionNode mulDivExpressionNode)
        {
            if (mulDivExpressionNode == null)
            {
                // pusty Node
            }
            mulDivExpressionNode.left.Accept(this);
            double leftValue = GetLastExpressionValue();
            ValueType leftValueType = GetLastExpressionType();
            ConsumeLastExpressionValue();

            mulDivExpressionNode.right.Accept(this);
            double rightValue = GetLastExpressionValue();
            ValueType rightValueType = GetLastExpressionType();
            ConsumeLastExpressionValue();
            if (leftValueType == ValueType.DOUBLE || rightValueType == ValueType.DOUBLE)
            {
                if (mulDivExpressionNode.operatorToken.text == "*")
                {
                    lastExpressionValues.Add(new LastExpressionValueDouble(leftValue * rightValue));
                    return;
                }
                if (mulDivExpressionNode.operatorToken.text == "/")
                {
                    lastExpressionValues.Add(new LastExpressionValueDouble(leftValue / rightValue));
                    return;
                }
            }
            else
            {
                if (mulDivExpressionNode.operatorToken.text == "*")
                {
                    lastExpressionValues.Add(new LastExpressionValueInt(((int)leftValue * (int)rightValue)));
                    return;
                }
                if (mulDivExpressionNode.operatorToken.text == "/")
                {
                    lastExpressionValues.Add(new LastExpressionValueInt(((int)leftValue / (int)rightValue)));
                    return;
                }
            }
        }

        public void Visit(UnaryExpressionNode unaryExpressionNode)
        {
            if (unaryExpressionNode == null)
            {
                // pusty Node
            }
            unaryExpressionNode.left.Accept(this);
            double leftValue = GetLastExpressionValue();
            ValueType leftValueType = GetLastExpressionType();
            ConsumeLastExpressionValue();
            if(leftValueType == ValueType.INT) 
            {
                lastExpressionValues.Add(new LastExpressionValueInt((int)(-leftValue)));
            }
            else 
            {
                lastExpressionValues.Add(new LastExpressionValueDouble((-leftValue)));
            }
        }

        

        //Methods

        public bool VariableExistsInCurrentFCC(string name)
        {
            bool exists = false;
            foreach (FunctionScope functionScope in functionCallContexts.Peek().functionScopes)
            {
                if (functionScope.IntVariables.ContainsKey(name)) exists = true;
                if (functionScope.DoubleVariables.ContainsKey(name)) exists = true;
            }
            return exists;
        }
        ValueType GetLastExpressionType() 
        {
            if (lastExpressionValues.Count != 1)
            {
                Console.WriteLine("0 or 2+ Expression Values in the list");
            }
            return lastExpressionValues[0].GetValueType();
        }
        double GetLastExpressionValue() 
        {
            if (lastExpressionValues.Count != 1)
            {
                Console.WriteLine("0 or 2+ Expression Values in the list");
            }
            ValueType valueType = GetLastExpressionType();
            switch (valueType) 
            {
                case ValueType.INT:
                    return ((LastExpressionValueInt)lastExpressionValues[0]).LastValue;
                case ValueType.DOUBLE:
                    return ((LastExpressionValueDouble)lastExpressionValues[0]).LastValue;
            }
            return 0;
        }
        void ConsumeLastExpressionValue()
        {
            lastExpressionValues.Clear();
        }

        bool TrySetValueOfVariable(string name,ValueType valueType, double value) 
        {
            foreach (FunctionScope functionScope in functionCallContexts.Peek().functionScopes)
            {
                if (valueType == ValueType.INT && functionScope.IntVariables.ContainsKey(name)) 
                {
                    functionScope.IntVariables[name] = (int)value;
                    return true;
                }
                if (valueType == ValueType.DOUBLE && functionScope.DoubleVariables.ContainsKey(name)) 
                {
                    functionScope.DoubleVariables[name] = value;
                    return true;
                }
            }
            return false;
        }
        bool FunctionDefinitionExists(string FuncName) 
        {
            bool exists = false;
            foreach(FunctionNode functionNode in functionDefinitions) 
            {
                if(functionNode.identifier == FuncName) 
                {
                    exists = true;
                }
            }
            return exists;
        }

        FunctionNode GetFunctionDefinition()
        {
            foreach (FunctionNode functionNode in functionDefinitions)
            {
                if (functionNode.identifier == functionCallContexts.Peek().functionName)
                {
                    return functionNode;
                }
            }
            return null;
        }
        bool IsFunctionReturnTypeCorrect(ValueType returnType) 
        {
            string funcName = functionCallContexts.Peek().functionName;
            foreach(FunctionNode functionNode in functionDefinitions) 
            {
                if(functionNode.identifier == funcName) 
                {
                    if (functionNode.returnType == TokenType.INT && returnType == ValueType.INT) return true;
                    if(functionNode.returnType == TokenType.DOUBLE && returnType == ValueType.DOUBLE) return true;
                }
            }
            return false;
        }
        
    }
}
