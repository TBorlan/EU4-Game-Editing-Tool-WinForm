using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace EU4UnitTests
{
    [TestClass]
    class FileReaderTests
    {
        [TestMethod]
        public void GetTokens_InvalidFileExtension_RaiseException()
        {

        }
        [TestMethod]
        public void GetTokens_FileDoesNotExist_RaiseException()
        {

        }
        [TestMethod]
        public void GetTokens_TextFileEmpty_EmptyListReturn()
        {

        }
        [TestMethod]
        public void GetTokens_TextFileWithText_ReturnsCorrectTokens()
        {

        }
        [TestMethod]
        public void GetTokens_TextFileWithQuotedString_ReturnsSingleString()
        {

        }
        [TestMethod]
        public void GetTokens_TextFileWithComments_ReturnsListWithoutComments()
        {

        }
        [TestMethod]
        public void GetTokens_TextFileMissingSpaceBetweenControlTokens_ReturnsSplitTokens()
        {

        }


    }
}
