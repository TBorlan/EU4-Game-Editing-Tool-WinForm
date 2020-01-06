using System;
using System.IO;
using System.Reflection;
using Pose;
using EU4_Game_Editing_Tool_WinForm.FileParsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EU4UnitTests
{
    [TestClass]
    public class FileReaderTests
    {
        [TestMethod]
        public void GetTokens_FileDoesNotExist_RaiseException()
        {
            //Arrange
            string fileName = "filedoesnotexist";
            //Act
            IFileReader fileReader = (IFileReader)new FileReader();
            //Assert
            Assert.ThrowsException<Exception>(() => fileReader.GetTokens(fileName));
        }

        [TestMethod]
        public void GetTokens_FileHasUnsupportedExtension_RaiseException()
        {
            //Arrange
            string fileName = "filewithunsupportedextension.extension";
            //Act
            IFileReader fileReader = new FileReader();
            string exceptionMessage = String.Empty;
            try
            {
                PoseContext.Isolate(() =>
                {
                    fileReader.GetTokens(fileName);

                }, fakeFileExists);
            }
            catch(TargetInvocationException exception)
            {
                exceptionMessage = exception.GetBaseException().Message;
            }
            //Assert
            StringAssert.Contains(exceptionMessage, "file extension not supported");
        }

        [TestMethod]
        public void GetTokens_FileIsTextFile_UseTextParsing()
        {
            //Arrange
            String[] expectedList = new string[1] { "txt" };
            string fileName = "TextFile.txt";
            //Act
            IFileReader fileReader = new TestableFileReader();
            String[] returnedList = null;
            PoseContext.Isolate(() =>
            {
                returnedList = fileReader.GetTokens(fileName);
            }, fakeFileExists);
            //Assert
            CollectionAssert.AreEqual(expected: expectedList, actual: returnedList);
        }

        [TestMethod]
        public void GetTokens_FileIsCsvFile_UseCsvParsing()
        {
            //Arrange
            String[] expectedList = new string[1] { "csv" };
            string fileName = "CsvFile.csv";
            //Act
            IFileReader fileReader = new TestableFileReader();
            String[] returnedList = null;
            PoseContext.Isolate(() =>
            {
                returnedList = fileReader.GetTokens(fileName);
            }, fakeFileExists);
            //Assert
            CollectionAssert.AreEqual(expected: expectedList, actual: returnedList);
        }

        Shim fakeFileExists = Shim.Replace(() => File.Exists(Is.A<String>())).With((string s) => { return true; });

        class TestableFileReader : FileReader
        {
            protected override string[] GetCsvTokens()
            {
                return new string[1] { "csv" };
            }
            protected override string[] GetTextTokens()
            {
                return new string[] { "txt" };
            }
        }
    }
}
