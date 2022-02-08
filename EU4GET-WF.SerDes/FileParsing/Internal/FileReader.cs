using System;
using System.IO;
using EU4GET_WF.SerDes.FileParsing.Internal.Interfaces;

namespace EU4GET_WF.SerDes.FileParsing.Internal
{
    /// <summary>
    /// C
    /// </summary>
    public class FileReader : IFileReader
    {

        #region Fields

        private protected IDeserializer _mDeserializer;

        private string _mFilePath;

        #endregion Fields

        #region Properties

        public string mFilePath
        {
            set
            {
                if (File.Exists(value))
                {
                    this._mFilePath = value;
                }
                else
                {
                    string message = "File " + Path.GetFileName(value) + " does not exist";
                    throw new Exception(message);
                }
            }
            get
            {
                return this._mFilePath;
            }
        }

        #endregion Properties

        #region Methods

        public TextNode ReadFile(string filePath)
        {
            this.mFilePath = filePath;
            this._mDeserializer = new DeserializerFactory().GetDeserializer(this.mFilePath);
            return this._mDeserializer.Deserialize(this.mFilePath);
        }
        #endregion Methods

    }
}
