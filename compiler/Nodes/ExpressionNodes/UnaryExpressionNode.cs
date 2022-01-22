using compiler.Interpreter.Visitor;
using compiler.Nodes.Interfaces;
using compiler.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes.ExpressionNodes
{
    public class UnaryExpressionNode : IExpressionNode, INode
    {
        public IExpressionNode left;
        public (int, int) position;

        public UnaryExpressionNode(IExpressionNode left, Token operatorToken)
        {
            this.left = left;
            this.position = operatorToken.position;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
