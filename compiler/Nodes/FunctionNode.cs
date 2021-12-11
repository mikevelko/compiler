using compiler.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes
{
    public class FunctionNode : Node
    {
        public TokenType returnType;
        public string identifier;
        public List<ArgumentsListNode> argumentsListNodes;
        public FunctionNode(List<Node> childrenNodes) : base(childrenNodes) { }
    }
}
