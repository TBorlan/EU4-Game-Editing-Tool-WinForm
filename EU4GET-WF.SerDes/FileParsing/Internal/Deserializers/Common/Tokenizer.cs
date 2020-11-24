using System;
using System.Collections.Generic;
using EU4GET_WF.SerDes.FileParsing.Internal.Interfaces;

namespace EU4GET_WF.SerDes.FileParsing.Internal.Deserializers.Common
{
    class Tokenizer : ITokenizer
    {
        private Queue<String> _mTokens;

        readonly char[] _mSpecials = { '"', '=', '{', '}', '#' };

        public void FeedLine(String[] line)
        {
            this._mTokens = new Queue<string>(line.Length);
            foreach (String element in line)
            {
                if (element.Length > 1)
                {
                    if (element.IndexOfAny(this._mSpecials) >= 0)
                    {
                        this.TrySplit(element);
                        continue;
                    }
                }
                this._mTokens.Enqueue(element);
            }
        }

        public string GetNextToken()
        {
            if (this._mTokens.Count == 0)
            {
                return null;
            }
            else
            {
                return this._mTokens.Dequeue();
            }
        }

        private void TrySplit(string element)
        {
            int remaining = 0;
            int index;
            while(remaining < element.Length)
            {
                if((index = element.IndexOfAny(this._mSpecials,remaining)) > remaining)
                {
                    this._mTokens.Enqueue(element.Substring(remaining, index - remaining));
                    this._mTokens.Enqueue(element.Substring(index, 1));
                    remaining = index + 1;
                }
                else if (index == remaining)
                {
                    this._mTokens.Enqueue(element.Substring(index, 1));
                    remaining = index + 1;
                }
                else
                {
                    this._mTokens.Enqueue(element.Substring(remaining));
                    return;
                }
            }
        }
    }
}
