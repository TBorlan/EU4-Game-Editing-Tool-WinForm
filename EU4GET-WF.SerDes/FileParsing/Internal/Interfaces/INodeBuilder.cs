namespace EU4GET_WF.SerDes.FileParsing.Internal.Interfaces
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
