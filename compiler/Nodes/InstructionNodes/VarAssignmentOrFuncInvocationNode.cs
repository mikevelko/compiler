using compiler.Interpreter.Visitor;
using compiler.Nodes.Interfaces;
using compiler.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes.InstructionNodes
{
    public class VarAssignmentOrFuncInvocationNode : INode, IInstructionNode
    {
        public IExpressionNode expression;
        public (int, int) position;
        public ParametersListNode identifierListNode;

        public VarAssignmentOrFuncInvocationNode(IExpressionNode expression, Token operatorAssignment)
        {
            this.expression = expression;
            this.position = operatorAssignment.position;
        }

        public VarAssignmentOrFuncInvocationNode(ParametersListNode identifierListNode)
        {
            this.identifierListNode = identifierListNode;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
