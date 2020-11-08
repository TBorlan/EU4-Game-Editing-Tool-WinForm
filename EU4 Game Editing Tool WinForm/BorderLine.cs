using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm
{
    public struct BorderLine : IEquatable<BorderLine>
    {
        private BorderPoint _mStart;

        private BorderPoint _mEnd;
        public BorderPoint mStart
        {
            get
            {
                return this._mStart;
            }
            set
            {
                if (this.mEnd != BorderPoint.Empty)
                {
                    if (!value.IsColinear(this.mEnd) || (value >= this.mEnd))
                    {
                        throw new ArgumentException("Changing the value will make the line inclined or change the sign", "mStart");
                    }
                    else
                    {
                        this._mStart = value;
                    }
                }
            }
        }
        public BorderPoint mEnd
        {
            get
            {
                return this._mEnd;
            }
            set
            {
                if (this.mStart != BorderPoint.Empty)
                {
                    if (!value.IsColinear(this.mStart) || (value <= this.mStart))
                    {
                        throw new ArgumentException("Changing the value will make the line inclined or change the sign", "mEnd");
                    }
                    else
                    {
                        this._mEnd = value;
                    }
                }
            }
        }

        public BorderPlane mOrientation
        {
            get
            {
                if (mStart.mX == mEnd.mX)
                {
                    return BorderPlane.Vertical;
                }
                else
                {
                    return BorderPlane.Horizontal;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public static readonly BorderLine EmptyLine = new BorderLine(BorderPoint.Empty, BorderPoint.Empty);

        public BorderLine (BorderPoint start, BorderPoint end)
        {
            _mStart = start;
            _mEnd = end;
        }

        public override bool Equals(object obj)
        {
            if(obj is BorderLine line)
            {
                return this.Equals(line);
            }
            return false;
        }

        public bool Equals(BorderLine other)
        {
            return (this.mStart == other.mStart) && (this.mEnd == other.mEnd);
        }

        public static bool operator ==(BorderLine a, BorderLine b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(BorderLine a, BorderLine b)
        {
            return !(a.Equals(b));
        }

        public override int GetHashCode()
        {
            return (mStart, mEnd).GetHashCode();
        }

        public bool IsColinear(BorderLine line)
        {
            if(this.mOrientation == line.mOrientation)
            {
                return this.mOrientation == BorderPlane.Horizontal ? this.mStart.mY == line.mStart.mY : this.mStart.mX == line.mStart.mX;
            }
            return false;
        }

        public bool IsOverlaping(BorderLine line)
        {
            if (this.IsColinear(line))
            {
                if (this.mStart >= line.mEnd || this.mEnd <= line.mStart)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public BorderLine[] Exclude(BorderLine line)
        {
            BorderLine[] result = null;
            if (this.IsOverlaping(line))
            {
                //if (this.mEnd < line.mEnd)
                //{
                //    if(this.mStart > line.mStart)
                //    {
                //        result = new BorderLine[] { new BorderLine(line.mStart, this.mStart), new BorderLine(this.mEnd, line.mEnd) };
                //    }
                //    else
                //    {
                //        result = new BorderLine[] { new BorderLine(this.mStart, line.mStart), new BorderLine(this.mEnd, line.mEnd) };
                //    }
                //}
                //else if (this.mEnd > line.mEnd)
                //{
                //    if(this.mStart > line.mStart)
                //    {
                //        result = new BorderLine[] { new BorderLine(line.mStart, this.mStart), new BorderLine(line.mEnd, this.mEnd) };
                //    }
                //    else
                //    {
                //        result = new BorderLine[] { new BorderLine(this.mStart, line.mStart), new BorderLine(line.mEnd, this.mEnd) };
                //    }
                //}
                //else if (this.mStart == line.mStart)
                //{
                //    if (this.mEnd > line.mEnd)
                //    {
                //        result = new BorderLine[] { new BorderLine(line.mEnd, this.mEnd) };
                //    }
                //    if (this.mEnd < line.mEnd)
                //    {
                //        result = new BorderLine[] { new BorderLine(this.mEnd, line.mEnd) };
                //    }
                //    else
                //    {
                //        result = Array.Empty<BorderLine>();
                //    }
                //}
                //else
                //{
                //    if (this.mStart == line.mEnd)
                //    {
                //        result = new BorderLine[] { new BorderLine(line.mStart, this.mEnd) };
                //    }
                //    else
                //    {
                //        result = new BorderLine[] { new BorderLine(this.mStart, line.mEnd) };
                //    }
                //}
                //if (this.mEnd == line.mStart) // 1
                //{
                //    result = new BorderLine[] { new BorderLine(this.mStart, line.mEnd) };
                //}
                //else if (this.mStart == line.mEnd) // 2
                //{
                //    result = new BorderLine[] { new BorderLine(line.mStart, this.mEnd) };
                //}
                if (this.mStart == line.mStart) // 3 and 9 and 11
                {
                    if (this.mEnd > line.mEnd) //3
                    {
                        result = new BorderLine[] { new BorderLine(line.mEnd, this.mEnd) };
                    }
                    else if (this.mEnd < line.mEnd) // 9
                    {
                        result = new BorderLine[] { new BorderLine(this.mEnd, line.mEnd) };
                    }
                    else // 11
                    {
                        result = Array.Empty<BorderLine>();
                    }
                }
                else if (this.mEnd == line.mEnd) // 4 and 10
                {
                    if (this.mStart > line.mStart) // 10
                    {
                        result = new BorderLine[] { new BorderLine(line.mStart, this.mStart) };
                    }
                    else // 4
                    {
                        result = new BorderLine[] { new BorderLine(this.mStart, line.mStart) };
                    }
                }
                else if (this.mStart > line.mStart) // 6 and 8
                {
                    if (this.mEnd > line.mEnd) // 6
                    {
                        result = new BorderLine[] { new BorderLine(line.mStart, this.mStart), new BorderLine(line.mEnd, this.mEnd) };
                    }
                    else // 8
                    {
                        result = new BorderLine[] { new BorderLine(line.mStart, this.mStart), new BorderLine(this.mEnd, line.mEnd) };
                    }
                }
                else // 5 and 7 
                {
                    if (this.mEnd > line.mEnd) // 7
                    {
                        result = new BorderLine[] { new BorderLine(this.mStart, line.mStart), new BorderLine(line.mEnd, this.mEnd) };
                    }
                    else // 5
                    {
                        result = new BorderLine[] { new BorderLine(this.mStart, line.mStart), new BorderLine(this.mEnd, line.mEnd) };
                    }
                }
            }
            return result;
        }

        public BorderLine Intersect(BorderLine line)
        {
            BorderLine result = BorderLine.EmptyLine;
            if (this.IsOverlaping(line))
            {
                BorderPoint start = this.mStart > line.mStart ? this.mStart : line.mStart;
                BorderPoint end = this.mEnd > line.mEnd ? line.mEnd : this.mEnd;
                result = new BorderLine(start, end);
            }
            return result;
        }

        public BorderLine[] GetGroupExclusiveIntersection(BorderLine[] group)
        {
            List<BorderLine> lines = new List<BorderLine>(group);
            List<BorderLine> result = new List<BorderLine>(lines.Count / 2 + 1);
            int originalCount = lines.Count;
            while (lines.Count >= (originalCount / 2 + originalCount % 2))
            {
                BorderLine intersection = lines[0];
                BorderLine temp;
                int count = 0;
                for (int i = 1; i < lines.Count; i++)
                {
                    if ((temp = intersection.Intersect(lines[i])) != BorderLine.EmptyLine)
                    {
                        if (intersection == lines[0])
                        {
                            intersection = temp;
                            count++;
                        }
                        else
                        {
                            if (temp == intersection)
                            {
                                count++;
                            }
                        }
                    }
                }
                if ((count == (originalCount / 2 + originalCount % 2)) || (count == 0))
                {
                    result.Add(intersection);
                    lines.Remove(lines[0]);
                }
            }
            return result.ToArray();
        }
    }

    public enum BorderPlane
    {
        Vertical = -2,
        Horizontal = -1,
        Invalid = 0
    }
}
