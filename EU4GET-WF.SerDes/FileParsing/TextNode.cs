using System;
using System.Collections.Generic;

namespace EU4GET_WF.SerDes.FileParsing
{
    public class TextNode
    {
        public List<TextNode> _mChildNodes = new List<TextNode>();

        public List<TextElement> _mChildElements = new List<TextElement>();

        public List<String> _mChildValues = new List<string>();

        public string _mValue;
    }
}
