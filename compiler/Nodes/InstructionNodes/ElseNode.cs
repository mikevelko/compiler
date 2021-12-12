using compiler.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes.InstructionNodes
{
    public class ElseNode : IInstructionNode
    {
        public InstructionsBlockNode InstructionsBlockNode;

        public ElseNode(InstructionsBlockNode instructionsBlockNode)
        {
            InstructionsBlockNode = instructionsBlockNode;
        }
    }
}
