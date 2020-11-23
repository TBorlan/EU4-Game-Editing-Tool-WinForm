using System;

namespace EU4GET_WF.SerDes.FileParsing.Internal.Interfaces
{
    internal interface IDeserializer
    {
        TextNode Deserialize(string fileName);

        event DeserializeMessageEventHandler NewDeserializeMessage;
    }

    public delegate void DeserializeMessageEventHandler(object sender, DeserializeMessageEventArgs args);

    public class DeserializeMessageEventArgs : EventArgs
    {
        public int mLineNumber { get; set; }
        public DeserializeMessageType mType { get; set; }
        public DeserializeMessageCode mCode { get; set; }
    }

    public enum DeserializeMessageType 
    {
        Warning,
        Error
    }

    public enum DeserializeMessageCode 
    {
        MissingClosingBracket,
        MissingLeftHandValueOfElement,
        MissingClosingCommaOnLineEnd,
        MissingClosingCommaBeforeSpecialToken,
        MissingSpaceBetweenEqualAndWord,
        MissingElementCsvEntry,
        IllegalNodeOpeningBracket,
        IllegalNodeClosingBracket,
        GeneralError
    }
}
