using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Pose;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EU4UnitTests
{
    [TestClass]
    public class StreamReaderTests
    {
        public TextEmitter text;

        public FileReaderFactory streamFactory;

        Shim EndOfStream;

        Shim ReadLine;

        Shim OpenStreamReader;

        [ClassInitialize]
        public void ClassInit()
        {
            streamFactory = new FileReaderFactory();
            OpenStreamReader = Shim.Replace(() => System.IO.File.OpenText(Is.A<string>())).With(delegate (string s) {});
            EndOfStream = Shim.Replace(() => Is.A<System.IO.StreamReader>().EndOfStream).With(delegate (System.IO.StreamReader @this) { return text.EoF(); });
            ReadLine = Shim.Replace(() => Is.A<System.IO.StreamReader>().ReadLine()).With(delegate (System.IO.StreamReader @this) { return text.ReadLine(); });
        }

        [TestInitialize]
        public void Init()
        {
            text = new TextEmitter();
        }

        [TestCleanup]
        public void CleanUp()
        {
            text = null;
        }

        [TestMethod]
        public void ReadLine_InputFileIsDefinitonCsvAndLineHasMoreThan4Elements_EachLineHas4Elements()
        {

            text.Text=new List<string> { "elementA;elementB;elementC;elementD;elementE" };
            List<String[]> lines = new List<string[]>(1);
            int lineNr;
            String[] lineTokens;
            PoseContext.Isolate(() => {
                IStream testStream = streamFactory.GetStream("definition.csv");
                while(testStream.ReadLine(out lineTokens,out lineNr))
                {

                }
            },EndOfStream,ReadLine,OpenStreamReader);
        }

        [TestMethod]
        public void ReadLine_InputFileIsDefinitonCsvAndLineHasLessThan4Elements_EachLineHas4Elements()
        {
        }

        [TestMethod]

        public void ReadLine_InputFileIsDefinitonCsv_EachLineHas4Elements()
        {
        }

        public class TextEmitter
        {
            public List<String> Text;

            public String ReadLine()
            {
                String line = Text[0];
                Text.RemoveAt(0);
                return line;
            }
            public bool EoF()
            {
                return Text.Count == 0;
            }
        }

        public String[] TestHelper(IStream stream,out int lineNumber)
        {
            String[] line;
            stream.ReadLine(out line,out lineNumber);
            return line;
        }
        
    }
}
