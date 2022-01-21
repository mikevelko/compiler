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

        LastExpressionValue lastExpressionValue = null;
        
        public Visitor() 
        {
            functionCallContexts = new Stack<FunctionCallContext>();
            functionDefinitions = new List<FunctionNode>();
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
        }

        public void Visit(FunctionNode functionNode)
        {
            functionCallContexts.Push(new FunctionCallContext(functionNode.identifier));
            FunctionCallContext functionCallContext = functionCallContexts.Peek();
            functionCallContext.functionScopes.Push(new FunctionScope());
            functionNode.instructionsBlockNode.Accept(this);
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
            lastExpressionValue = new LastExpressionValue(1);
            ifNode.expressionNode.Accept(this);
            bool ifVisited = false;
            if(ConsumeLastExpressionValue() > 0) 
            {
                ifVisited = true;
                functionCallContexts.Peek().functionScopes.Push(new FunctionScope());
                ifNode.instructionsBlockNode.Accept(this);
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
            functionCallContexts.Peek().functionScopes.Pop();
        }

        public void Visit(WhileNode whileNode)
        {
            if(whileNode == null)
            {
                //wyjatek
            }
            lastExpressionValue = new LastExpressionValue(1);
            whileNode.expressionNode.Accept(this);
            while(lastExpressionValue.LastValue > 0) 
            {
                functionCallContexts.Peek().functionScopes.Push(new FunctionScope());
                whileNode.instructionsBlockNode.Accept(this);
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
                TrySetValueOfVariable(varName, lastExpressionValue.LastValue);
            }
            else if(identifierAssignmentOrInvocationNode.varAssignmentOrFuncInvocationNode.identifierListNode != null) 
            {
                functionCallContexts.Push(new FunctionCallContext(identifierAssignmentOrInvocationNode.identifier));
            }
        }

        public void Visit(VarAssignmentOrFuncInvocationNode varAssignmentOrFuncInvocationNode) 
        {
            varAssignmentOrFuncInvocationNode?.expression.Accept(this);

        }



        //Expressions

        public void Visit(SimpleIntNode simpleIntNode)
        {
            if(simpleIntNode == null) 
            {
                //wyjatek jest pusty 
            }
            lastExpressionValue.LastValue = simpleIntNode.value;
        }

        public void Visit(ComparisonExpressionNode comparisonExpressionNode)
        {
            if (comparisonExpressionNode == null)
            {
                //wyjatek jest pusty 
            }
            comparisonExpressionNode.left.Accept(this);
            double leftValue = lastExpressionValue.LastValue;
            comparisonExpressionNode.right.Accept(this);
            switch (comparisonExpressionNode.relativeOperator.text) 
            {
                case ">":
                    if(leftValue > lastExpressionValue.LastValue) 
                    {
                        lastExpressionValue.LastValue = 1;
                    }
                    else 
                    {
                        lastExpressionValue.LastValue = 0;
                    }
                    break;
                case ">=":
                    if (leftValue >= lastExpressionValue.LastValue)
                    {
                        lastExpressionValue.LastValue = 1;
                    }
                    else
                    {
                        lastExpressionValue.LastValue = 0;
                    }
                    break;
                case "<":
                    if (leftValue < lastExpressionValue.LastValue)
                    {
                        lastExpressionValue.LastValue = 1;
                    }
                    else
                    {
                        lastExpressionValue.LastValue = 0;
                    }
                    break;
                case "<=":
                    if (leftValue <= lastExpressionValue.LastValue)
                    {
                        lastExpressionValue.LastValue = 1;
                    }
                    else
                    {
                        lastExpressionValue.LastValue = 0;
                    }
                    break;
                case "==":
                    if (leftValue == lastExpressionValue.LastValue)
                    {
                        lastExpressionValue.LastValue = 1;
                    }
                    else
                    {
                        lastExpressionValue.LastValue = 0;
                    }
                    break;
                case "!=":
                    if (leftValue != lastExpressionValue.LastValue)
                    {
                        lastExpressionValue.LastValue = 1;
                    }
                    else
                    {
                        lastExpressionValue.LastValue = 0;
                    }
                    break;
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
        double ConsumeLastExpressionValue()
        {
            double val = lastExpressionValue.LastValue;
            lastExpressionValue = null;
            return val;
        }

        bool TrySetValueOfVariable(string name, double value) 
        {
            bool valueSet = false;
            foreach (FunctionScope functionScope in functionCallContexts.Peek().functionScopes)
            {
                if (functionScope.IntVariables.ContainsKey(name)) 
                {
                    functionScope.IntVariables[name] = (int)value;
                    valueSet = true;
                }
                if (functionScope.DoubleVariables.ContainsKey(name)) 
                {
                    functionScope.DoubleVariables[name] = value;
                    valueSet = true;
                }
            }
            return valueSet;
        }
    }
}
