namespace EU4_Game_Editing_Tool_WinForm.GameData
{
    interface ILookupable
    {
        string LookUpId(int id);

        int LookUpValue(string value);
    }
}
