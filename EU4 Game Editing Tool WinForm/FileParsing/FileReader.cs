using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing
{
    public class FileReader : IFileReader
    {
        protected virtual string[] GetTextTokens()
        {
            throw new Exception(this.GetType().Name + " doesn't implement Text parsing");
        }

        protected virtual string[] GetCsvTokens()
        {
            throw new Exception(this.GetType().Name + " doesn't implement CSV parsing");
        }

        private string _mFilePath;

        public string mFilePath
        {
            private set
            {
                if (File.Exists(value))
                {
                    _mFilePath = value;
                }
                else
                {
                    string message = "File " + Path.GetFileName(value) + " does not exist";
                    throw new Exception(message);
                }
            }
            get => _mFilePath;
        }

        public string[] GetTokens(string filePath)
        {
            this.mFilePath = filePath;
            string extension = Path.GetExtension(this.mFilePath);
            if (extension.Equals(".txt"))
            {
                return this.GetTextTokens();
            }
            else if (extension.Equals(".csv"))
            {
                return this.GetCsvTokens();
            }
            else
            {
                string message = extension + " file extension not supported";
                throw new Exception(message);
            }

        }
    }
}
