using compiler.CharReader;
using compiler.Parsers;
using compiler.Scanners;
using compiler.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace compilerTests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void Parser_ShouldParseTokensCorrectly() 
        {
            if (!File.Exists("test.txt"))
            {
                File.Create("test.txt").Dispose();

                using (StreamWriter sr = new StreamWriter("test.txt", false))
                {
                    sr.Write("int main(double argument) {}");
                }
            }
            try
            {
                FileReader fileReader = new FileReader("test.txt");
                Scanner scanner = new Scanner(fileReader);
                Parser parser = new Parser(scanner);
                parser.Parse();
                Console.WriteLine("A");
            }
            catch 
            {
                Assert.Fail("No error is need");
            }
        }
    }
}
