using compiler.CharReader;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace compilerTests
{
    [TestClass]
    public class CharReaderTests
    {
        [TestMethod]
        public void CreateCharReader_TextShouldBeEqual()
        {
            CharReader charReader = new CharReader("main()");

            Assert.AreEqual(charReader.text, "main()");
        }
        [TestMethod]
        public void CharReaderStart_ShouldDefinePosition() 
        {
            CharReader charReader = new CharReader("text");
            charReader.Start();
            Assert.AreEqual(charReader.currentLine, 1);
            Assert.AreEqual(charReader.currentPositionInLine, 0);
            Assert.AreEqual(charReader.position, -1);
        }
        [TestMethod]
        public void CharReaderMoveToNextChar_ShouldMoveAcrossText() 
        {
            CharReader charReader = new CharReader("a=1");
            charReader.Start();
            charReader.MoveToNextChar();
            Assert.AreEqual(charReader.currentChar, 'a');
            Assert.AreEqual(charReader.position, 0);
            Assert.AreEqual(charReader.currentLine, 1);
            Assert.AreEqual(charReader.currentPositionInLine, 1);
            charReader.MoveToNextChar();
            Assert.AreEqual(charReader.currentChar, '=');
            Assert.AreEqual(charReader.position, 1);
            Assert.AreEqual(charReader.currentLine, 1);
            Assert.AreEqual(charReader.currentPositionInLine, 2);
        }
        [TestMethod]
        public void CharReaderMoveToNextCharNewLine_ShouldChangeCurrentLine()
        {
            CharReader charReader = new CharReader("\na");
            charReader.Start();
            charReader.MoveToNextChar();
            charReader.MoveToNextChar();
            Assert.AreEqual(charReader.currentChar, 'a');
            Assert.AreEqual(charReader.position, 1);
            Assert.AreEqual(charReader.currentLine, 2);
            Assert.AreEqual(charReader.currentPositionInLine, 1);
        }
    }
}
