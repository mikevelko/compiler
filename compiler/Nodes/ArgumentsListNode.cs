using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes
{
    public class ArgumentsListNode
    {
        public List<VariableDefinitionNode> variableDefinitionNodes;
        public ArgumentsListNode(List<VariableDefinitionNode> variableDefinitionNodes) 
        {
            this.variableDefinitionNodes = variableDefinitionNodes;
        }
    }
}
