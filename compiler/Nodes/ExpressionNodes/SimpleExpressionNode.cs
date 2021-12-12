using compiler.Nodes.InstructionNodes;
using compiler.Nodes.Interfaces;
using compiler.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes.ExpressionNodes
{
    public class SimpleExpressionNode : IExpressionNode
    {
        public Token doubleNumberToken;
        public Token intNumberToken;
        public Token identifierToken;
        public IExpressionNode expression;
        public IdentifierAssignmentOrInvocationNode funcInvocNode;

        public SimpleExpressionNode(Token token)
        {
            if (token.tokenType == TokenType.NUMBER_DOUBLE) 
            {
                doubleNumberToken = token;
            }
            if (token.tokenType == TokenType.NUMBER_INT)
            {
                intNumberToken = token;
            }
            if (token.tokenType == TokenType.IDENTIFIER)
            {
                identifierToken = token;
            }
        }

        public SimpleExpressionNode(IExpressionNode expression)
        {
            this.expression = expression;
        }

        public SimpleExpressionNode(IdentifierAssignmentOrInvocationNode funcInvocNode)
        {
            this.funcInvocNode = funcInvocNode;
        }
    }
}
