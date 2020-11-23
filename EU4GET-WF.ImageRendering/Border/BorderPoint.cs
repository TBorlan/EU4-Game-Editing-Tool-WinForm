using System;

namespace EU4GET_WF.ImageRendering.Border
{
    public struct BorderPoint : IEquatable<BorderPoint>, IEquatable<Point>
    {
        public int mX
        {
            get;
            set;
        }
        public int mY
        {
            get;
            set;
        }

        public BorderPoint(int x, int y)
        {
            this.mX = x;
            this.mY = y;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public static readonly BorderPoint Empty = new BorderPoint(0, 0);


        public bool Equals(Point other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(BorderPoint other)
        {
            return (this.mX == other.mX) && (this.mY == other.mY);
        }

        public override bool Equals(object obj)
        {
            if (obj is BorderPoint borderPoint)
            {
                return this.Equals(borderPoint);
            }
            else if (obj is Point point)
            {
                return point.Equals(this);
            }
            return false;
        }

        public static implicit operator Point(BorderPoint v)
        {
            return new Point(v.mX, v.mY);
        }

        public static implicit operator BorderPoint(Point v)
        {
            return new BorderPoint(v.X, v.Y);
        }

        public static explicit operator PointF(BorderPoint v)
        {
            return (PointF)((Point)v);
        }

        public static BorderPoint operator +(BorderPoint a, BorderPoint b)
        {
            return new BorderPoint(a.mX + b.mX, a.mY + b.mY);
        }

        public static BorderPoint operator -(BorderPoint a, BorderPoint b)
        {
            return new BorderPoint(a.mX - b.mX, a.mY - b.mY);
        }

        public static bool operator ==(BorderPoint a, BorderPoint b)
        {
            return (Point)a == (Point)b;
        }

        public static bool operator !=(BorderPoint a, BorderPoint b)
        {
            return (Point)a != (Point)b;
        }

        public static bool operator >(BorderPoint a, BorderPoint b)
        {
            BorderPlane plane = a.GetPlane(b);
            if(plane != BorderPlane.Invalid)
            {
                return (plane == BorderPlane.Horizontal) ? (a.mX > b.mX) : (a.mY > b.mY);
            }
            else
            {
                return false;
            }
        }

        public static bool operator <(BorderPoint a, BorderPoint b)
        {
            BorderPlane plane = a.GetPlane(b);
            if (plane != BorderPlane.Invalid)
            {
                return (plane == BorderPlane.Horizontal) ? (a.mX < b.mX) : (a.mY < b.mY);
            }
            else
            {
                return false;
            }
        }

        public static bool operator >=(BorderPoint a, BorderPoint b)
        {
            return (a == b) || (a > b);
        }

        public static bool operator <=(BorderPoint a, BorderPoint b)
        {
            return (a == b) || (a < b);
        }

        public override int GetHashCode()
        {
            return new Point(this.mX, this.mY).GetHashCode();
        }

        public bool IsColinear(BorderPoint point)
        {
            if ((this.mX == point.mX) || (this.mY == point.mY))
            {
                return true;
            }
            return false;
        }
        
        public BorderPlane GetPlane(BorderPoint point)
        {
            if (this.IsColinear(point))
            {
                if(this.mX == point.mX)
                {
                    return BorderPlane.Vertical;
                }
                else
                {
                    return BorderPlane.Horizontal;
                }
            }
            else
            {
                return BorderPlane.Invalid;
            }
        }

        public BorderPlane GetPlane(BorderPoint point, ref int line)
        {
            if (this.IsColinear(point))
            {
                if (this.mX == point.mX)
                {
                    line = this.mX;
                    return BorderPlane.Horizontal;
                }
                else
                {
                    line = this.mY;
                    return BorderPlane.Vertical;
                }
            }
            else
            {
                return BorderPlane.Invalid;
            }
        }
    }
}
