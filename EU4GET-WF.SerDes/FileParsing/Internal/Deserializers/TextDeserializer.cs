using System;
using System.Collections.Generic;
using System.IO;
using EU4GET_WF.SerDes.FileParsing.Internal.Deserializers.Common;
using EU4GET_WF.SerDes.FileParsing.Internal.Interfaces;

namespace EU4GET_WF.SerDes.FileParsing.Internal.Deserializers
{
    internal class TextDeserializer : Deserializer
    {
        #region Constructors

        public TextDeserializer(StreamReaderFactory streamReaderFactory) : base(streamReaderFactory)
        {
            this._mNodeBuilder = new NodeBuilder();

            this._mTokenizer = new Tokenizer();

            this._mStateStack = new Stack<State>();
        }

        #endregion Constructors

        #region Fields

        bool _mErrorFlag;
        int _mLineNumber;
        readonly NodeBuilder _mNodeBuilder;

        readonly Stack<State> _mStateStack;
        readonly Tokenizer _mTokenizer;

        #endregion Fields

        #region Methods

        public override TextNode Deserialize(string fileName)
        {
            string[] line;
            this._mNodeBuilder.CreateRootNode(Path.GetFileName(fileName));
            while ((this._mStream.ReadLine(out line, out this._mLineNumber)) == true)
            {
                this._mTokenizer.FeedLine(line);
                string token;
                while(!String.IsNullOrEmpty(token = this._mTokenizer.GetNextToken()))
                {
                    this.ProcessToken(token);
                    if (this._mErrorFlag)
                    {
                        return null;
                    }
                }
                this.ProcessEndOfLine();
            } 
            this.Verify();
            return this._mErrorFlag ? null : this._mNodeBuilder.mNode;
        }

        private void OpenOrCloseMultiStringValue(string token)
        {
            State actualState;
            actualState._mState = StateType.Unknown;
            actualState._mValue = null;
            if (this._mStateStack.Count > 0)
            {
                actualState = this._mStateStack.Peek();
            }
            if (actualState._mState == StateType.CommaValue)
            {
                actualState._mState = StateType.SingleValue;
                this._mStateStack.Pop();
                this._mStateStack.Push(actualState);
                return;
            }
            else if (actualState._mState != StateType.HalfTextElement)
            {
                if (actualState._mState == StateType.SingleValue)
                {
                    this._mNodeBuilder.AddValue(actualState._mValue);
                    this._mStateStack.Pop();
                    actualState._mState = StateType.CommaValue;
                    actualState._mValue = null;
                    this._mStateStack.Push(actualState);
                }
                else if (actualState._mState == StateType.FullTextElement)
                {
                    string[] leftAndRight = actualState._mValue.Split('=');
                    this._mNodeBuilder.AddElement(leftAndRight[0], leftAndRight[1]);
                    this._mStateStack.Pop();
                }
                else
                {
                    actualState._mState = StateType.CommaValue;
                    actualState._mValue = null;
                    this._mStateStack.Push(actualState);
                }
                return;
            }
            else
            {
                actualState._mState = StateType.FullTextElement;
                actualState._mValue += "=";
                this._mStateStack.Pop();
                this._mStateStack.Push(actualState);
                return;
            }
        }

        private void PopNodeFromStack(string token)
        {
            State actualState;
            actualState._mState = StateType.Unknown;
            actualState._mValue = null;
            if (this._mStateStack.Count > 0)
            {
                actualState = this._mStateStack.Peek();
            }
            switch (actualState._mState)
            {
                case StateType.CommaValue:
                {
                    this.OnNewDeserializeMessage(new DeserializeMessageEventArgs
                    {
                        mCode = DeserializeMessageCode.MissingClosingCommaBeforeSpecialToken,
                        mLineNumber = this._mLineNumber,
                        mType = DeserializeMessageType.Error
                    });
                    this._mErrorFlag = true;
                    return;
                }
                case StateType.HalfTextElement:
                {
                    this.OnNewDeserializeMessage(new DeserializeMessageEventArgs
                    {
                        mCode = DeserializeMessageCode.IllegalNodeClosingBracket,
                        mLineNumber = this._mLineNumber,
                        mType = DeserializeMessageType.Error
                    });
                    this._mErrorFlag = true;
                    return;
                }
                case StateType.SingleValue:
                {
                    this._mNodeBuilder.AddValue(actualState._mValue);
                    this._mStateStack.Pop();
                    this._mNodeBuilder.CloseNode();
                    this._mStateStack.Pop();
                    return;
                }
                case StateType.TextNode:
                {
                    this._mNodeBuilder.CloseNode();
                    this._mStateStack.Pop();
                    return;
                }
            }
        }

        private void ProcessEndOfLine()
        {
            if (this._mStateStack.Count > 0)
            {
                State actualState = this._mStateStack.Peek();
                if (actualState._mState == StateType.CommaValue)
                {
                    this.OnNewDeserializeMessage(new DeserializeMessageEventArgs
                    {
                        mCode = DeserializeMessageCode.MissingClosingCommaOnLineEnd,
                        mLineNumber = this._mLineNumber,
                        mType = DeserializeMessageType.Warning
                    });
                    return;
                }
                if (actualState._mState == StateType.Comments)
                {
                    this._mStateStack.Pop();
                }
            }
        }

