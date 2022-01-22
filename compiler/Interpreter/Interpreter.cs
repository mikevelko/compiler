using compiler.Interpreter.Visitor;
using compiler.Nodes;
using compiler.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Interpreter
{
    public class InterpreterClass
    {
        
        public ProgramNode programNode;
        public IVisitor visitor;

        public InterpreterClass(SyntaxTree tree , IVisitor visitor) 
        {
            this.visitor = visitor;
            this.programNode = tree.rootNode;
        }

        public void Start() 
        {
            programNode.Accept(visitor);
        }
    }
}
