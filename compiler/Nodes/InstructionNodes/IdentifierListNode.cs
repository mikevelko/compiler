using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes.InstructionNodes
{
    public class IdentifierListNode
    {
        public List<string> identifiers;

        public IdentifierListNode(List<string> identifiers)
        {
            this.identifiers = identifiers;
        }
    }
}
