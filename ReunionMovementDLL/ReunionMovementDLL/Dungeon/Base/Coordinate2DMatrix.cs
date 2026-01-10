using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Base
{
    /// <summary>
    /// 二维矩阵区域坐标
    /// </summary>
    public class Coordinate2DMatrix : IEquatable<Coordinate2DMatrix>, IComparable<Coordinate2DMatrix>, IComparable
    {
        public int x { get; set; }
        public int y { get; set; }
        public int w { get; set; }
        public int h { get; set; }

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Coordinate2DMatrix? other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return x == other.x && y == other.y && w == other.w && h == other.h;
        }

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj) => Equals(obj as Coordinate2DMatrix);

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + x;
                hash = hash * 31 + y;
                hash = hash * 31 + w;
                hash = hash * 31 + h;
                return hash;
            }
        }

        /// <summary>
        /// 比较大小
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Coordinate2DMatrix other)
        {
            if (ReferenceEquals(other, null)) return 1;
            int c = x.CompareTo(other.x);
            if (c != 0) return c;
            c = y.CompareTo(other.y);
            if (c != 0) return c;
            c = w.CompareTo(other.w);
            if (c != 0) return c;
            return h.CompareTo(other.h);
        }

        /// <summary>
        /// 比较大小
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        int IComparable.CompareTo(object obj)
        {
            if (ReferenceEquals(obj, null)) return 1;
            if (!(obj is Coordinate2DMatrix other)) throw new ArgumentException("对象必须为 Coordinate2DMatrix 类型。", nameof(obj));
            return CompareTo(other);
        }

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Coordinate2DMatrix left, Coordinate2DMatrix right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;
            return left.Equals(right);
        }

        /// <summary>
        /// 判断是否不相等
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Coordinate2DMatrix left, Coordinate2DMatrix right) => !(left == right);
    }
}
