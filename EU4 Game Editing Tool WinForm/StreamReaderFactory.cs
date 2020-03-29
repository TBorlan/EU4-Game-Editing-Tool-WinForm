using EU4_Game_Editing_Tool_WinForm.FileParsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EU4_Game_Editing_Tool_WinForm.Factory.FileParsing
{
    internal class StreamReaderFactory
    {
        private String mFileName;

        public StreamReaderFactory(string fileName)
        {
            this.mFileName = fileName;
        }

        public IStream GetStream()
        {
            if (Path.GetFileName(this.mFileName) == "definition.csv")
            {
                return new StreamReader(this.mFileName, new char[1] { ';' }, 4);
            }
            else if (Path.GetFileName(this.mFileName) == "adjancencies.csv")
            {
                return new StreamReader(this.mFileName, new char[1] { ';' }, 8);
            }
            else
            {
                return new StreamReader(this.mFileName, new char[2] { ' ', '\t' }, -1);
            }
        }
    }

    internal class StreamReader : IStream
    {
        private System.IO.StreamReader mStreamReader;
        private int mLineNumber;
        private char[] mSeparators;
        private int mNumberOfLineTokens;

        internal StreamReader(string file, char[] separators, int nrTokens)
        {
            this.mStreamReader = File.OpenText(file);
            this.mSeparators = separators;
            this.mLineNumber = 0;
            this.mNumberOfLineTokens = nrTokens;
        }

        public bool ReadLine(out string[] line, out int lineNumber)
        {
            if (!this.mStreamReader.EndOfStream)
            {
                List<String> tokenList = this.mStreamReader.ReadLine().Split(this.mSeparators).ToList();
                if (tokenList.Count > this.mNumberOfLineTokens && this.mNumberOfLineTokens > 0)
                {
                    line = tokenList.Take(this.mNumberOfLineTokens).ToArray();
                }
                else if (tokenList.Count < this.mNumberOfLineTokens && this.mNumberOfLineTokens > 0)
                {
                    int elementsToAdd = this.mNumberOfLineTokens - tokenList.Count;
                    for (int i = 0; i < elementsToAdd; i++)
                    {
                        tokenList.Add(new string(string.Empty.ToCharArray()));
                    }
                    line = tokenList.ToArray();
                }
                else
                {
                    line = tokenList.ToArray();
                }
                lineNumber = ++this.mLineNumber;
                return true;
            }
            else
            {
                this.mStreamReader.Close();
                line = null;
                lineNumber = this.mLineNumber;
                return false;
            }
        }
    }
}
