using compiler.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes.InstructionNodes
{
    public class ReturnNode : IInstructionNode
    {
        public IExpressionNode expression;

        public ReturnNode(IExpressionNode expression)
        {
            this.expression = expression;
        }
    }
}
