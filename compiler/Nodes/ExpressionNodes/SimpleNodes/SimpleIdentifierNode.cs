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
    public class SimpleIdentifierNode : IExpressionNode, INode
    {
        public string value;
        public (int, int) position;

        public SimpleIdentifierNode(Token token)
        {
            this.value = token.text;
            this.position = token.position;
        }

        public void Accept(IVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
