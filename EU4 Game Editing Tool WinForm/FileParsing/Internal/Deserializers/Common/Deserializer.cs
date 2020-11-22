namespace EU4_Game_Editing_Tool_WinForm.FileParsing.Internal.Deserializers.Common
{
    internal abstract class Deserializer : IDeserializer
    {
        #region Constructors

        public Deserializer(StreamReaderFactory streamReaderFactory)
        {
            this.mStream = streamReaderFactory.GetStream();
        }

        #endregion Constructors

        #region Fields

        protected IStream mStream;

        #endregion Fields

        #region Methods

        public abstract TextNode Deserialize(string filename);

        protected virtual void OnNewDeserializeMessage(DeserializeMessageEventArgs args)
        {
            NewDeserializeMessage?.Invoke(this, args);
        }

        #endregion Methods

        #region Events

        public event DeserializeMessageEventHandler NewDeserializeMessage;

        #endregion Events
    }

    internal enum StateType
    {
        Unknown,
        TextNode,
        HalfTextElement,
        FullTextElement,
        SingleValue,
        CommaValue,
        Comments
    }
}
