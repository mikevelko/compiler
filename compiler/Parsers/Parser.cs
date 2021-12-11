using compiler.CharReader;
using compiler.Nodes;
using compiler.Scanners;
using compiler.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Parsers
{
    public class Parser : IParser
    {
        IScanner scanner;
        SyntaxTree syntaxTree;
        public Parser(IScanner scanner) 
        {
            this.scanner = scanner;
        }

        public SyntaxTree Parse(IScanner scanner)
        {
            return new SyntaxTree(CreateProgramNode());
        }

        private ProgramNode CreateProgramNode()
        {
            List<FunctionNode> functionList = new();

            FunctionNode f = CreateFunctionNode();
            while (f != null) 
            {
                functionList.Add(f);
                f = CreateFunctionNode();
            }
            
            return new ProgramNode(functionList);
            
        }
        private FunctionNode CreateFunctionNode() 
        {
            return new FunctionNode();
        }

    }
}
