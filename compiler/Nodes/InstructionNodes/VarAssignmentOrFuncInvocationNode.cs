using compiler.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes.InstructionNodes
{
    public class VarAssignmentOrFuncInvocationNode
    {
        public IExpressionNode expression;
        public IdentifierListNode identifierListNode;

        public VarAssignmentOrFuncInvocationNode(IExpressionNode expression)
        {
            this.expression = expression;
        }

        public VarAssignmentOrFuncInvocationNode(IdentifierListNode identifierListNode)
        {
            this.identifierListNode = identifierListNode;
        }
    }
}
