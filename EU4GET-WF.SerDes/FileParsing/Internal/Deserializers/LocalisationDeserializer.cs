using System;
using System.Linq;
using EU4GET_WF.SerDes.FileParsing.Internal.Deserializers.Common;
using EU4GET_WF.SerDes.FileParsing.Internal.Interfaces;

namespace EU4GET_WF.SerDes.FileParsing.Internal.Deserializers
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
            this._mStream.ReadLine(out line, out lineNumber);
            tokenArray = new string[line.Length - 1];
            Array.Copy(line, 1, tokenArray, 0, tokenArray.Length);
            while (this._mStream.ReadLine(out line, out lineNumber))
            {
                if (line.Any(item => string.IsNullOrEmpty(item)))
                {
                    DeserializeMessageEventArgs messageEventArgs = new DeserializeMessageEventArgs();
                    messageEventArgs.mLineNumber = lineNumber;
                    messageEventArgs.mCode = DeserializeMessageCode.MissingElementCsvEntry;
                    messageEventArgs.mType = DeserializeMessageType.Warning;
                    this.OnNewDeserializeMessage(messageEventArgs);
                }
                mainNode._mChildNodes.Add(this.CreateLocalisationNode(line, tokenArray));
            }
            return mainNode;
        }

        private TextNode CreateLocalisationNode(string[] line, string[] tokens)
        {
            TextNode node = new TextNode();
            node._mValue = line[0];
            int i = 0;
            for (i = 0; i < tokens.Length; i++)
            {
                TextElement element = new TextElement();
                element._mLeftValue = tokens[i];
                element._mRightValue = line[i + 1];
                node._mChildElements.Add(element);
            }
            return node;
        }

        #endregion Methods
    }
}
