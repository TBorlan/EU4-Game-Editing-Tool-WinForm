using System.IO;
using EU4GET_WF.SerDes.FileParsing.Internal.Deserializers;
using EU4GET_WF.SerDes.FileParsing.Internal.Interfaces;

namespace EU4GET_WF.SerDes.FileParsing.Internal
{


    internal class DeserializerFactory
    {
        #region Methods

        public IDeserializer GetDeserializer(string fileName)
        {
            switch (Path.GetFileName(fileName))
            {
                case "definition.csv":
                    {
                        return new DefinitionDeserializer(new StreamReaderFactory(fileName));
                    }
                case "adjancencies.csv":
                    {
                        return new AdjacenciesDeserializer(new StreamReaderFactory(fileName));
                    }
                default:
                    {
                        if (Path.GetExtension(fileName) == ".csv")
                        {
                            return new LocalisationDeserializer(new StreamReaderFactory(fileName));
                        }
                        else
                        {
                            return new TextDeserializer(new StreamReaderFactory(fileName));
                        }
                    }
            }
        }

        #endregion Methods
    }
}
