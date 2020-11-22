using System;
using System.IO;
using EU4_Game_Editing_Tool_WinForm.FileParsing.Internal.Streams;
using static EU4_Game_Editing_Tool_WinForm.FileParsing.Internal.Streams.Separators;
using StreamReader = EU4_Game_Editing_Tool_WinForm.FileParsing.Internal.Streams.StreamReader;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing.Internal
{
    internal class StreamReaderFactory
    {
        private String mFileName;

        public StreamReaderFactory(string fileName)
        {
            this.mFileName = fileName;
        }

        public IStream GetStream()
        {
            if (Path.GetExtension(this.mFileName) == ".csv")
            {
                return new StreamReader(this.mFileName, CSV_SEPARATOR, NumberOfSeparators.UnokownNrOfSeparators);
            }
            else
            {
                return new StreamReader(this.mFileName, TEXT_SEPARATOR, NumberOfSeparators.UnlimitedNrOfSeparators);
            }
        }
    }
}
