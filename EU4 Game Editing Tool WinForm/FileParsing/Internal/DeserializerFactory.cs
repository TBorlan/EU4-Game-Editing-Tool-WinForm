using EU4_Game_Editing_Tool_WinForm.FileParsing.Internal.Deserializers;
using System.IO;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing.Internal
{


    internal class DeserializerFactory
    {
        #region Methods

        public IDeserializer GetDeserializer(string FileName)
        {
            switch (Path.GetFileName(FileName))
            {
                case "definition.csv":
                    {
                        return new DefinitionDeserializer(new StreamReaderFactory(FileName));
                    }
                case "adjancencies.csv":
                    {
                        return new AdjacenciesDeserializer(new StreamReaderFactory(FileName));
                    }
                default:
                    {
                        if (Path.GetExtension(FileName) == ".csv")
                        {
                            return new LocalisationDeserializer(new StreamReaderFactory(FileName));
                        }
                        else
                        {
                            return new TextDeserializer(new StreamReaderFactory(FileName));
                        }
                    }
            }
        }

        #endregion Methods
    }
}
