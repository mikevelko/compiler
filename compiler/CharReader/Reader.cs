using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.CharReader
{
    public abstract class Reader
    {
        //atrybuty klasy
        public char currentChar;
        public int position;
        public int currentLineNumber;
        public int currentPositionInLine;
        public abstract bool MoveToNextChar();
        public abstract void Start();

        public abstract string GetStringFromPosition((int,int) position, int length);

        // metoda daj kawalek tekstu od tej pozycji 
    }
}
