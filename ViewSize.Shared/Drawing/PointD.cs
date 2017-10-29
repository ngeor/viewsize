// <copyright file="PointD.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize.Drawing
{
    /// <summary>
    /// A point in 2D space.
    /// </summary>
    public struct PointD : IEquatable<PointD>
    {
        public PointD(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; }

        public double Y { get; }

        public override bool Equals(object obj)
        {
            return obj is PointD point && this.Equals(point);
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }

        public override string ToString() => $"({this.X}, {this.Y})";

        public bool Equals(PointD other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public static bool operator ==(PointD left, PointD right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PointD left, PointD right)
        {
            return !(left == right);
        }

        public static PointD operator +(PointD left, PointD right)
        {
            return new PointD(left.X + right.X, left.Y + right.Y);
        }

        public static PointD operator -(PointD left, PointD right)
        {
            return new PointD(left.X - right.X, left.Y - right.Y);
        }
    }
}
