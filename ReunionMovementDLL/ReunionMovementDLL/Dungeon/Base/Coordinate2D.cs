using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Base
{
    /// <summary>
    /// 二维坐标
    /// </summary>
    public class Coordinate2D : IEquatable<Coordinate2D>, IComparable<Coordinate2D>, IComparable
    {
        public int x { get; set; }
        public int y { get; set; }

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Coordinate2D? other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return x == other.x && y == other.y;
        }

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj) => Equals(obj as Coordinate2D);

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (x * 397) ^ y;
            }
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Coordinate2D other)
        {
            if (ReferenceEquals(other, null)) return 1;
            int c = x.CompareTo(other.x);
            if (c != 0) return c;
            return y.CompareTo(other.y);
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        int IComparable.CompareTo(object obj)
        {
            if (ReferenceEquals(obj, null)) return 1;
            if (!(obj is Coordinate2D other)) throw new ArgumentException("对象必须为 Coordinate2D 类型。", nameof(obj));
            return CompareTo(other);
        }

        /// <summary>
        /// 等于运算符重载
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Coordinate2D left, Coordinate2D right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;
            return left.Equals(right);
        }

        /// <summary>
        /// 不等于运算符重载
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Coordinate2D left, Coordinate2D right) => !(left == right);
    }
}
