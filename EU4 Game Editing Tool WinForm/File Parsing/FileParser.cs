using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm.File_Parsing
{
    abstract class FileParser : IDisposable
    {
        protected StreamReader mReader;

        protected char[] mSeparator;

        private enum LineParseState
        {
            SimpleElement,
            DoubleQuoteElement,
        }

        protected StreamReader GetReader(String file)
        {
            FileStream stream = File.OpenRead(file);
            StreamReader reader = new StreamReader(stream);
            return reader;
        }

        protected void CloseReader(StreamReader stream)
        {
            stream.Close();
        }

        protected virtual List<String> ParseLine(String line)
        {
            LineParseState state = LineParseState.SimpleElement;
            StringBuilder currentElement = null;
            List<String> elements = new List<string>(line.Split(this.mSeparator,StringSplitOptions.RemoveEmptyEntries));
            List<String> newElements = new List<string>(elements.Count);
            foreach(String element in elements)
            {
                if(state == LineParseState.SimpleElement)
                {
                    if (!element.StartsWith("\"") && !element.StartsWith("#"))
                    {
                        newElements.Add(element);
                    }
                    else if (element.StartsWith("\"") && element.EndsWith("\""))
                    {
                        newElements.Add(element);
                    }
                    else if(element.StartsWith("\""))
                    {
                        currentElement = new StringBuilder(element);
                        state = LineParseState.DoubleQuoteElement;
                    }
                    else
                    {
                        break;
                    }
                }
                else if(state == LineParseState.DoubleQuoteElement)
                {
                    if (element.EndsWith("\""))
                    {
                        currentElement.Append(" ");
                        currentElement.Append(element);
                        newElements.Add(currentElement.ToString());
                        currentElement.Clear();
                        state = LineParseState.SimpleElement;
                    }
                    else
                    {
                        currentElement.Append(" ");
                        currentElement.Append(element);
                    }
                }
            }
            return newElements;
        }

        protected virtual String[] ReadTextData(StreamReader stream)
        {
            List<String> data = new List<string>();
            while (!stream.EndOfStream)
            {
                String line = stream.ReadLine();
                if(line.Count() > 0)
                {
                    data.AddRange(this.ParseLine(line));
                }
            }
            return data.ToArray();
        }

        protected virtual TextNode ConvertToSerializedNode()
        {
            String[] values = ReadTextData(mReader);
            Stack<TextNode> nodes = new Stack<TextNode>(1);
            TextNode baseNode = new TextNode();
            nodes.Push(baseNode);
            int index = 0;
            while (index < values.Length)
            {
                if (index < values.Length - 1)
                {
                    if (values[index + 1].Equals("="))
                    {
                        if (index < values.Length - 2)
                        {
                            if (values[index + 2].Equals("{"))
                            {
                                TextNode node = new TextNode();
                                node.mValue = values[index];
                                nodes.Peek().mChildNodes.Add(node);
                                nodes.Push(node);
                                index += 3;
                                continue;
                            }
                            else
                            {
                                TextElement element = new TextElement();
                                element.mLeftValue = values[index];
                                element.mRightValue = values[index + 2];
                                nodes.Peek().mChildElements.Add(element);
                                index += 3;
                                continue;
                            }
                        }
                    }
                }
                if (values[index].Equals("}"))
                {
                    nodes.Pop();
                    index++;
                }
                else
                {
                    nodes.Peek().mChildValues.Add(values[index]);
                    index++;
                }
            }
            return nodes.Pop();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.mReader.Close();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            //GC.SuppressFinalize(this);
        }
        #endregion
    }
}
