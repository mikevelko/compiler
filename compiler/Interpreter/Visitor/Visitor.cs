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
        bool IsPrintInvoked;
        //ILastExpressionValue returnedValue;
        
        public Visitor() 
        {
            functionCallContexts = new Stack<FunctionCallContext>();
            functionDefinitions = new List<FunctionNode>();
            lastExpressionValues = new List<ILastExpressionValue>();
            IsFunctionReturned = false;
            IsPrintInvoked = false;
        }
        public void Visit(ProgramNode programNode)
        {
            FunctionNode main = null;
            foreach (FunctionNode functionNode in programNode.functionNodes) 
            {
                if(functionNode == null) 
                {
                    //wyjatek - jakiś node z def funkcji jest pusty
                    throw new Exception("Some node from function definitions is empty");
                }
                if(functionNode.identifier == "main") 
                {
                    if(main != null) 
                    {
                        //wyjatek - there is more than 1 main function
                        throw new Exception("there is more than 1 main function");
                    }
                    main = functionNode;
                    
                    
                }
                functionDefinitions.Add(functionNode);
            }
            if(main == null) 
            {
                throw new Exception("no main function");
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
                throw new Exception("no return in main");
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
                throw new Exception("instructionsBlockNode = null");
            }
            instructionsBlockNode.instructionsList.Accept(this);
        }

        public void Visit(InstructionsListNode instructionsListNode)
        {
            if (instructionsListNode == null)
            {
                // wyjatek instructionsListNode = null
                throw new Exception("instructionsListNode = null");
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
                throw new Exception("redifine of variable" + variableDefinitionNode.identifier);
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
                throw new Exception("ifNode jest pusty");
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
                throw new Exception("whileNode jest pusty");
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
                value = GetLastExpressionValue();
                ConsumeLastExpressionValue();
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
                    throw new Exception("There is no variable - " + varName + " or we are trying to assign value of another type to it");
                }
                ConsumeLastExpressionValue();
            }
            //wywolanie funkcji
            else if(identifierAssignmentOrInvocationNode.varAssignmentOrFuncInvocationNode.parametersListNode != null) 
            {
                if (!FunctionDefinitionExists(identifierAssignmentOrInvocationNode.identifier) && identifierAssignmentOrInvocationNode.identifier != "print") 
                {
                    // wyjatek - nie ma definicji funkcji z taką nazwą 
                    throw new Exception("There is no func definition : " + identifierAssignmentOrInvocationNode.identifier);
                }
                CreateParametersList(identifierAssignmentOrInvocationNode.varAssignmentOrFuncInvocationNode.parametersListNode, identifierAssignmentOrInvocationNode.identifier);
                if (IsPrintInvoked) 
                {
                    IsPrintInvoked = false;
                    return;
                }
                identifierAssignmentOrInvocationNode.varAssignmentOrFuncInvocationNode.Accept(this);
                functionCallContexts.Pop();
            }
        }

        public void CreateParametersList(ParametersListNode parametersListNode, string functionName) 
        {
            if(functionName == "print") 
            {
                if(parametersListNode.expressionFunctionParameters.Count != 1) 
                {
                    // wyjatek - liczba argumentow funkcji print nie jest 1
                    throw new Exception("print can take only one parameter");
                }
                if(parametersListNode.expressionFunctionParameters[0] is SimpleIdentifierNode) 
                {
                    Console.WriteLine(((SimpleIdentifierNode)parametersListNode.expressionFunctionParameters[0]).value);
                    IsPrintInvoked = true;
                }
                else
                {
                    // wyjatek - zly typ argumentu w print
                    throw new Exception("wrong type in print argument");
                }
                return;
            }

            List<(ValueType type, double value, string identifier)> parameters = new List<(ValueType type, double value, string identifier)>();
            FunctionNode funcDef = GetFunctionDefinition(functionName);
            
            if (parametersListNode.expressionFunctionParameters.Count != funcDef.argumentsListNodes.variableDefinitionNodes.Count())
            {
                //wyjatek arguments count is not the same in definition and invocation
                throw new Exception("arguments count is not the same in definition and invocation");
            }
            int ParameterIndex = 0;
            foreach (IExpressionNode expression in parametersListNode.expressionFunctionParameters)
            {
                expression.Accept(this);
                double Argvalue = GetLastExpressionValue();
                ValueType ArgType = GetLastExpressionType();
                ConsumeLastExpressionValue();
                if (ParameterIndex == funcDef.argumentsListNodes.variableDefinitionNodes.Count)
                {
                    //wyjatek definicja funkcji ma mniej parametrow niz w wywolaniu 
                    throw new Exception("Function definition has less arguments than in invocation");
                }
                if(parameters.FindIndex(t => t.identifier == funcDef.argumentsListNodes.variableDefinitionNodes[ParameterIndex].identifier) >= 0) 
                {
                    //wyjatek definicja funkcji zawiera parametry z taką samą nazwą
                    throw new Exception("Function definition has arguments with the same name");
                }
                if (funcDef.argumentsListNodes.variableDefinitionNodes[ParameterIndex].variableType == TokenType.INT && ArgType == ValueType.INT)
                {
                    parameters.Add((ArgType, Argvalue, funcDef.argumentsListNodes.variableDefinitionNodes[ParameterIndex].identifier));
                }
                else if (funcDef.argumentsListNodes.variableDefinitionNodes[ParameterIndex].variableType == TokenType.DOUBLE && ArgType == ValueType.DOUBLE)
                {
                    parameters.Add((ArgType, Argvalue, funcDef.argumentsListNodes.variableDefinitionNodes[ParameterIndex].identifier));
                }
                else
                {
                    // wyjatek - typ argumentu w definicji funkcji się nie zgadza z type argumentu w wywolaniu
                    throw new Exception(funcDef.argumentsListNodes.variableDefinitionNodes[ParameterIndex].identifier + " - has another type in function invoc than in definition");
                }
                ParameterIndex++;
            }
            functionCallContexts.Push(new FunctionCallContext(functionName));
            functionCallContexts.Peek().functionScopes.Push(new FunctionScope());
            foreach((ValueType type, double value, string identifier) item in parameters) 
            {
                if(item.type == ValueType.INT) 
                {
                    functionCallContexts.Peek().functionScopes.Peek().IntVariables.Add(item.identifier, (int)item.value);
                }
                else if (item.type == ValueType.DOUBLE) 
                {
                    functionCallContexts.Peek().functionScopes.Peek().DoubleVariables.Add(item.identifier, item.value);
                }
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
            funcDef.instructionsBlockNode.Accept(this);
            if (IsFunctionReturned)
            {
                IsFunctionReturned = false;
                return;
            }
            else 
            {
                // wyjatek - funkcja nic nie zwraca
                throw new Exception("Function is not returning anything");
            }
        }

        public void Visit(ReturnNode returnNode) 
        {
            if(returnNode == null) 
            {
                //wyjatek returnNode jest pusty 
                throw new Exception("returnNode is null");
            }
            returnNode.expression.Accept(this);
            ValueType returnType = GetLastExpressionType();
            if (!IsFunctionReturnTypeCorrect(returnType)) 
            {
                // wyjatek - zwracany typ wartosci nie zgadza się ze zwracanym typem funkcji
                throw new Exception("Return value is not the same type as function type");
            }
            IsFunctionReturned = true;
        }


        //Expressions

        public void Visit(SimpleIntNode simpleIntNode)
        {
            if(simpleIntNode == null) 
            {
                //wyjatek jest pusty 
                throw new Exception("simpleIntNode is null");
            }
            ConsumeLastExpressionValue();
            lastExpressionValues.Add(new LastExpressionValueInt(simpleIntNode.value));
        }
        public void Visit(SimpleDoubleNode simpleDoubleNode) 
        {
            if(simpleDoubleNode == null) 
            {
                // jest pusty
                throw new Exception("simpleDoubleNode is null");
            }
            ConsumeLastExpressionValue();
            lastExpressionValues.Add(new LastExpressionValueDouble(simpleDoubleNode.value));
        }
        public void Visit(SimpleIdentifierNode simpleIdentifierNode)
        {
            if(simpleIdentifierNode == null) 
            {
                //jest pusty
                throw new Exception("simpleIdentifierNode is null");
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
            }
            throw new Exception("There is no variable - " + simpleIdentifierNode.value + " in current scope");

        }

        public void Visit(AndExpressionNode andExpressionNode)
        {
            if(andExpressionNode == null) 
            {
                // pusty Node
                throw new Exception("wrong type in print argument");
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
                throw new Exception("wrong type in print argument");
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
                throw new Exception("logicNegationExpressionNode is null");
            }
            logicNegationExpressionNode.left.Accept(this);
            double leftValue = GetLastExpressionValue();
            ValueType valueType = GetLastExpressionType();
            ConsumeLastExpressionValue();
            if(valueType != ValueType.INT) 
            {
                //wyjatek - logic negations na double , powinno być int 0 lub 1
                throw new Exception("Can't apply logic negation to double value");
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
                throw new Exception("Negation applied for value that is not INT(0,1)");
            }
        }

        public void Visit(ComparisonExpressionNode comparisonExpressionNode)
        {
            if (comparisonExpressionNode == null)
            {
                //wyjatek jest pusty 
                throw new Exception("comparisonExpressionNode is null");
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
                throw new Exception("addSubExpressionNode is empty");
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
                throw new Exception("empty mulDivExpressionNode");
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
                throw new Exception("unaryExpressionNode is empty");
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
                throw new Exception("0 or 2+ Expression Values in the list (INTERPRETER ERROR)");
            }
            return lastExpressionValues[0].GetValueType();
        }
        double GetLastExpressionValue() 
        {
            if (lastExpressionValues.Count != 1)
            {
                throw new Exception("0 or 2+ Expression Values in the list (INTERPRETER ERROR)");
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

        FunctionNode GetFunctionDefinition(string name)
        {
            foreach (FunctionNode functionNode in functionDefinitions)
            {
                if (functionNode.identifier == name)
                {
                    return functionNode;
                }
            }
            return null;
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
