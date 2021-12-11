using compiler.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Trees
{
    public class SyntaxTree
    {
        public ProgramNode rootNode;
        public SyntaxTree(ProgramNode rootNode) 
        {
            this.rootNode = rootNode;
        }
        
    }
}
