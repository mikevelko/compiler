using compiler.Interpreter.Visitor;
using compiler.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes
{
    public class InstructionsListNode : INode
    {
        public List<IInstructionNode> instructionNodes;

        public InstructionsListNode(List<IInstructionNode> instructionNodes)
        {
            this.instructionNodes = instructionNodes;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
