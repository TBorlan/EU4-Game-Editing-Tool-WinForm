using System;
using System.Linq;
using EU4_Game_Editing_Tool_WinForm.FileParsing.Internal.Deserializers.Common;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing.Internal.Deserializers
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
            this.mStream.ReadLine(out line, out lineNumber);
            while (this.mStream.ReadLine(out line, out lineNumber))
            {
                if (line.Any(item => String.IsNullOrEmpty(item)))
                {
                    DeserializeMessageEventArgs messageEventArgs = new DeserializeMessageEventArgs();
                    messageEventArgs.LineNumber = lineNumber;
                    messageEventArgs.Code = DesserializeMessageCode.MissingElementCsvEntry;
                    messageEventArgs.Type = DesserializeMessageType.Warning;
                    this.OnNewDeserializeMessage(messageEventArgs);
                    continue;
                }
                mainNode.mChildNodes.Add(this.CreateProvinceNode(line));
            }
            return mainNode;
        }

        private TextNode CreateProvinceNode(String[] entry)
        {
            TextNode node = new TextNode();
            node.mValue = entry[0];
            TextElement element = new TextElement();
            element.mLeftValue = "red";
            element.mRightValue = entry[1];
            node.mChildElements.Add(element);
            element = new TextElement();
            element.mLeftValue = "green";
            element.mRightValue = entry[2];
            node.mChildElements.Add(element);
            element = new TextElement();
            element.mLeftValue = "blue";
            element.mRightValue = entry[3];
            node.mChildElements.Add(element);
            return node;
        }

        #endregion Methods
    }
}
