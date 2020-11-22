using System;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing
{
    internal interface IDeserializer
    {
        TextNode Deserialize(string fileName);

        event DeserializeMessageEventHandler NewDeserializeMessage;
    }

    public delegate void DeserializeMessageEventHandler(object sender, DeserializeMessageEventArgs args);

    public class DeserializeMessageEventArgs : EventArgs
    {
        public int LineNumber { get; set; }
        public DesserializeMessageType Type { get; set; }
        public DesserializeMessageCode Code { get; set; }
    }

    public enum DesserializeMessageType 
    {
        Warning,
        Error
    }

    public enum DesserializeMessageCode 
    {
        MissingClosingBracket,
        MissingLefthandValueOfElement,
        MissingClosingCommaOnLineEnd,
        MissingClosingCommaBeforeSpecialToken,
        MissingSpaceBetweenEqualAndWord,
        MissingElementCsvEntry,
        IllegalNodeOpeningBracket,
        IllegalNodeClosingBracket,
        GeneralError
    }
}
