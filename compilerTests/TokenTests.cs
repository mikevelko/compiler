using compiler.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace compilerTests
{
    [TestClass]
    public class TokenTests
    {
        [TestMethod]
        public void CreateTokenConstructor_ShouldBeEqual()
        {
            Token token = new Token(TokenType.ELSE, "else", (1, 1));

            Assert.AreEqual(TokenType.ELSE, token.tokenType);
            Assert.AreEqual(token.text, "else");
            Assert.AreEqual(token.position, (1, 1));
        }
    }
}
