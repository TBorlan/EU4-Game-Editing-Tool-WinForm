using System;
using EU4GET_WF.SerDes.FileParsing.Internal.Deserializers.Common;

namespace EU4GET_WF.SerDes.FileParsing.Internal.Deserializers
{
    internal class AdjacenciesDeserializer : Deserializer
    {
        #region Constructors

        public AdjacenciesDeserializer(StreamReaderFactory streamReaderFactory) : base(streamReaderFactory) { }

        #endregion Constructors

        #region Methods

        public override TextNode Deserialize(string fileName)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}
