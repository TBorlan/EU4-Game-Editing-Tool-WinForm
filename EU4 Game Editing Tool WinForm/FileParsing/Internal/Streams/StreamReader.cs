using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing.Internal.Streams
{
    internal class StreamReader : IStream
    {
        private System.IO.StreamReader mStreamReader;
        private int mLineNumber;
        private char[] mSeparators;
        private int mNumberOfLineTokens;

        internal StreamReader(string file, char[] separators, NumberOfSeparators nrTokens)
        {
            this.mStreamReader = File.OpenText(file);
            this.mSeparators = separators;
            this.mLineNumber = 0;
            this.mNumberOfLineTokens = (int)nrTokens;
        }

        public bool ReadLine(out string[] line, out int lineNumber)
        {
            if (!this.mStreamReader.EndOfStream)
            {
                List<String> tokenList;
                do
                {
                    if (this.mNumberOfLineTokens == 0)
                    {
                        tokenList = this.mStreamReader.ReadLine().Split(this.mSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
                        lineNumber = ++this.mLineNumber;
                        this.mNumberOfLineTokens = tokenList.Count;
                        break;
                    }
                    tokenList = this.mStreamReader.ReadLine().Split(this.mSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
                    lineNumber = ++this.mLineNumber;
                } while (tokenList.Count == 0 && !this.mStreamReader.EndOfStream);

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
