using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using EU4_Game_Editing_Tool_WinForm.FileParsing;

namespace EU4_Game_Editing_Tool_WinForm.Factory.FileParsing
{
    public class FileReaderFactory
    {
        private System.IO.StreamReader streamReader;

        public IStream GetStream(string fileName)
        {
            if(Path.GetFileName(fileName) == "definition.csv")
            {
                return new StreamReader(fileName, new char[1] { ';' }, 4);
            }
            else if(Path.GetFileName(fileName) == "adjancencies.csv")
            {
                return new StreamReader(fileName, new char[1] { ';' }, 8);
            }
            else
            {
                return new StreamReader(fileName, new char[2] { ' ', '\t' }, -1);
            }
        }
    }

    class StreamReader : IStream
    {
        System.IO.StreamReader mStreamReader;

        int mLineNumber;

        char[] mSeparators;

        int mNumberOfLineTokens;

        public StreamReader(string file, char[] separators, int nrTokens)
        {
            mStreamReader = File.OpenText(file);
            mSeparators = separators;
            mLineNumber = 0;
            mNumberOfLineTokens = nrTokens;
        }

        public bool ReadLine(out string[] line, out int lineNumber)
        {
            if (!mStreamReader.EndOfStream)
            {
                List<String> tokenList = mStreamReader.ReadLine().Split(mSeparators).ToList();
                if(tokenList.Count > this.mNumberOfLineTokens && this.mNumberOfLineTokens > 0)
                {
                    line = tokenList.Take(mNumberOfLineTokens).ToArray();
                }
                else if(tokenList.Count < this.mNumberOfLineTokens && this.mNumberOfLineTokens > 0)
                {
                    int elementsToAdd = this.mNumberOfLineTokens - tokenList.Count;
                    for(int i = 0; i < elementsToAdd; i++)
                    {
                        tokenList.Add(new string(string.Empty.ToCharArray()));
                    }
                    line = tokenList.ToArray();
                }
                else
                {
                    line = tokenList.ToArray();
                }
                lineNumber = ++mLineNumber;
                return true;
            }
            else
            {
                mStreamReader.Close();
                line = null;
                lineNumber = mLineNumber;
                return false;
            }
        }
    }
}
