using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing
{
    class FileReader : IFileReader
    {
        protected virtual string[] GetTextTokens()
        {
            return null;
        }

        protected virtual string[] GetCsvTokens()
        {
            return null;
        }

        private string _mFilePath;

        public string mFilePath
        {
            set
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
        }

        public string[] GetTokens(string filePath)
        {
            
        }
    }
}
