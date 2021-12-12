using compiler.CharReader;
using compiler.Nodes;
using compiler.Nodes.ExpressionNodes;
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
        Scanner scanner;
        SyntaxTree syntaxTree;
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
                //obsluga bledu
            }
            scanner.NextToken();
            if(scanner.token.tokenType != TokenType.LEFT_ROUND_BRACKET) 
            {
                //obsluga bledu nawiasu
            }
            scanner.NextToken();
            ArgumentsListNode argumentsListNode = CreateArgumentsListNode();
            if(scanner.token.tokenType != TokenType.RIGHT_ROUND_BRACKET) 
            {
                //obsluga bledu
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
                }
                return new VariableDefinitionNode(typeName.tokenType, identifier.text);
            }
            else 
            {
                return null;
            }
        }
        private InstructionsBlockNode CreateInstructionsBlockNode() 
        {
            scanner.NextToken();
            if(scanner.token.tokenType != TokenType.LEFT_CURLY_BRACKET) 
            {
                // obsluga bledu 
            }
            scanner.NextToken();
            InstructionsListNode instructionsListNode = CreateInstructionsListNode();

            if(scanner.token.tokenType != TokenType.RIGHT_CURLY_BRACKET) 
            {
                //obsluga bledu
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
                return CreateVariableDefinitionNode();
            }
            else if(scanner.token.tokenType == TokenType.IDENTIFIER) 
            {

            }
            return null;
        }
        private IfNode CreateIfNode() 
        {
            scanner.NextToken();
            if(scanner.token.tokenType != TokenType.LEFT_ROUND_BRACKET) 
            {
                // obsluga bledu
            }
            scanner.NextToken();
            IExpressionNode expression = TryToCreateExpressionNode();
            if(scanner.token.tokenType != TokenType.RIGHT_ROUND_BRACKET) 
            {
                //obsluga bledu
            }
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
            InstructionsBlockNode instructionsBlockNode = CreateInstructionsBlockNode();
            return new ElseNode(instructionsBlockNode);
        }
        private WhileNode CreateWhileNode() 
        {
            scanner.NextToken();
            if(scanner.token.tokenType != TokenType.LEFT_ROUND_BRACKET) 
            {
                //obsluga bledu
            }
            IExpressionNode expression = TryToCreateExpressionNode();
            if(scanner.token.tokenType != TokenType.RIGHT_ROUND_BRACKET) 
            {
                // obsluga bledu
            }
            InstructionsBlockNode instructionsBlockNode = CreateInstructionsBlockNode();
            return new WhileNode(expression, instructionsBlockNode);
        }
        private ReturnNode CreateReturnNode() 
        {
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
                scanner.NextToken();
                return new VarAssignmentOrFuncInvocationNode(TryToCreateExpressionNode());
            }
            else if(scanner.token.tokenType == TokenType.LEFT_ROUND_BRACKET) 
            {
                scanner.NextToken();
                return new VarAssignmentOrFuncInvocationNode(CreateIdentifierListNode());
            }
            else 
            {
                // obsluga bledu
                return null;
            }
        }
        private IdentifierListNode CreateIdentifierListNode() 
        {
            List<string> identifiers = new();
            while(scanner.token.tokenType == TokenType.IDENTIFIER) 
            {
                identifiers.Add(scanner.token.text);
                scanner.NextToken();
                if (scanner.token.tokenType == TokenType.COMMA)
                {
                    scanner.NextToken();
                    if (scanner.token.tokenType == TokenType.IDENTIFIER)
                    {
                        continue;
                    }
                    else 
                    {
                        //obsluga bledu
                    }
                }
                if(scanner.token.tokenType == TokenType.RIGHT_ROUND_BRACKET) 
                {
                    break;
                }
                else
                {
                    //obsluga bledu
                }

            }
            return new IdentifierListNode(identifiers);
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
                IExpressionNode right = TryToCreateAndExpressionNode();
                if(right == null) 
                {
                    //nie ma prawego - wyjatek
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
                IExpressionNode right = TryToCreateLogicNegationExpressionNode();
                if (right == null)
                {
                    //nie ma prawego - wyjatek
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
                IExpressionNode right = TryToCreateAndExpressionNode();
                if (right == null)
                {
                    //nie ma prawego - wyjatek
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
                IExpressionNode right = TryToCreateMulDivExpressionNode();
                if (right == null)
                {
                    //nie ma prawego - wyjatek
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
                IExpressionNode right = TryToCreateUnaryExpressionNode();
                if (right == null)
                {
                    //nie ma prawego - wyjatek
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
                return new UnaryExpressionNode(TryToCreateSimpleExpression(), operatorToken);
            }
            return TryToCreateSimpleExpression();
        }
        private IExpressionNode TryToCreateSimpleExpression() 
        {
            if(scanner.token.tokenType == TokenType.IDENTIFIER) 
            {
                IExpressionNode functionInvoc = CreateIdentifierAssignmentOrInvocationNode();
                if(functionInvoc != null) 
                {

                }
            }
            if(scanner.token.tokenType == TokenType.NUMBER_INT || scanner.token.tokenType == TokenType.NUMBER_DOUBLE || scanner.token.tokenType == TokenType.IDENTIFIER) 
            {
                Token token = scanner.token;
                scanner.NextToken();
                return new SimpleExpressionNode(token);
            }
            if(scanner.token.tokenType == TokenType.LEFT_ROUND_BRACKET) 
            {
                scanner.NextToken();
                IExpressionNode expression = TryToCreateExpressionNode();
                scanner.NextToken();
                if(scanner.token.tokenType != TokenType.RIGHT_ROUND_BRACKET) 
                {
                    // obsluga bledu - brak nawiasu
                }
                scanner.NextToken();
                return expression;
            }


        }
    }
}
