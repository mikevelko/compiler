using compiler.Nodes.Interfaces;
using compiler.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes.ExpressionNodes
{
    class ComparisonExpressionNode : IExpressionNode
    {
        IExpressionNode left;
        IExpressionNode right;
        Token relativeOperator;

        public ComparisonExpressionNode(IExpressionNode left, IExpressionNode right, Token relativeOperator)
        {
            this.left = left;
            this.right = right;
            this.relativeOperator = relativeOperator;
        }
    }
}
