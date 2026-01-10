using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Base
{
    /// <summary>
    /// 三维坐标
    /// </summary>
    public class Coordinate3D : IEquatable<Coordinate3D>, IComparable<Coordinate3D>, IComparable
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Coordinate3D? other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return x == other.x && y == other.y && z == other.z;
        }

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj) => Equals(obj as Coordinate3D);

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
                hash = hash * 31 + z;
                return hash;
            }
        }

        /// <summary>
        /// 比较大小
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Coordinate3D other)
        {
            if (ReferenceEquals(other, null)) return 1;
            int c = x.CompareTo(other.x);
            if (c != 0) return c;
            c = y.CompareTo(other.y);
            if (c != 0) return c;
            return z.CompareTo(other.z);
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
            if (!(obj is Coordinate3D other)) throw new ArgumentException("对象必须为 Coordinate3D 类型。", nameof(obj));
            return CompareTo(other);
        }

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Coordinate3D left, Coordinate3D right)
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
        public static bool operator !=(Coordinate3D left, Coordinate3D right) => !(left == right);
    }
}
