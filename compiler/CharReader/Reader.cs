using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.CharReader
{
    public abstract class Reader
    {
        public char currentChar;
        public int position;
        public int currentLine;
        public int currentPositionInLine;
        public abstract bool MoveToNextChar();
        public abstract void Start();
    }
}
