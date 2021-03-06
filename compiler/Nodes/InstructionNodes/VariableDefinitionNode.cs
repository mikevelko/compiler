using compiler.Interpreter.Visitor;
using compiler.Nodes.Interfaces;
using compiler.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes
{
    public class VariableDefinitionNode : IInstructionNode, INode
    {
        public TokenType variableType;
        public string identifier;
        public VariableDefinitionNode(TokenType variableType, string identifier) 
        {
            this.variableType = variableType;
            this.identifier = identifier;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
