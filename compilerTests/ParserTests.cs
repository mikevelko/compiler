using compiler.CharReader;
using compiler.Nodes;
using compiler.Nodes.ExpressionNodes;
using compiler.Nodes.InstructionNodes;
using compiler.Nodes.Interfaces;
using compiler.Parsers;
using compiler.Scanners;
using compiler.Tokens;
using compiler.Trees;
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
        public void Parser_ShouldParseTokensCorrectly_MainWithInstructionsBlockWhichHasExpressionAndVarAssignment() 
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
                    sr.Write("int main() { int a a=4}");
                }
            }
            try
            {
                FileReader fileReader = new FileReader("test.txt");
                Scanner scanner = new Scanner(fileReader);
                Parser parser = new Parser(scanner);
                parser.Parse();
                SyntaxTree syntaxTreeResult = parser.syntaxTree;

                List<IInstructionNode> instructionNodes = new List<IInstructionNode>();
                
                VariableDefinitionNode variableDefinitionNode = new VariableDefinitionNode(TokenType.INT, "a");
                instructionNodes.Add(variableDefinitionNode);

                SimpleExpressionNode expressionNode = new SimpleExpressionNode(new Token(TokenType.NUMBER_INT, "4", (1, 22)));
                VarAssignmentOrFuncInvocationNode varAssignmentOrFuncInvocationNode = new VarAssignmentOrFuncInvocationNode(expressionNode, new Token(TokenType.ASSIGN, "=", (1, 21)));
                IdentifierAssignmentOrInvocationNode identifierAssignmentOrInvocationNode = new IdentifierAssignmentOrInvocationNode("a", varAssignmentOrFuncInvocationNode);

                InstructionsListNode instructionsListNode = new InstructionsListNode(instructionNodes);
                InstructionsBlockNode instructionsBlockNode = new InstructionsBlockNode(instructionsListNode);
                ArgumentsListNode argumentsListNode = new ArgumentsListNode(new List<VariableDefinitionNode>());
                FunctionNode functionNode = new FunctionNode(TokenType.INT, "main", argumentsListNode ,instructionsBlockNode);
                List<FunctionNode> functionNodes = new List<FunctionNode>();
                functionNodes.Add(functionNode);
                ProgramNode programNode = new ProgramNode(functionNodes);
                SyntaxTree syntaxTreeShouldBe = new SyntaxTree(programNode);
                Assert.AreEqual(syntaxTreeShouldBe.rootNode.functionNodes[0].returnType, syntaxTreeResult.rootNode.functionNodes[0].returnType);
                Assert.AreEqual(syntaxTreeShouldBe.rootNode.functionNodes[0].identifier, syntaxTreeResult.rootNode.functionNodes[0].identifier);
                Assert.AreEqual(syntaxTreeShouldBe.rootNode.functionNodes[0].argumentsListNodes.variableDefinitionNodes.Count, syntaxTreeResult.rootNode.functionNodes[0].argumentsListNodes.variableDefinitionNodes.Count);
                
            }
            catch 
            {
                Assert.Fail("Syntax trees are not equal");
            }
        }

        [TestMethod]
        public void Parser_ShouldParseTokensCorrectly_MainWithIfElse()
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
                    sr.Write("int main() { int a \n if(1>2) { a=100 } \r else { a=12 } }");
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
            }
            catch
            {
                Assert.Fail("Syntax trees are not equal");
            }
        }
        [TestMethod]
        public void Parser_ShouldParseTokensCorrectly_MainWithReturn()
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
                    sr.Write("int main() { return 0 }");
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
            }
            catch
            {
                Assert.Fail("Syntax trees are not equal");
            }
        }
        [TestMethod]
        public void Parser_ShouldParseTokensCorrectly_MainWithWhile()
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
                    sr.Write("int main() { int a while(2>1) { a = a + 1 } }");
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
            }
            catch
            {
                Assert.Fail("Syntax trees are not equal");
            }
        }
        [TestMethod]
        public void Parser_ShouldParseTokensCorrectly_MainWithLongExpression()
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
                    sr.Write("int main() { int a \n a = -(a * 3 + 4) return 0 }");
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
            }
            catch
            {
                Assert.Fail("Syntax trees are not equal");
            }
        }
    }
}
