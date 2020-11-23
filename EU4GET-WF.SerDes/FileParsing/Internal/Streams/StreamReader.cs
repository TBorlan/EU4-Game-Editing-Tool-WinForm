using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using EU4GET_WF.SerDes.FileParsing.Internal.Interfaces;

namespace EU4GET_WF.SerDes.FileParsing.Internal.Streams
{
    internal class StreamReader : IStream
    {
        private readonly System.IO.StreamReader _mStreamReader;
        private int _mLineNumber;
        private readonly char[] _mSeparators;
        private int _mNumberOfLineTokens;

        internal StreamReader(string file, char[] separators, NumberOfSeparators nrTokens)
        {
            this._mStreamReader = File.OpenText(file);
            this._mSeparators = separators;
            this._mLineNumber = 0;
            this._mNumberOfLineTokens = (int)nrTokens;
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public bool ReadLine(out string[] line, out int lineNumber)
        {
            if (!this._mStreamReader.EndOfStream)
            {
                List<String> tokenList;
                do
                {
                    if (this._mNumberOfLineTokens == 0)
                    {
                        tokenList = this._mStreamReader.ReadLine().Split(this._mSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
                        lineNumber = ++this._mLineNumber;
                        this._mNumberOfLineTokens = tokenList.Count;
                        break;
                    }
                    tokenList = this._mStreamReader.ReadLine().Split(this._mSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
                    lineNumber = ++this._mLineNumber;
                } while (tokenList.Count == 0 && !this._mStreamReader.EndOfStream);

                if (tokenList.Count > this._mNumberOfLineTokens && this._mNumberOfLineTokens > 0)
                {
                    line = tokenList.Take(this._mNumberOfLineTokens).ToArray();
                }
                else if (tokenList.Count < this._mNumberOfLineTokens && this._mNumberOfLineTokens > 0)
                {
                    int elementsToAdd = this._mNumberOfLineTokens - tokenList.Count;
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
                this._mStreamReader.Close();
                line = null;
                lineNumber = this._mLineNumber;
                return false;
            }
        }
    }
}
