using compiler.Nodes.Interfaces;
using compiler.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes.ExpressionNodes
{
    public class LogicNegationExpressionNode : IExpressionNode
    {
        public IExpressionNode left;
        public Token operatorToken;

        public LogicNegationExpressionNode(IExpressionNode left, Token operatorToken)
        {
            this.left = left;
            this.operatorToken = operatorToken;
        }
    }
}
