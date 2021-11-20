using compiler.CharReader;
using compiler.Scanners;
using compiler.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace compilerTests
{
    [TestClass]
    public class ScannerTests
    {
        [TestMethod]
        public void ScannerConstructor_ShouldDefineTokenDictionaries()
        {
            Scanner scanner = new Scanner(new CharReader("main()"));
            Assert.IsTrue(scanner.KeywordTokens.ContainsKey("while"));
            Assert.IsTrue(scanner.SignsTokens.ContainsKey("="));
        }
        [TestMethod]
        public void ScannerNextToken_ShouldCreateCorrectTokens()
        {
            Scanner scanner = new Scanner(new CharReader("main()"));
            scanner.NextToken();
            Token mainToken = new Token(TokenType.IDENTIFIER, "main", (1, 1));
            Assert.AreEqual(scanner.token.text, mainToken.text);
            Assert.AreEqual(scanner.token.tokenType, mainToken.tokenType);
            Assert.AreEqual(scanner.token.position, mainToken.position);
            scanner.NextToken();
            Token left_bracket = new Token(TokenType.LEFT_ROUND_BRACKET, "(", (1, 5));
            Assert.AreEqual(scanner.token.text, left_bracket.text);
            Assert.AreEqual(scanner.token.tokenType, left_bracket.tokenType);
            Assert.AreEqual(scanner.token.position, left_bracket.position);
        }
        [TestMethod]
        public void ScannerNextToken_ShouldCreateCorrectTokens2() 
        {
            List<TokenType> tokenTypes = new List<TokenType>(); 
            Scanner scanner = new Scanner(new CharReader(
                "main()\n { \n int a = 5 \n a = a + 1.0 \n }"
                ));
            while (scanner.NextToken()) 
            {
                tokenTypes.Add(scanner.token.tokenType);
            }
            List<TokenType> correctTokens = new List<TokenType>() { TokenType.IDENTIFIER, TokenType.LEFT_ROUND_BRACKET, TokenType.RIGHT_ROUND_BRACKET,TokenType.LEFT_CURLY_BRACKET, TokenType.INT, TokenType.IDENTIFIER, TokenType.ASSIGN,
            TokenType.NUMBER_INT, TokenType.IDENTIFIER, TokenType.ASSIGN, TokenType.IDENTIFIER, TokenType.PLUS, TokenType.NUMBER_DOUBLE, TokenType.RIGHT_CURLY_BRACKET};
            for(int i = 0; i < correctTokens.Count; i++) 
            {
                Assert.AreEqual(correctTokens[i], tokenTypes[i]);
            }
        }
    }
}
