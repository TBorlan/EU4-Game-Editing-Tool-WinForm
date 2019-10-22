using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm.File_Parsing
{
    class TextNode
    {
        public List<TextNode> mChildNodes = new List<TextNode>();

        public List<TextElement> mChildElements = new List<TextElement>();

        public List<String> mChildValues = new List<string>();

        public String mValue;
    }
}
