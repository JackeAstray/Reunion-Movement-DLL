#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Base
{
    /// <summary>
    /// 一维坐标
    /// </summary>
    public class Coordinate1D : IEquatable<Coordinate1D>, IComparable<Coordinate1D>, IComparable
    {
        public int x { get; set; }

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Coordinate1D? other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return x == other.x;
        }

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj) => Equals(obj as Coordinate1D);

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => x.GetHashCode();

        /// <summary>
        /// 比较大小
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Coordinate1D other)
        {
            if (ReferenceEquals(other, null)) return 1;
            return x.CompareTo(other.x);
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
            if (!(obj is Coordinate1D other)) throw new ArgumentException("对象必须为 Coordinate1D 类型。", nameof(obj));
            return CompareTo(other);
        }

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Coordinate1D left, Coordinate1D right)
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
        public static bool operator !=(Coordinate1D left, Coordinate1D right) => !(left == right);
    }
}
