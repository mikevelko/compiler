using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes
{
    public class InstructionsBlockNode
    {
        public InstructionsListNode instructionsList;

        public InstructionsBlockNode(InstructionsListNode instructionsList)
        {
            this.instructionsList = instructionsList;
        }
    }
}
