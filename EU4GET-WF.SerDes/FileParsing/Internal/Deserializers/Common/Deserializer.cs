using EU4GET_WF.SerDes.FileParsing.Internal.Interfaces;

namespace EU4GET_WF.SerDes.FileParsing.Internal.Deserializers.Common
{
    internal abstract class Deserializer : IDeserializer
    {
        #region Constructors

        public Deserializer(StreamReaderFactory streamReaderFactory)
        {
            this._mStream = streamReaderFactory.GetStream();
        }

        #endregion Constructors

        #region Fields

        protected IStream _mStream;

        #endregion Fields

        #region Methods

        public abstract TextNode Deserialize(string filename);

        protected virtual void OnNewDeserializeMessage(DeserializeMessageEventArgs args)
        {
            this.NewDeserializeMessage?.Invoke(this, args);
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
