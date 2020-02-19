using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing
{
    class Tokenizer : ITokenizer
    {
        private Queue<String> mTokens;

        readonly char[] specials = { '"', '=', '{', '}' };

        public void FeedLine(String[] line)
        {
            mTokens = new Queue<string>(line.Length);
            foreach (String element in line)
            {
                if (element.Length > 1)
                {
                    if (element.IndexOfAny(specials) > 0)
                    {
                        this.TrySplit(element);
                        continue;
                    }
                }
                mTokens.Enqueue(element);
            }
        }

        public string GetNextToken()
        {
            if (mTokens.Count == 0)
            {
                return null;
            }
            else
            {
                return mTokens.Dequeue();
            }
        }

        private void TrySplit(string element)
        {
            int remaining = 0;
            int index;
            while(remaining < element.Length)
            {
                if((index = element.IndexOfAny(this.specials,remaining)) > 0)
                {
                    this.mTokens.Enqueue(element.Substring(remaining, index - remaining));
                    this.mTokens.Enqueue(element.Substring(index, 1));
                    remaining = index + 1;
                }
                else
                {
                    this.mTokens.Enqueue(element.Substring(remaining));
                }
            }
        }
    }
}
