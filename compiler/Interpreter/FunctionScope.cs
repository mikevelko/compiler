using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Interpreter
{
    public class FunctionScope
    {
        public Dictionary<string, int> IntVariables;
        public Dictionary<string, double> DoubleVariables;

        public FunctionScope() 
        {
            IntVariables = new Dictionary<string, int>();
            DoubleVariables = new Dictionary<string, double>();
        }
    }
}
