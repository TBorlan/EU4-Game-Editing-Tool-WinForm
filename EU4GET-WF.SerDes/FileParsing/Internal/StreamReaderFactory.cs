using System;
using System.IO;
using EU4GET_WF.SerDes.FileParsing.Internal.Interfaces;
using EU4GET_WF.SerDes.FileParsing.Internal.Streams;
using static EU4GET_WF.SerDes.FileParsing.Internal.Streams.Separators;
using StreamReader = EU4GET_WF.SerDes.FileParsing.Internal.Streams.StreamReader;

namespace EU4GET_WF.SerDes.FileParsing.Internal
{
    internal class StreamReaderFactory
    {
        private readonly String _mFileName;

        public StreamReaderFactory(string fileName)
        {
            this._mFileName = fileName;
        }

        public IStream GetStream()
        {
            if (Path.GetExtension(this._mFileName) == ".csv")
            {
                return new StreamReader(this._mFileName, CsvSeparator, NumberOfSeparators.UnknownNrOfSeparators);
            }
            else
            {
                return new StreamReader(this._mFileName, TextSeparator, NumberOfSeparators.UnlimitedNrOfSeparators);
            }
        }
    }
}
