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
        public ArgumentsListNode argumentsListNodes;
        public InstructionsBlockNode instructionsBlockNode;

        public FunctionNode(TokenType returnType, string identifier, ArgumentsListNode argumentsListNodes, InstructionsBlockNode instructionsBlockNode)
        {
            this.returnType = returnType;
            this.identifier = identifier;
            this.argumentsListNodes = argumentsListNodes;
            this.instructionsBlockNode = instructionsBlockNode;
        }
    }
}
