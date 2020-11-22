namespace EU4_Game_Editing_Tool_WinForm.FileParsing
{
    internal interface IParser
    {
        TextNode ParseFile(string filename);
    }
}
