using compiler.Interpreter.Visitor;
using compiler.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes
{
    public class InstructionsBlockNode : INode
    {
        public InstructionsListNode instructionsList;

        public InstructionsBlockNode(InstructionsListNode instructionsList)
        {
            this.instructionsList = instructionsList;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
