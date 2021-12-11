using compiler.CharReader;
using compiler.Scanners;
using compiler.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Parsers
{
    public interface IParser
    {
        public SyntaxTree Parse(IScanner scanner);
    }
}
