using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing
{
    class TextParser : FileParser, IParser
    {
        public TextParser()
        {
            this.mSeparator = new char[] { ' ','\t' };
        }

        public TextNode ParseFile(string filename)
        {
            this.mReader = this.GetReader(filename);
            return this.ConvertToSerializedNode();
        }
    }
}
