using compiler.Interpreter.Visitor;
using compiler.Nodes.Interfaces;
using compiler.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes.ExpressionNodes.SimpleExpressionNodes
{
    public class SimpleDoubleNode : IExpressionNode, INode
    {
        public double value;
        public (int, int) position;

        public SimpleDoubleNode(Token token)
        {
            this.value = token.doubleValue;
            this.position = token.position;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
