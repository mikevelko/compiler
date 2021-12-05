using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using compiler.CharReader;

namespace compilerTests
{
    [TestClass]
    public class FileReaderTests
    {
        [TestMethod]
        public void CreateFileReaderCorectly()
        {
            if (!File.Exists("test.txt"))
            {
                File.Create("test.txt").Dispose();

                using (StreamWriter sr = new StreamWriter("test.txt", false))
                {
                    sr.Write("int abc \n abc = 123");
                }
            }
            try
            {
                FileReader fileReader = new FileReader("test.txt");
                Assert.AreEqual('\0', fileReader.currentChar);
            }
            catch
            {
                Assert.Fail("No error is nead");
            }
        }
    }
}
