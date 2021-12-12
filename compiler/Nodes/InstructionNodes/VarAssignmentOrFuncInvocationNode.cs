using compiler.Nodes.Interfaces;
using compiler.Tokens;
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
        public Token operatorAssignment;
        public IdentifierListNode identifierListNode;

        public VarAssignmentOrFuncInvocationNode(IExpressionNode expression, Token operatorAssignment)
        {
            this.expression = expression;
            this.operatorAssignment = operatorAssignment;
        }

        public VarAssignmentOrFuncInvocationNode(IdentifierListNode identifierListNode)
        {
            this.identifierListNode = identifierListNode;
        }
    }
}
