using System;
using System.Linq;
using EU4_Game_Editing_Tool_WinForm.FileParsing.Internal.Deserializers.Common;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing.Internal.Deserializers
{
    internal class LocalisationDeserializer : Deserializer
    {
        #region Constructors

        public LocalisationDeserializer(StreamReaderFactory streamReaderFactory) : base(streamReaderFactory) { }

        #endregion Constructors

        #region Methods

        public override TextNode Deserialize(string filename)
        {
            TextNode mainNode = new TextNode();
            string[] line;
            int lineNumber;
            string[] tokenArray;
            this.mStream.ReadLine(out line, out lineNumber);
            tokenArray = new string[line.Length - 1];
            Array.Copy(line, 1, tokenArray, 0, tokenArray.Length);
            while (this.mStream.ReadLine(out line, out lineNumber))
            {
                if (line.Any(item => string.IsNullOrEmpty(item)))
                {
                    DeserializeMessageEventArgs messageEventArgs = new DeserializeMessageEventArgs();
                    messageEventArgs.LineNumber = lineNumber;
                    messageEventArgs.Code = DesserializeMessageCode.MissingElementCsvEntry;
                    messageEventArgs.Type = DesserializeMessageType.Warning;
                    this.OnNewDeserializeMessage(messageEventArgs);
                }
                mainNode.mChildNodes.Add(this.CreateLocalisationNode(line, tokenArray));
            }
            return mainNode;
        }

        private TextNode CreateLocalisationNode(string[] line, string[] tokens)
        {
            TextNode node = new TextNode();
            node.mValue = line[0];
            int i = 0;
            for (i = 0; i < tokens.Length; i++)
            {
                TextElement element = new TextElement();
                element.mLeftValue = tokens[i];
                element.mRightValue = line[i + 1];
                node.mChildElements.Add(element);
            }
            return node;
        }

        #endregion Methods
    }
}
