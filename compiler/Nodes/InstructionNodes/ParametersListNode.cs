using compiler.Interpreter.Visitor;
using compiler.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes.InstructionNodes
{
    public class ParametersListNode : INode
    {
        public List<IExpressionNode> identifiers;

        public ParametersListNode(List<IExpressionNode> identifiers)
        {
            this.identifiers = identifiers;
        }

        public void Accept(IVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
