using EU4_Game_Editing_Tool_WinForm.FileParsing;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace EU4_Game_Editing_Tool_WinForm.Factory.FileParsing
{
    internal class DeserializerFactory
    {
        public IDeserializer GetDeserializer(string FileName)
        {
            switch (Path.GetFileName(FileName))
            {
                case "definition.csv":
                    {
                        return new DefinitionDeserializer(new StreamReaderFactory(FileName));
                    }
                case "adjancencies.csv":
                    {
                        return new AdjacenciesDeserializer(new StreamReaderFactory(FileName));
                    }
                default:
                    {
                        return new TextDeserializer(new StreamReaderFactory(FileName));
                    }
            }
        }
    }

    internal class DefinitionDeserializer : Deserializer
    {
        public DefinitionDeserializer(StreamReaderFactory streamReaderFactory) : base(streamReaderFactory) { }

        public override TextNode Deserialize(string fileName)
        {
            TextNode mainNode = new TextNode();
            String[] line;
            int lineNumber;
            this.mStream.ReadLine(out line, out lineNumber);
            while (this.mStream.ReadLine(out line, out lineNumber))
            {
                if (line.Any(item => String.IsNullOrEmpty(item)))
                {
                    DeserializeMessageEventArgs messageEventArgs = new DeserializeMessageEventArgs();
                    messageEventArgs.LineNumber = lineNumber;
                    messageEventArgs.Code = DesserializeMessageCode.MissingElementCsvEntry;
                    messageEventArgs.Type = DesserializeMessageType.Warning;
                    this.OnNewDeserializeMessage(messageEventArgs);
                    continue;
                }
                mainNode.mChildNodes.Add(this.CreateProvinceNode(line));
            }
            return mainNode;
        }

        private TextNode CreateProvinceNode(String[] entry)
        {
            TextNode node = new TextNode();
            node.mValue = entry[0];
            TextElement element = new TextElement();
            element.mLeftValue = "red";
            element.mRightValue = entry[1];
            node.mChildElements.Add(element);
            element = new TextElement();
            element.mLeftValue = "green";
            element.mRightValue = entry[2];
            node.mChildElements.Add(element);
            element = new TextElement();
            element.mLeftValue = "blue";
            element.mRightValue = entry[3];
            node.mChildElements.Add(element);
            return node;
        }
    }

    internal class AdjacenciesDeserializer : Deserializer
    {
        public AdjacenciesDeserializer(StreamReaderFactory streamReaderFactory) : base(streamReaderFactory) { }

        public override TextNode Deserialize(string fileName)
        {
            throw new NotImplementedException();
        }
    }

    internal class TextDeserializer : Deserializer
    {
        NodeBuilder mNodeBuilder;

        Tokenizer mTokenizer;

        Queue<State> mStateStack;

        bool mErrorFlag;

        int mLineNumber;

        public TextDeserializer(StreamReaderFactory streamReaderFactory) : base(streamReaderFactory)
        {
            this.mNodeBuilder = new NodeBuilder();

            this.mTokenizer = new Tokenizer();

            this.mStateStack = new Queue<State>();
        }

        public override TextNode Deserialize(string fileName)
        {
            string[] line;
            bool endOfFile;
            string token;
            this.mNodeBuilder.CreateRootNode(Path.GetFileName(fileName));
            do
            {
                endOfFile = this.mStream.ReadLine(out line, out mLineNumber);
                this.mTokenizer.FeedLine(line);
                while((token = this.mTokenizer.GetNextToken()) != null)
                {
                    this.ProcessToken(token);
                    if (this.mErrorFlag)
                    {
                        return null;
                    }
                }
                this.ProcessEndOfLine();
            } while (!endOfFile);
            this.Verify();
            if (this.mErrorFlag)
            {
                return null;
            }
            else
            {
                return this.mNodeBuilder.mNode;
            }
        }

        private void ProcessToken(string token)
        {
            if(this.mStateStack.Peek().state == StateType.Comments)
            {
                return;
            }
            switch (token)
            {
                case "#":
                    {
                        PushCommentsStateOntoStack(token);
                        return;
                    }
                case "=":
                    {
                        PushElementOntoStack();
                        return;
                    }
                case "{":
                    {
                        PushNodeOntoStack();
                        return;
                    }
                case "}":
                    {
                        PopNodeFromStack(token);
                        return;
                    }
                case "\"":
                    {
                        OpenOrCloseMultiStringValue(token);
                        return;
                    }
                default:
                    {
                        PushValue(token);
                        return;
                    }
            }
        }

        private void PushCommentsStateOntoStack(string token)
        {
            State state;
            state.state = StateType.Comments;
            state.value = null;
            this.mStateStack.Enqueue(state);
        }
        private void PushElementOntoStack()
        {
            State acutualState = this.mStateStack.Peek();
            if(acutualState.state != StateType.SingleValue)
            {
                switch (acutualState.state)
                {
                    case StateType.CommaValue:
                    {
                            this.OnNewDeserializeMessage(new DeserializeMessageEventArgs {
                                Code = DesserializeMessageCode.MissingClosingCommaBeforeSpecialToken,
                                LineNumber = this.mLineNumber,
                                Type = DesserializeMessageType.Error
                            });
                            this.mErrorFlag = true;
                            return;
                    }
                    default:
                    {
                            this.OnNewDeserializeMessage(new DeserializeMessageEventArgs
                            {
                                Code = DesserializeMessageCode.MissingLefthandValueOfElement,
                                LineNumber = this.mLineNumber,
                                Type = DesserializeMessageType.Error
                            });
                            this.mErrorFlag = true;
                            return;
                    }
                }
            }
            else
            {
                acutualState.state = StateType.HalfTextElement;
                this.mStateStack.Dequeue();
                this.mStateStack.Enqueue(acutualState);
            }
        }
        private void PushNodeOntoStack()
        {
            State acutualState = this.mStateStack.Peek();
            if(acutualState.state != StateType.HalfTextElement)
            {
                this.OnNewDeserializeMessage(new DeserializeMessageEventArgs
                {
                    Code = DesserializeMessageCode.IllegalNodeOpeningBracket,
                    LineNumber = this.mLineNumber,
                    Type = DesserializeMessageType.Error
                });
                this.mErrorFlag = true;
                return;
            }
            else
            {
                acutualState.state = StateType.TextNode;
                this.mNodeBuilder.OpenNode(acutualState.value);
                this.mStateStack.Dequeue();
                this.mStateStack.Enqueue(acutualState);
            }
        }
        private void PopNodeFromStack(string token)
        {
            State acutualState = this.mStateStack.Peek();
            switch (acutualState.state)
            {
                case StateType.CommaValue:
                    {
                        this.OnNewDeserializeMessage(new DeserializeMessageEventArgs
                        {
                            Code = DesserializeMessageCode.MissingClosingCommaBeforeSpecialToken,
                            LineNumber = this.mLineNumber,
                            Type = DesserializeMessageType.Error
                        });
                        this.mErrorFlag = true;
                        return;
                    }
                case StateType.HalfTextElement:
                    {
                        this.OnNewDeserializeMessage(new DeserializeMessageEventArgs
                        {
                            Code = DesserializeMessageCode.IllegalNodeClosingBracket,
                            LineNumber = this.mLineNumber,
                            Type = DesserializeMessageType.Error
                        });
                        this.mErrorFlag = true;
                        return;
                    }
                case StateType.SingleValue:
                    {
                        this.mNodeBuilder.AddValue(acutualState.value);
                        this.mStateStack.Dequeue();
                        this.mNodeBuilder.CloseNode();
                        this.mStateStack.Dequeue();
                        return;
                    }
                case StateType.TextNode:
                    {
                        this.mNodeBuilder.CloseNode();
                        this.mStateStack.Dequeue();
                        return;
                    }
            }
        }
        private void OpenOrCloseMultiStringValue(string token)
        {
            State acutualState = this.mStateStack.Peek();
            if(acutualState.state == StateType.CommaValue)
            {
                acutualState.state = StateType.SingleValue;
                this.mStateStack.Dequeue();
                this.mStateStack.Enqueue(acutualState);
                return;
            }
            else if(acutualState.state != StateType.HalfTextElement)
            {
                if(acutualState.state == StateType.SingleValue)
                {
                    this.mNodeBuilder.AddValue(acutualState.value);
                    this.mStateStack.Dequeue();
                    acutualState.state = StateType.CommaValue;
                    acutualState.value = null;
                    this.mStateStack.Enqueue(acutualState);
                }
                else if(acutualState.state == StateType.FullTextElement)
                {
                    string[] leftAndRight = acutualState.value.Split('=');
                    this.mNodeBuilder.AddElement(leftAndRight[0], leftAndRight[1]);
                    this.mStateStack.Dequeue();
                }
                else
                {
                    acutualState.state = StateType.CommaValue;
                    acutualState.value = null;
                    this.mStateStack.Enqueue(acutualState);
                }
                return;
            }
            else
            {
                acutualState.state = StateType.FullTextElement;
                acutualState.value += "=";
                this.mStateStack.Dequeue();
                this.mStateStack.Enqueue(acutualState);
                return;
            }
        }
        private void PushValue(string token)
        {
            State acutualState = this.mStateStack.Peek();
            switch (acutualState.state)
            {
                case StateType.FullTextElement:
                case StateType.CommaValue:
                {
                    acutualState.value = acutualState.value + " " + token;
                    break;
                }
                case StateType.SingleValue:
                {
                    acutualState = this.mStateStack.Dequeue();
                    this.mNodeBuilder.AddValue(acutualState.value);
                    acutualState.state = StateType.SingleValue;
                    acutualState.value = token;
                    this.mStateStack.Enqueue(acutualState);
                    break;
                }
                case StateType.HalfTextElement:
                {
                    acutualState = this.mStateStack.Dequeue();
                    this.mNodeBuilder.AddElement(acutualState.value, token);
                    break;
                }
                default:
                {
                    acutualState.state = StateType.SingleValue;
                    acutualState.value = token;
                    this.mStateStack.Enqueue(acutualState);
                    break;
                }
            }
        }
        private void ProcessEndOfLine()
        {
            State actualState = this.mStateStack.Peek();
            if(actualState.state == StateType.CommaValue)
            {
                this.OnNewDeserializeMessage(new DeserializeMessageEventArgs
                {
                    Code = DesserializeMessageCode.MissingClosingCommaOnLineEnd,
                    LineNumber = this.mLineNumber,
                    Type = DesserializeMessageType.Warning
                });
            }
        }
        private void Verify()
        {
            if(this.mStateStack.Count != 0)
            {
                this.OnNewDeserializeMessage(new DeserializeMessageEventArgs
                {
                    Code = DesserializeMessageCode.GeneralError,
                    LineNumber = this.mLineNumber,
                    Type = DesserializeMessageType.Error
                });
                this.mErrorFlag = true;
            }
        }
    }

    internal abstract class Deserializer : IDeserializer
    {
        public Deserializer(StreamReaderFactory streamReaderFactory)
        {
            this.mStream = streamReaderFactory.GetStream();
        }

        protected IStream mStream;

        public event DeserializeMessageEventHandler NewDeserializeMessage;

        protected virtual void OnNewDeserializeMessage(DeserializeMessageEventArgs args)
        {
            NewDeserializeMessage?.Invoke(this, args);
        }

        public abstract TextNode Deserialize(string filename);
    }

    internal struct State
    {
        public StateType state;

        public String value;
    }

    internal enum StateType
    {
        Unknown,
        TextNode,
        HalfTextElement,
        FullTextElement,
        SingleValue,
        CommaValue,
        Comments
    }
}
