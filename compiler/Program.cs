using compiler.CharReader;
using compiler.Interpreter.Visitor;
using compiler.Parsers;
using compiler.Scanners;
using compiler.Trees;
using System;
using System.IO;
using compiler.Interpreter;

namespace compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists("test.txt"))
            {
                File.Delete("test.txt");
            }
            if (!File.Exists("test.txt"))
            {
                File.Create("test.txt").Dispose();

                using (StreamWriter sr = new StreamWriter("test.txt", false))
                {
                    sr.Write("int main () { int a a=1 while (a < 5) { a = a +1}  return a}");
                }
            }
            FileReader fileReader = new FileReader("test.txt");
            Scanner scanner = new Scanner(fileReader);
            Parser parser = new Parser(scanner);
            try 
            {
                parser.Parse();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            
            SyntaxTree syntaxTreeResult = parser.syntaxTree;

            Visitor visitor = new Visitor();
            InterpreterClass interpreter = new InterpreterClass(syntaxTreeResult, visitor);
            

            try
            {
                interpreter.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

        }
    }
}
