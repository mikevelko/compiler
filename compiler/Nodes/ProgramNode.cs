using compiler.Interpreter.Visitor;
using compiler.Nodes.Interfaces;
using compiler.Scanners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes
{
    public class ProgramNode : INode
    {
        public List<FunctionNode> functionNodes;
        public ProgramNode(List<FunctionNode> functionNodes) 
        {
            this.functionNodes = functionNodes;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
