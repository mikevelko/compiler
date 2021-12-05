using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.CharReader
{
    public class CharReader : Reader
    {
        public string text;
        public CharReader(string text) 
        {
            if(text == null) 
            {
                throw new ArgumentNullException();
            }
            if(text.Length == 0) 
            {
                throw new ArgumentException("Empty text");
            }
            this.text = text;
            Start();
        }

        public override void Start() 
        {
            position = -1;
            currentLineNumber = 1;
            currentPositionInLine = 0;
            currentChar = (char)0;
        }

        public override bool MoveToNextChar()
        {
            position++;
            if (position >= text.Length) 
            {
                position = text.Length;
                currentChar = (char)0;
                return false;
            }
            currentChar = text[position];
            if(currentChar == '\n' || currentChar == '\r') 
            {
                currentLineNumber++;
                currentPositionInLine = 0;
            }
            else 
            {
                currentPositionInLine++;
            }
            return true;
        }

        public override string GetStringFromPosition((int, int) position, int length)
        {
            int positionInString = -1;
            int line = 1;

            while (line != position.Item1)
            {
                positionInString += 1;

                if (positionInString >= text.Length)
                {
                    throw new ArgumentOutOfRangeException();
                }
                if (text[positionInString] == '\n' || text[positionInString] == '\r')
                {
                    line += 1;
                }
            }
            positionInString += position.Item2;

            return text.Substring(positionInString, length);
        }
    }
}
