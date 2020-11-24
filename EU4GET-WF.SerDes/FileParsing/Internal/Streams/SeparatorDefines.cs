namespace EU4GET_WF.SerDes.FileParsing.Internal.Streams
{
    internal enum NumberOfSeparators
    {
        UnknownNrOfSeparators = 0,
        UnlimitedNrOfSeparators = -1
    }

    internal static class Separators
    {
        internal static readonly char[] CsvSeparator = new char[] { ';' };
        internal static readonly char[] TextSeparator = new char[] { ' ', '\t' };
    }

}
