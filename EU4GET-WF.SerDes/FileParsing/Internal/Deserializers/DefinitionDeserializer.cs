using System;
using System.Linq;
using EU4GET_WF.SerDes.FileParsing.Internal.Deserializers.Common;
using EU4GET_WF.SerDes.FileParsing.Internal.Interfaces;

namespace EU4GET_WF.SerDes.FileParsing.Internal.Deserializers
{
    internal class DefinitionDeserializer : Deserializer
    {
        #region Constructors

        public DefinitionDeserializer(StreamReaderFactory streamReaderFactory) : base(streamReaderFactory) { }

        #endregion Constructors

        #region Methods

        public override TextNode Deserialize(string fileName)
        {
            TextNode mainNode = new TextNode();
            String[] line;
            int lineNumber;
            this._mStream.ReadLine(out line, out lineNumber);
            while (this._mStream.ReadLine(out line, out lineNumber))
            {
                if (line.Any(item => String.IsNullOrEmpty(item)))
                {
                    DeserializeMessageEventArgs messageEventArgs = new DeserializeMessageEventArgs();
                    messageEventArgs.mLineNumber = lineNumber;
                    messageEventArgs.mCode = DeserializeMessageCode.MissingElementCsvEntry;
                    messageEventArgs.mType = DeserializeMessageType.Warning;
                    this.OnNewDeserializeMessage(messageEventArgs);
                    continue;
                }
                mainNode._mChildNodes.Add(this.CreateProvinceNode(line));
            }
            return mainNode;
        }

        private TextNode CreateProvinceNode(String[] entry)
        {
            TextNode node = new TextNode();
            node._mValue = entry[0];
            TextElement element = new TextElement();
            element._mLeftValue = "red";
            element._mRightValue = entry[1];
            node._mChildElements.Add(element);
            element = new TextElement();
            element._mLeftValue = "green";
            element._mRightValue = entry[2];
            node._mChildElements.Add(element);
            element = new TextElement();
            element._mLeftValue = "blue";
            element._mRightValue = entry[3];
            node._mChildElements.Add(element);
            return node;
        }

        #endregion Methods
    }
}
