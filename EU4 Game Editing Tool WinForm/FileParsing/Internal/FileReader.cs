using System;
using System.IO;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing.Internal
{
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
            get => this._mFilePath;
        }

        #endregion Properties

        #region Methods

        public TextNode ReadFile(string filePath)
        {
            this.mFilePath = filePath;
            string extension = Path.GetExtension(this.mFilePath);
            this._mDeserializer = new DeserializerFactory().GetDeserializer(this.mFilePath);
            return this._mDeserializer.Deserialize(this.mFilePath);
        }
        #endregion Methods

    }
}
