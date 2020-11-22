using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing
{
    class NodeBuilder : INodeBuilder
    {
        public TextNode mNode { get; private set; }

        private TextNode mActiveNode;

        public void AddElement(string name, string value)
        {
            mActiveNode.mChildElements.Add(new TextElement { mLeftValue = name, mRightValue = value });
        }

        public void AddValue(string value)
        {
            mActiveNode.mChildValues.Add(value);
        }

        public void CloseNode()
        {
            if (this.mActiveNode != this.mNode)
            {
                this.TraverseTreeNode(this.mNode);
            }
        }

        public void CreateRootNode(string value)
        {
            mNode = new TextNode{ mValue = value };
            mActiveNode = mNode;
        }

        public void OpenNode(string value)
        {
            TextNode node = new TextNode();
            node.mValue = value;
            mActiveNode.mChildNodes.Add(node);
            mActiveNode = node;
        }

        bool TraverseTreeNode(TextNode parent)
        {
            if (parent.mChildNodes.Count == 0)
            {
                return false;
            }
            else
            {
                foreach (TextNode childNode in parent.mChildNodes)
                {
                    if (childNode == this.mActiveNode)
                    {
                        this.mActiveNode = parent;
                        return true;
                    }

                    if (TraverseTreeNode(childNode))
                    {
                        return true;
                    }                 
                }
                return false;
            }
        }
    }
}
