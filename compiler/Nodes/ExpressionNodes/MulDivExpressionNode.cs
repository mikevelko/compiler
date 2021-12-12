using compiler.Nodes.Interfaces;
using compiler.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes.ExpressionNodes
{
    public class MulDivExpressionNode : IExpressionNode
    {
        IExpressionNode left;
        IExpressionNode right;
        Token operatorToken;

        public MulDivExpressionNode(IExpressionNode left, IExpressionNode right, Token operatorToken)
        {
            this.left = left;
            this.right = right;
            this.operatorToken = operatorToken;
        }
    }
}
