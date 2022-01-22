using compiler.Interpreter.Visitor;
using compiler.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes.InstructionNodes
{
    public class ParametersListNode : INode
    {
        public List<IExpressionNode> expressionFunctionParameters;

        public ParametersListNode(List<IExpressionNode> identifiers)
        {
            this.expressionFunctionParameters = identifiers;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
