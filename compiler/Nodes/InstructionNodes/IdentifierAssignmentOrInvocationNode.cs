using compiler.Interpreter.Visitor;
using compiler.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes.InstructionNodes
{
    public class IdentifierAssignmentOrInvocationNode : IInstructionNode, IExpressionNode, INode
    {
        public string identifier;
        public VarAssignmentOrFuncInvocationNode varAssignmentOrFuncInvocationNode;

        public IdentifierAssignmentOrInvocationNode(string identifier, VarAssignmentOrFuncInvocationNode varAssignmentOrFuncInvocationNode)
        {
            this.identifier = identifier;
            this.varAssignmentOrFuncInvocationNode = varAssignmentOrFuncInvocationNode;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
