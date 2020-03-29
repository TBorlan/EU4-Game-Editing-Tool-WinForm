using System;
using System.Collections.Generic;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing
{
    public class TextNode
    {
        public List<TextNode> mChildNodes = new List<TextNode>();

        public List<TextElement> mChildElements = new List<TextElement>();

        public List<String> mChildValues = new List<string>();

        public String mValue;
    }
}
