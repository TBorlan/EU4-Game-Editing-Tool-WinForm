using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EU4_Game_Editing_Tool_WinForm.Factory.FileParsing;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing
{
    class TextReader : FileReader
    {
        protected override TextNode ReadText()
        {
            this.mDeserializer = new DeserializerFactory().GetDeserializer(this.mFilePath);
            return this.mDeserializer.Deserialize(this.mFilePath);
        }
    }
}
