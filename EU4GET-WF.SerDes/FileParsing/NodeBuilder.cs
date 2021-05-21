using EU4GET_WF.SerDes.FileParsing.Internal.Interfaces;

namespace EU4GET_WF.SerDes.FileParsing
{
    class NodeBuilder : INodeBuilder
    {
        public TextNode mNode { get; private set; }

        private TextNode _mActiveNode;

        public void AddElement(string name, string value)
        {
            this._mActiveNode._mChildElements.Add(new TextElement { _mLeftValue = name, _mRightValue = value });
        }

        public void AddValue(string value)
        {
            this._mActiveNode._mChildValues.Add(value);
        }

        public void CloseNode()
        {
            if (this._mActiveNode != this.mNode)
            {
                this.TraverseTreeNode(this.mNode);
            }
        }

        public void CreateRootNode(string value)
        {
            this.mNode = new TextNode{ _mValue = value };
            this._mActiveNode = this.mNode;
        }

        public void OpenNode(string value)
        {
            TextNode node = new TextNode {_mValue = value};
            this._mActiveNode._mChildNodes.Add(node);
            this._mActiveNode = node;
        }

        private bool TraverseTreeNode(TextNode parent)
        {
            if (parent._mChildNodes.Count == 0)
            {
                return false;
            }
            else
            {
                foreach (TextNode childNode in parent._mChildNodes)
                {
                    if (childNode == this._mActiveNode)
                    {
                        this._mActiveNode = parent;
                        return true;
                    }

                    if (this.TraverseTreeNode(childNode))
                    {
                        return true;
                    }                 
                }
                return false;
            }
        }
    }
}
