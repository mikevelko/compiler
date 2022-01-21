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
    public class SimpleIntNode : IExpressionNode, INode
    {
        public int value;
        public (int, int) position;

        public SimpleIntNode(Token token)
        {
            this.value = token.intValue;
            this.position = token.position;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