        private void ProcessToken(string token)
        {
            if (this._mStateStack.Count > 0)
            {
                if (this._mStateStack.Peek()._mState == StateType.Comments)
                {
                    return;
                }
            }    

            switch (token)
            {
                case "#":
                    {
                        this.PushCommentsStateOntoStack(token);
                        return;
                    }
                case "=":
                    {
                        this.PushElementOntoStack();
                        return;
                    }
                case "{":
                    {
                        this.PushNodeOntoStack();
                        return;
                    }
                case "}":
                    {
                        this.PopNodeFromStack(token);
                        return;
                    }
                case "\"":
                    {
                        this.OpenOrCloseMultiStringValue(token);
                        return;
                    }
                default:
                    {
                        this.PushValue(token);
                        return;
                    }
            }
        }

        private void PushCommentsStateOntoStack(string token)
        {
            State state;
            state._mState = StateType.Comments;
            state._mValue = null;
            this._mStateStack.Push(state);
        }
        private void PushElementOntoStack()
        {
            State actualState;
            actualState._mState = StateType.Unknown;
            actualState._mValue = null;
            if (this._mStateStack.Count > 0)
            {
                actualState = this._mStateStack.Peek();
            }
            if (actualState._mState != StateType.SingleValue)
            {
                switch (actualState._mState)
                {
                    case StateType.CommaValue:
                    {
                            this.OnNewDeserializeMessage(new DeserializeMessageEventArgs {
                                mCode = DeserializeMessageCode.MissingClosingCommaBeforeSpecialToken,
                                mLineNumber = this._mLineNumber,
                                mType = DeserializeMessageType.Error
                            });
                            this._mErrorFlag = true;
                            return;
                    }
                    default:
                    {
                            this.OnNewDeserializeMessage(new DeserializeMessageEventArgs
                            {
                                mCode = DeserializeMessageCode.MissingLeftHandValueOfElement,
                                mLineNumber = this._mLineNumber,
                                mType = DeserializeMessageType.Error
                            });
                            this._mErrorFlag = true;
                            return;
                    }
                }
            }
            else
            {
                actualState._mState = StateType.HalfTextElement;
                this._mStateStack.Pop();
                this._mStateStack.Push(actualState);
            }
        }
        private void PushNodeOntoStack()
        {
            State actualState;
            actualState._mState = StateType.Unknown;
            actualState._mValue = null;
            if (this._mStateStack.Count > 0)
            {
                actualState = this._mStateStack.Peek();
            }
            if (actualState._mState != StateType.HalfTextElement)
            {
                this.OnNewDeserializeMessage(new DeserializeMessageEventArgs
                {
                    mCode = DeserializeMessageCode.IllegalNodeOpeningBracket,
                    mLineNumber = this._mLineNumber,
                    mType = DeserializeMessageType.Error
                });
                this._mErrorFlag = true;
                return;
            }
            else
            {
                actualState._mState = StateType.TextNode;
                this._mNodeBuilder.OpenNode(actualState._mValue);
                this._mStateStack.Pop();
                this._mStateStack.Push(actualState);
            }
        }
        private void PushValue(string token)
        {
            State actualState;
            actualState._mState = StateType.Unknown;
            actualState._mValue = null;
            if (this._mStateStack.Count > 0)
            {
                actualState = this._mStateStack.Peek();
            }
            switch (actualState._mState)
            {
                case StateType.FullTextElement:
                case StateType.CommaValue:
                {
                    actualState = this._mStateStack.Pop();
                    actualState._mValue = actualState._mValue + " " + token;
                    this._mStateStack.Push(actualState);
                    break;
                }
                case StateType.SingleValue:
                {
                    actualState = this._mStateStack.Pop();
                    this._mNodeBuilder.AddValue(actualState._mValue);
                    actualState._mState = StateType.SingleValue;
                    actualState._mValue = token;
                    this._mStateStack.Push(actualState);
                    break;
                }
                case StateType.HalfTextElement:
                {
                    actualState = this._mStateStack.Pop();
                    this._mNodeBuilder.AddElement(actualState._mValue, token);
                    break;
                }
                default:
                {
                    actualState._mState = StateType.SingleValue;
                    actualState._mValue = token;
                    this._mStateStack.Push(actualState);
                    break;
                }
            }
        }
        private void Verify()
        {
            if(this._mStateStack.Count != 0)
            {
                this.OnNewDeserializeMessage(new DeserializeMessageEventArgs
                {
                    mCode = DeserializeMessageCode.GeneralError,
                    mLineNumber = this._mLineNumber,
                    mType = DeserializeMessageType.Error
                });
                this._mErrorFlag = true;
            }
        }

        #endregion Methods

        private struct State
        {
            #region Fields

            public StateType _mState;

            public String _mValue;

            #endregion Fields
        }
    }
}
