using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm.File_Parsing
{
    class CsvParser : FileParser, IParser
    {
        public CsvParser()
        {
            this.mSeparator = new char[] { ' ', '\t' };
        }

        protected override List<string> ParseLine(string line)
        {
            String[] values = line.Split(mSeparator, 4);
            int index = values.Count();
            if(index == 4)
            {
                for(index = 0; index < 4; index++)
                {
                    if (String.IsNullOrEmpty(values[index]))
                    {
                        return new List<string>();
                    }
                }
                return new List<string>(values);
            }
            else
            {
                return new List<string>();
            }
        }

        protected override TextNode ConvertToSerializedNode()
        {
            String[] values = ReadTextData(mReader);
            TextNode baseNode = new TextNode();
            int index = 0;
            while(index < values.Length)
            {
                TextNode node = new TextNode();
                node.mValue = values[index];
                node.mChildValues.Add(values[index + 1]);
                node.mChildValues.Add(values[index + 2]);
                node.mChildValues.Add(values[index + 3]);
                baseNode.mChildNodes.Add(node);
                index += 4;
            }
            return baseNode;
        }

        public TextNode ParseFile(string filename)
        {
            this.mReader = this.GetReader(filename);
            return this.ConvertToSerializedNode();
        }
    }
}
