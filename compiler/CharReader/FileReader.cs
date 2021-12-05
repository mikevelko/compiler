using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.CharReader
{
    public class FileReader : Reader
    {
        public string fileName;
        string currentLine;
        StreamReader streamReader;
        public FileReader(string FileName)
        {
            if (!File.Exists(FileName))
            {
                throw new ArgumentException("File does not exist");
            }
            fileName = FileName;

            Start();

            if (currentLine == null)
            {
                throw new ArgumentException("File is empty");
            }
        }


        public override string GetStringFromPosition((int, int) position, int length)
        {
            StreamReader streamReader = new StreamReader(fileName);
            for (int i = 1; i < position.Item1; i++)
            {
                streamReader.ReadLine();
            }
            string line = streamReader.ReadLine();
            if (line == null)
            {
                throw new ArgumentOutOfRangeException();
            }

            return line.Substring(position.Item2 - 1, length);
        }

        public override bool MoveToNextChar()
        {
            if (currentLine == null)
            {
                return false;
            }

            if (currentPositionInLine >= currentLine.Length)
            {
                currentPositionInLine = 0;
                currentLineNumber += 1;
                currentLine = streamReader.ReadLine();
                currentChar = '\n';
                return true;
            }

            currentChar = currentLine[currentPositionInLine];
            currentPositionInLine += 1;
            return true;
        }

        public override void Start()
        {
            if (streamReader != null)
            {
                streamReader.Close();
            }
            streamReader = new StreamReader(fileName);

            currentChar = '\0';

            currentPositionInLine = 0;
            currentLineNumber = 1;
            currentLine = streamReader.ReadLine();
        }
    }
}
