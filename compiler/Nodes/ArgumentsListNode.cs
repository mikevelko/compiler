using compiler.Interpreter.Visitor;
using compiler.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes
{
    public class ArgumentsListNode : INode
    {
        public List<VariableDefinitionNode> variableDefinitionNodes;
        public ArgumentsListNode(List<VariableDefinitionNode> variableDefinitionNodes) 
        {
            this.variableDefinitionNodes = variableDefinitionNodes;
        }

        public void Accept(IVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
