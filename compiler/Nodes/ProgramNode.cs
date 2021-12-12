using compiler.Scanners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes
{
    public class ProgramNode
    {
        public List<FunctionNode> functionNodes;
        public ProgramNode(List<FunctionNode> functionNodes) 
        {
            this.functionNodes = functionNodes;
        }

        
    }
}
