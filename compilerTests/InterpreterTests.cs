using compiler.CharReader;
using compiler.Interpreter;
using compiler.Interpreter.Visitor;
using compiler.Parsers;
using compiler.Scanners;
using compiler.Trees;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compilerTests
{
    [TestClass]
    public class InterpreterTests
    {
        [TestMethod]
        public void VariableDefinitionTest_ShouldExistsInFS()
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
                    sr.Write("int main(){ a = foo()} int foo() {} ");
                }
            }
            try
            {
                FileReader fileReader = new FileReader("test.txt");
                Scanner scanner = new Scanner(fileReader);
                Parser parser = new Parser(scanner);
                parser.Parse();
                //jest ok
                SyntaxTree syntaxTreeResult = parser.syntaxTree;

                Visitor visitor = new Visitor();
                InterpreterClass interpreter = new InterpreterClass(syntaxTreeResult, visitor);
                interpreter.Start();
            }
            catch
            {
                Assert.Fail("Syntax trees are not equal");
            }
        }
    }
}
