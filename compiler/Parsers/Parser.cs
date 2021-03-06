using compiler.CharReader;
using compiler.Nodes;
using compiler.Nodes.ExpressionNodes;
using compiler.Nodes.ExpressionNodes.SimpleExpressionNodes;
using compiler.Nodes.InstructionNodes;
using compiler.Nodes.Interfaces;
using compiler.Scanners;
using compiler.Tokens;
using compiler.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Parsers
{
    public class Parser : IParser
    {
        public Scanner scanner;
        public SyntaxTree syntaxTree;
        public Parser(Scanner scanner) 
        {
            this.scanner = scanner;
        }

        public void Parse()
        {
            syntaxTree = new SyntaxTree(CreateProgramNode());
        }

        private ProgramNode CreateProgramNode()
        {
            List<FunctionNode> functionList = new();
            scanner.NextToken();
            FunctionNode f = CreateFunctionNode();
            while (f != null) 
            {
                functionList.Add(f);
                if (!scanner.NextToken()) 
                {
                    break;
                }
                f = CreateFunctionNode();
            }
            
            return new ProgramNode(functionList);
            
        }
        private FunctionNode CreateFunctionNode() 
        {
            Token returnValueToken = scanner.token;
            if (returnValueToken.tokenType == TokenType.INT || returnValueToken.tokenType == TokenType.DOUBLE) 
            {
            }
            else 
            {
                return null;
            }
            scanner.NextToken();
            Token identifierToken = scanner.token;
            if(identifierToken.tokenType != TokenType.IDENTIFIER) 
            {
                throw new Exception("Wrong token in " + PositionToString(identifierToken.position) + ". Expected identifier");
            }
            scanner.NextToken();
            if(scanner.token.tokenType != TokenType.LEFT_ROUND_BRACKET) 
            {
                //obsluga bledu nawiasu
                throw new Exception("Wrong token in " + PositionToString(scanner.token.position) + ". Exprected LEFT_ROUND_BRACKET");
            }
            scanner.NextToken();
            ArgumentsListNode argumentsListNode = CreateArgumentsListNode();
            if(scanner.token.tokenType != TokenType.RIGHT_ROUND_BRACKET) 
            {
                throw new Exception("Wrong token in " + PositionToString(scanner.token.position) + ". Exprected RIGHT_ROUND_BRACKET");
            }
            scanner.NextToken();
            InstructionsBlockNode instructionsBlockNode = CreateInstructionsBlockNode();

            return new FunctionNode(returnValueToken.tokenType,identifierToken.text,argumentsListNode,instructionsBlockNode);
        }
        private ArgumentsListNode CreateArgumentsListNode() 
        {
            List<VariableDefinitionNode> variableDefinitionNodes = new();
            VariableDefinitionNode variableDefinitionNode = CreateVariableDefinitionNode();
            while(variableDefinitionNode != null) 
            {
                variableDefinitionNodes.Add(variableDefinitionNode);
                if (scanner.token.tokenType != TokenType.COMMA) 
                {
                    return new ArgumentsListNode(variableDefinitionNodes);
                }
                
                variableDefinitionNode = CreateVariableDefinitionNode();
            }
            return new ArgumentsListNode(variableDefinitionNodes);
        }
        private VariableDefinitionNode CreateVariableDefinitionNode() 
        {
            Token typeName = scanner.token;
            if(typeName.tokenType == TokenType.INT || typeName.tokenType == TokenType.DOUBLE) 
            {
                scanner.NextToken();
                Token identifier = scanner.token;
                if(identifier.tokenType != TokenType.IDENTIFIER) 
                {
                    // obsluga bledu
                    throw new Exception("Wrong token in " + PositionToString(identifier.position) + ". Expected identifier");
                }
                scanner.NextToken();
                return new VariableDefinitionNode(typeName.tokenType, identifier.text);
            }
            else 
            {
                return null;
            }
        }
        private InstructionsBlockNode CreateInstructionsBlockNode() 
        {
            if(scanner.token.tokenType != TokenType.LEFT_CURLY_BRACKET) 
            {
                // obsluga bledu 
                throw new Exception("Wrong token in " + PositionToString(scanner.token.position) + ". Exprected LEFT_CURLY_BRACKET");
            }
            scanner.NextToken();
            InstructionsListNode instructionsListNode = CreateInstructionsListNode();

            if(scanner.token.tokenType != TokenType.RIGHT_CURLY_BRACKET) 
            {
                throw new Exception("Wrong token in " + PositionToString(scanner.token.position) + ". Exprected RIGHT_CURLY_BRACKET");
            }
            return new InstructionsBlockNode(instructionsListNode);
        }
        private InstructionsListNode CreateInstructionsListNode() 
        {
            List<IInstructionNode> instructionNodes = new();
            IInstructionNode instructionNode = CreateInstructionNode();
            while (instructionNode != null) 
            {
                instructionNodes.Add(instructionNode);
                instructionNode = CreateInstructionNode();
            }
            return new InstructionsListNode(instructionNodes);
        }
        private IInstructionNode CreateInstructionNode() 
        {
            if(scanner.token.tokenType == TokenType.IF) 
            {
                return CreateIfNode();
            }
            else if(scanner.token.tokenType == TokenType.WHILE) 
            {
                return CreateWhileNode();
            }
            else if(scanner.token.tokenType == TokenType.RETURN) 
            {
                return CreateReturnNode();
            }
            else if(scanner.token.tokenType == TokenType.INT || scanner.token.tokenType == TokenType.DOUBLE) 
            {
                //scanner.NextToken();
                VariableDefinitionNode variableDefinitionNode = CreateVariableDefinitionNode();
                //scanner.NextToken();
                return variableDefinitionNode;
                
            }
            else if(scanner.token.tokenType == TokenType.IDENTIFIER) 
            {
                return CreateIdentifierAssignmentOrInvocationNode();
            }
            return null;
        }
        private IfNode CreateIfNode() 
        {
            scanner.NextToken();
            if(scanner.token.tokenType != TokenType.LEFT_ROUND_BRACKET) 
            {
                throw new Exception("Wrong token in " + PositionToString(scanner.token.position) + ". Exprected LEFT_ROUND_BRACKET");
            }
            scanner.NextToken();
            IExpressionNode expression = TryToCreateExpressionNode();
            if(scanner.token.tokenType != TokenType.RIGHT_ROUND_BRACKET) 
            {
                //obsluga bledu
            }
            scanner.NextToken();
            InstructionsBlockNode instructionsBlockNode = CreateInstructionsBlockNode();
            ElseNode optionalElseNode = CreateElseNode();
            if(optionalElseNode == null) 
            {
                return new IfNode(expression,instructionsBlockNode);
            }
            return new IfNode(expression,instructionsBlockNode,optionalElseNode);
        }
        private ElseNode CreateElseNode() 
        {
            scanner.NextToken();
            if(scanner.token.tokenType != TokenType.ELSE) 
            {
                return null;
            }
            scanner.NextToken();
            InstructionsBlockNode instructionsBlockNode = CreateInstructionsBlockNode();
            return new ElseNode(instructionsBlockNode);
        }
        private WhileNode CreateWhileNode() 
        {
            scanner.NextToken();
            if(scanner.token.tokenType != TokenType.LEFT_ROUND_BRACKET) 
            {
                //obsluga bledu
                throw new Exception("Wrong token in " + PositionToString(scanner.token.position) + ". Exprected LEFT_ROUND_BRACKET");
            }
            scanner.NextToken();
            IExpressionNode expression = TryToCreateExpressionNode();
            if(scanner.token.tokenType != TokenType.RIGHT_ROUND_BRACKET) 
            {
                // obsluga bledu
                throw new Exception("Wrong token in " + PositionToString(scanner.token.position) + ". Exprected RIGHT_ROUND_BRACKET");
            }
            scanner.NextToken();
            InstructionsBlockNode instructionsBlockNode = CreateInstructionsBlockNode();
            scanner.NextToken();
            return new WhileNode(expression, instructionsBlockNode);
        }
        private ReturnNode CreateReturnNode() 
        {
            scanner.NextToken();
            IExpressionNode expression = TryToCreateExpressionNode();
            return new ReturnNode(expression);
        }
        private IdentifierAssignmentOrInvocationNode CreateIdentifierAssignmentOrInvocationNode() 
        {
            string identifier = scanner.token.text;
            scanner.NextToken();
            VarAssignmentOrFuncInvocationNode varAssignmentOrFuncInvocationNode = CreateVarAssignmentOrFuncInvocationNode();
            return new IdentifierAssignmentOrInvocationNode(identifier, varAssignmentOrFuncInvocationNode);
        }
        private VarAssignmentOrFuncInvocationNode CreateVarAssignmentOrFuncInvocationNode() 
        {
            //var assignment scope
            if(scanner.token.tokenType == TokenType.ASSIGN) 
            {
                Token assignOperator = scanner.token;
                scanner.NextToken();
                return new VarAssignmentOrFuncInvocationNode(TryToCreateExpressionNode(),assignOperator);
            }
            else if(scanner.token.tokenType == TokenType.LEFT_ROUND_BRACKET) 
            {
                scanner.NextToken();
                return new VarAssignmentOrFuncInvocationNode(CreateParametersListNode());
            }
            else 
            {
                // obsluga bledu
                throw new Exception("Wrong token in " + PositionToString(scanner.token.position) + ". Exprected ASSIGN or LEFT_ROUND_BRACKET");
            }
        }
        private ParametersListNode CreateParametersListNode() 
        {
            List<IExpressionNode> parameters = new();
            IExpressionNode expression = TryToCreateExpressionNode();
            while (expression != null) 
            {
                parameters.Add(expression);
                //scanner.NextToken();
                if (scanner.token.tokenType == TokenType.COMMA)
                {
                    scanner.NextToken();
                    expression = TryToCreateExpressionNode();
                    continue;
                }
                if(scanner.token.tokenType == TokenType.RIGHT_ROUND_BRACKET) 
                {
                    scanner.NextToken();
                    break;
                }
                else
                {
                    //obsluga bledu
                    throw new Exception("Wrong token in " + PositionToString(scanner.token.position) + ". Exprected COMMA or RIGHT_ROUND_BRACKET");
                }

            }
            if (scanner.token.tokenType == TokenType.RIGHT_ROUND_BRACKET)
            {
                scanner.NextToken();
            }
            return new ParametersListNode(parameters);
        }
        private IExpressionNode TryToCreateExpressionNode() 
        {
            return TryToCreateOrExpressionNode();
        }
        private IExpressionNode TryToCreateOrExpressionNode() 
        {
            IExpressionNode left = TryToCreateAndExpressionNode();
            if (left == null)
            {
                return null;
            }
            while (scanner.token.tokenType == TokenType.OR)
            {
                Token operatorToken = scanner.token;
                scanner.NextToken();
                IExpressionNode right = TryToCreateAndExpressionNode();
                if(right == null) 
                {
                    //nie ma prawego - wyjatek
                    throw new Exception("Wrong token in " + PositionToString(scanner.token.position) + ". TryToCreateOrExpressionNode - nie ma prawego");
                }
                left = new OrExpressionNode(left, right, operatorToken);
            }
            return left;
        }
        private IExpressionNode TryToCreateAndExpressionNode() 
        {
            IExpressionNode left = TryToCreateLogicNegationExpressionNode();
            if (left == null)
            {
                return null;
            }
            while (scanner.token.tokenType == TokenType.AND)
            {
                Token operatorToken = scanner.token;
                scanner.NextToken();
                IExpressionNode right = TryToCreateLogicNegationExpressionNode();
                if (right == null)
                {
                    //nie ma prawego - wyjatek
                    throw new Exception("Wrong token in " + PositionToString(scanner.token.position) + ". TryToCreateAndExpressionNode - nie ma prawego");
                }
                left = new AndExpressionNode(left, right, operatorToken);
            }
            return left;
        }
        private IExpressionNode TryToCreateLogicNegationExpressionNode() 
        {
            if(scanner.token.tokenType == TokenType.NEGATION) 
            {
                Token operatorToken = scanner.token;
                scanner.NextToken();
                //sprawdzic czy powstanie ComprasionExpressionNode
                return new LogicNegationExpressionNode(TryToCreateComparisonExpressionNode(), operatorToken);
            }
            return TryToCreateComparisonExpressionNode();
        }
        private IExpressionNode TryToCreateComparisonExpressionNode() 
        {
            IExpressionNode left = TryToCreateAddSubExpressionNode();
            if (left == null)
            {
                return null;
            }
            if (scanner.token.tokenType == TokenType.EQUAL || scanner.token.tokenType == TokenType.NOT_EQUAL || scanner.token.tokenType == TokenType.MORE ||
                scanner.token.tokenType == TokenType.MORE_EQUAL || scanner.token.tokenType == TokenType.LESS || scanner.token.tokenType == TokenType.LESS_EQUAL)
            {
                Token operatorToken = scanner.token;
                scanner.NextToken();
                IExpressionNode right = TryToCreateAddSubExpressionNode();
                if (right == null)
                {
                    throw new Exception("Wrong token in " + PositionToString(scanner.token.position) + ". TryToCreateComparisonExpressionNode - nie ma prawego");
                }
                left = new ComparisonExpressionNode(left, right, operatorToken);
            }
            return left;
        }
        private IExpressionNode TryToCreateAddSubExpressionNode() 
        {
            IExpressionNode left = TryToCreateMulDivExpressionNode();
            if (left == null)
            {
                return null;
            }
            while (scanner.token.tokenType == TokenType.PLUS || scanner.token.tokenType == TokenType.MINUS)
            {
                Token operatorToken = scanner.token;
                scanner.NextToken();
                IExpressionNode right = TryToCreateMulDivExpressionNode();
                if (right == null)
                {
                    throw new Exception("Wrong token in " + PositionToString(scanner.token.position) + ". TryToCreateAddSubExpressionNode - nie ma prawego");
                }
                left = new AddSubExpressionNode(left, right, operatorToken);
            }
            return left;
        }
        private IExpressionNode TryToCreateMulDivExpressionNode() 
        {
            IExpressionNode left = TryToCreateUnaryExpressionNode();
            if (left == null)
            {
                return null;
            }
            while (scanner.token.tokenType == TokenType.MULTIPLE || scanner.token.tokenType == TokenType.DIVIDE)
            {
                Token operatorToken = scanner.token;
                scanner.NextToken();
                IExpressionNode right = TryToCreateUnaryExpressionNode();
                if (right == null)
                {
                    throw new Exception("Wrong token in " + PositionToString(scanner.token.position) + ". TryToCreateUnaryExpressionNode - nie ma prawego");
                }
                left = new MulDivExpressionNode(left, right, operatorToken);
            }
            return left;
        }
        private IExpressionNode TryToCreateUnaryExpressionNode() 
        {
            if (scanner.token.tokenType == TokenType.MINUS)
            {
                Token operatorToken = scanner.token;
                scanner.NextToken();
                //to samo sprawdzenie czy jest null
                return new UnaryExpressionNode(TryToCreateSimpleExpression(), operatorToken);
            }
            return TryToCreateSimpleExpression();
        }
        private IExpressionNode TryToCreateSimpleExpression() 
        {
            if(scanner.token.tokenType == TokenType.IDENTIFIER) 
            {
                Token token = scanner.token;
                scanner.NextToken();
                if(scanner.token.tokenType == TokenType.LEFT_ROUND_BRACKET) 
                {
                    VarAssignmentOrFuncInvocationNode varAssignmentOrFuncInvocationNode = CreateVarAssignmentOrFuncInvocationNode();
                    //sprawdzic czy nie jest null
                    return new IdentifierAssignmentOrInvocationNode(token.text, varAssignmentOrFuncInvocationNode);
                }
                else return new SimpleIdentifierNode(token);
            }
            if(scanner.token.tokenType == TokenType.NUMBER_INT ) 
            {
                Token token = scanner.token;
                scanner.NextToken();
                return new SimpleIntNode(token);
            }
            else if (scanner.token.tokenType == TokenType.NUMBER_DOUBLE)
            {
                Token token = scanner.token;
                scanner.NextToken();
                return new SimpleDoubleNode(token);
            }
            if (scanner.token.tokenType == TokenType.LEFT_ROUND_BRACKET) 
            {
                scanner.NextToken();
                IExpressionNode expression = TryToCreateExpressionNode();
                //scanner.NextToken();
                if(scanner.token.tokenType != TokenType.RIGHT_ROUND_BRACKET) 
                {
                    throw new Exception("Wrong token in " + PositionToString(scanner.token.position) + ". Expected RIGHT_ROUND_BRACKET");
                }
                scanner.NextToken();
                return expression;
            }
            //obsluga bledu 
            throw new Exception("Wrong token in " + PositionToString(scanner.token.position) + ". Expected LEFT_ROUND_BRACKET");
        }
        string PositionToString((int line, int posInLine) position) 
        {
            return "Line: " + position.line.ToString() + " Pos in line: " + position.posInLine.ToString();
        }
    }
}
