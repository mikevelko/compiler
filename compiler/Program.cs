﻿using compiler.CharReader;
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
                    sr.Write("int silnia(int a){ return dupa(a+1) } int dupa(int a) {return a+1} int main(){ return 1/0}");
                }
            }
            FileReader fileReader = new FileReader("test.txt");
            Scanner scanner = new Scanner(fileReader);
            Parser parser = new Parser(scanner);
            parser.Parse();
            SyntaxTree syntaxTreeResult = parser.syntaxTree;

            Visitor visitor = new Visitor();
            InterpreterClass interpreter = new InterpreterClass(syntaxTreeResult, visitor);
            interpreter.Start();

        }
    }
}
