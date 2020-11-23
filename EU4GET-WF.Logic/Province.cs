using System;
using System.Drawing;

namespace EU4GET_WF.Logic
{
    internal class Province : IEquatable<Province>
    {
        public Province(Color color, int id, string name)
        {
            this._mColor = color;
            this._mId = id;
            this._mName = name;
        }

        private readonly Color _mColor;
        private readonly int _mId;
        private readonly string _mName;

        public Color mColor
        {
            get { return this._mColor; }
        }

        public int mID
        {
            get { return this._mId; }
        }

        public String mName
        {
            get { return this._mName; }
        }

        public override int GetHashCode()
        {
            return this._mId;
        }

        public bool Equals(Province other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this._mId == other._mId;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == this.GetType() && this.Equals((Province) obj);
        }

        public static bool operator ==(Province left, Province right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Province left, Province right)
        {
            return !Equals(left, right);
        }
    }
}
