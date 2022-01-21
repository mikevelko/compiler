using compiler.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Interpreter
{
    public class FunctionCallContext
    {
        public Stack<FunctionScope> functionScopes;
        public string functionName; 

        public FunctionCallContext(string functionName)
        {
            functionScopes = new Stack<FunctionScope>();
            this.functionName = functionName;
        }
    }
}
