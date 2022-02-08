using System;
using System.Collections.Generic;

namespace EU4GET_WF.SerDes.FileParsing
{
    /// <summary>
    /// Represents a hierarchical container that can be a parent for other <see cref="TextNode"/> or <see cref="TextElement"/>.
    /// </summary>
    public class TextNode
    {
        /// <summary>
        /// Represents a list of the children <see cref="TextNode"/> of this node.
        /// </summary>
        public List<TextNode> _mChildNodes = new List<TextNode>();

        /// <summary>
        /// Represents a list of the children <see cref="TextElement"/> of this node.
        /// </summary>
        public List<TextElement> _mChildElements = new List<TextElement>();

        /// <summary>
        /// Represents a list of the children <see langword="string"/> values of this node.
        /// </summary>
        public List<String> _mChildValues = new List<string>();

        /// <summary>
        /// The <see langword="string"/> value of this node
        /// </summary>
        public string _mValue;
    }
}
