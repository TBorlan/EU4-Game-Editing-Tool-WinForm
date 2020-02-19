using System;
using System.IO;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing
{
    public class FileReader : IFileReader
    {
        protected virtual TextNode ReadText()
        {
            throw new Exception(this.GetType().Name + " doesn't implement Text parsing");
        }

        protected virtual TextNode ReadCsv()
        {
            throw new Exception(this.GetType().Name + " doesn't implement CSV parsing");
        }

        private protected IDeserializer mDeserializer;

        private string _mFilePath;

        public string mFilePath
        {
            private set
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

        public TextNode ReadFile(string filePath)
        {
            this.mFilePath = filePath;
            string extension = Path.GetExtension(this.mFilePath);
            if (extension.Equals(".txt"))
            {
                return this.ReadText();
            }
            else if (extension.Equals(".csv"))
            {
                return this.ReadCsv();
            }
            else
            {
                string message = extension + " file extension not supported";
                throw new Exception(message);
            }

        }
    }
}
