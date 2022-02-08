using EU4GET_WF.SerDes.FileParsing.Internal.Interfaces;

namespace EU4GET_WF.SerDes.FileParsing
{
    /// <summary>
    /// Provides APIs to create a hierarchical <see cref="TextNode"/> data tree.
    /// </summary>
    //TODO: This class is used to build a data tree, name doesn't reflect that. Also the data structure that should be visible to the app is the data tree, not the TextNode. I think the data tree should be bidirectional.
    class NodeBuilder : INodeBuilder
    {
        /// <summary>
        /// The root <see cref="TextNode"/> of the tree.
        /// </summary>
        public TextNode mNode { get; private set; }

        /// <summary>
        /// The current active <see cref="TextNode"/> of the tree.
        /// </summary>
        private TextNode _mActiveNode;

        /// <summary>
        /// Adds a new <see cref="TextElement"/> to the current <see cref="NodeBuilder._mActiveNode"/>.
        /// </summary>
        /// <param name="name">The <see cref="TextElement._mLeftValue"/> of the new element.</param>
        /// <param name="value">The <see cref="TextElement._mRightValue"/> of the new element.</param>
        public void AddElement(string name, string value)
        {
            this._mActiveNode._mChildElements.Add(new TextElement { _mLeftValue = name, _mRightValue = value });
        }

        /// <summary>
        /// Adds a new value to the current <see cref="NodeBuilder._mActiveNode"/>.
        /// </summary>
        /// <param name="value"></param>
        public void AddValue(string value)
        {
            this._mActiveNode._mChildValues.Add(value);
        }

        /// <summary>
        /// Close the current active <see cref="TextNode"/> and activates its parent.
        /// </summary>
        public void CloseNode()
        {
            if (this._mActiveNode != this.mNode)
            {
                this.TraverseTreeNode(this.mNode);
            }
        }

        /// <summary>
        /// Creates a new <see cref="TextNode"/> data tree,
        /// </summary>
        /// <param name="value">Value of the root <see cref="TextNode"/>.</param>
        public void CreateRootNode(string value)
        {
            this.mNode = new TextNode{ _mValue = value };
            this._mActiveNode = this.mNode;
        }

        /// <summary>
        /// Creates a new <see cref="TextNode"/> as the child of the current active <see cref="TextNode"/> and selects it.
        /// </summary>
        /// <param name="value">Value of the new <see cref="TextNode"/>.</param>
        public void OpenNode(string value)
        {
            TextNode node = new TextNode {_mValue = value};
            this._mActiveNode._mChildNodes.Add(node);
            this._mActiveNode = node;
        }

        /// <summary>
        /// Activate the parent of the current active <see cref="TextNode"/>.
        /// </summary>
        /// <param name="parent">The <see cref="TextNode"/> from where to start looking for the parent.</param>
        /// <returns></returns>
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
