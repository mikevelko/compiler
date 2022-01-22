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
    public class LogicNegationExpressionNode : IExpressionNode, INode
    {
        public IExpressionNode left;
        public (int,int) position;

        public LogicNegationExpressionNode(IExpressionNode left, Token operatorToken)
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
