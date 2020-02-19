namespace EU4_Game_Editing_Tool_WinForm.FileParsing
{
    public interface IFileReader
    {
        TextNode ReadFile(string filePath);
    }
}
