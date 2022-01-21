using compiler.Interpreter.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes.Interfaces
{
    public interface INode
    {
        void Accept(IVisitor visitor);
    }
}
