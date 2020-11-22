using System;
using System.IO;
using System.Collections.Generic;
using EU4_Game_Editing_Tool_WinForm.FileParsing.Internal.Deserializers.Common;
using EU4_Game_Editing_Tool_WinForm.FileParsing.Internal.Common;

namespace EU4_Game_Editing_Tool_WinForm.FileParsing.Internal.Deserializers
{
    internal class TextDeserializer : Deserializer
    {
        #region Constructors

        public TextDeserializer(StreamReaderFactory streamReaderFactory) : base(streamReaderFactory)
        {
            this.mNodeBuilder = new NodeBuilder();

            this.mTokenizer = new Tokenizer();

            this.mStateStack = new Stack<State>();
        }

        #endregion Constructors

        #region Fields

        bool mErrorFlag;
        int mLineNumber;
        NodeBuilder mNodeBuilder;

        Stack<State> mStateStack;
        Tokenizer mTokenizer;

        #endregion Fields

        #region Methods

        public override TextNode Deserialize(string fileName)
        {
            string[] line;
            bool endOfFile;
            string token;
            this.mNodeBuilder.CreateRootNode(Path.GetFileName(fileName));
            while ((endOfFile = this.mStream.ReadLine(out line, out mLineNumber)) == true)
            {
                this.mTokenizer.FeedLine(line);
                while(!String.IsNullOrEmpty(token = this.mTokenizer.GetNextToken()))
                {
                    this.ProcessToken(token);
                    if (this.mErrorFlag)
                    {
                        return null;
                    }
                }
                this.ProcessEndOfLine();
            } 
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

        private void OpenOrCloseMultiStringValue(string token)
        {
            State acutualState;
            acutualState.state = StateType.Unknown;
            acutualState.value = null;
            if (this.mStateStack.Count > 0)
            {
                acutualState = this.mStateStack.Peek();
            }
            if (acutualState.state == StateType.CommaValue)
            {
                acutualState.state = StateType.SingleValue;
                this.mStateStack.Pop();
                this.mStateStack.Push(acutualState);
                return;
            }
            else if (acutualState.state != StateType.HalfTextElement)
            {
                if (acutualState.state == StateType.SingleValue)
                {
                    this.mNodeBuilder.AddValue(acutualState.value);
                    this.mStateStack.Pop();
                    acutualState.state = StateType.CommaValue;
                    acutualState.value = null;
                    this.mStateStack.Push(acutualState);
                }
                else if (acutualState.state == StateType.FullTextElement)
                {
                    string[] leftAndRight = acutualState.value.Split('=');
                    this.mNodeBuilder.AddElement(leftAndRight[0], leftAndRight[1]);
                    this.mStateStack.Pop();
                }
                else
                {
                    acutualState.state = StateType.CommaValue;
                    acutualState.value = null;
                    this.mStateStack.Push(acutualState);
                }
                return;
            }
            else
            {
                acutualState.state = StateType.FullTextElement;
                acutualState.value += "=";
                this.mStateStack.Pop();
                this.mStateStack.Push(acutualState);
                return;
            }
        }

        private void PopNodeFromStack(string token)
        {
            State acutualState;
            acutualState.state = StateType.Unknown;
            acutualState.value = null;
            if (this.mStateStack.Count > 0)
            {
                acutualState = this.mStateStack.Peek();
            }
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
                    this.mStateStack.Pop();
                    this.mNodeBuilder.CloseNode();
                    this.mStateStack.Pop();
                    return;
                }
                case StateType.TextNode:
                {
                    this.mNodeBuilder.CloseNode();
                    this.mStateStack.Pop();
                    return;
                }
            }
        }

        private void ProcessEndOfLine()
        {
            if (mStateStack.Count > 0)
            {
                State actualState = this.mStateStack.Peek();
                if (actualState.state == StateType.CommaValue)
                {
                    this.OnNewDeserializeMessage(new DeserializeMessageEventArgs
                    {
                        Code = DesserializeMessageCode.MissingClosingCommaOnLineEnd,
                        LineNumber = this.mLineNumber,
                        Type = DesserializeMessageType.Warning
                    });
                    return;
                }
                if (actualState.state == StateType.Comments)
                {
                    this.mStateStack.Pop();
                }
            }
        }

        private void ProcessToken(string token)
        {
            if (this.mStateStack.Count > 0)
            {
                if (this.mStateStack.Peek().state == StateType.Comments)
                {
                    return;
                }
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
            this.mStateStack.Push(state);
        }
        private void PushElementOntoStack()
        {
            State acutualState;
            acutualState.state = StateType.Unknown;
            acutualState.value = null;
            if (this.mStateStack.Count > 0)
            {
                acutualState = this.mStateStack.Peek();
            }
            if (acutualState.state != StateType.SingleValue)
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
                this.mStateStack.Pop();
                this.mStateStack.Push(acutualState);
            }
        }
        private void PushNodeOntoStack()
        {
            State acutualState;
            acutualState.state = StateType.Unknown;
            acutualState.value = null;
            if (this.mStateStack.Count > 0)
            {
                acutualState = this.mStateStack.Peek();
            }
            if (acutualState.state != StateType.HalfTextElement)
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
                this.mStateStack.Pop();
                this.mStateStack.Push(acutualState);
            }
        }
        private void PushValue(string token)
        {
            State acutualState;
            acutualState.state = StateType.Unknown;
            acutualState.value = null;
            if (this.mStateStack.Count > 0)
            {
                acutualState = this.mStateStack.Peek();
            }
            switch (acutualState.state)
            {
                case StateType.FullTextElement:
                case StateType.CommaValue:
                {
                    acutualState = this.mStateStack.Pop();
                    acutualState.value = acutualState.value + " " + token;
                    this.mStateStack.Push(acutualState);
                    break;
                }
                case StateType.SingleValue:
                {
                    acutualState = this.mStateStack.Pop();
                    this.mNodeBuilder.AddValue(acutualState.value);
                    acutualState.state = StateType.SingleValue;
                    acutualState.value = token;
                    this.mStateStack.Push(acutualState);
                    break;
                }
                case StateType.HalfTextElement:
                {
                    acutualState = this.mStateStack.Pop();
                    this.mNodeBuilder.AddElement(acutualState.value, token);
                    break;
                }
                default:
                {
                    acutualState.state = StateType.SingleValue;
                    acutualState.value = token;
                    this.mStateStack.Push(acutualState);
                    break;
                }
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

        #endregion Methods

        private struct State
        {
            #region Fields

            public StateType state;

            public String value;

            #endregion Fields
        }
    }
}
