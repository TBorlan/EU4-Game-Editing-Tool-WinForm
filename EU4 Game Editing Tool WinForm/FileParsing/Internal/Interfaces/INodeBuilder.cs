using System;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing
{
    public interface INodeBuilder
    {
        TextNode mNode { get; }

        void CreateRootNode(string value);

        void OpenNode(string value);

        void CloseNode();

        void AddElement(string name, string value);

        void AddValue(string value);
    }
}
