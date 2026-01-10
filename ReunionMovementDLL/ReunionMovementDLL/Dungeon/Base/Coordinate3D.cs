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
        /// <summary>
        /// X 坐标。
        /// </summary>
        public int x { get; set; }
        /// <summary>
        /// Y 坐标。
        /// </summary>
        public int y { get; set; }
        /// <summary>
        /// Z 坐标。
        /// </summary>
        public int z { get; set; }

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="other">要比较的另一个 Coordinate3D。</param>
        /// <returns>当 x, y, z 全部相等时返回 true。</returns>
        public bool Equals(Coordinate3D? other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return x == other.x && y == other.y && z == other.z;
        }

        /// <summary>
        /// 判断是否相等（覆盖对象）。
        /// </summary>
        /// <param name="obj">要比较的对象。</param>
        /// <returns>如果 obj 是 Coordinate3D 且等于当前实例则返回 true。</returns>
        public override bool Equals(object? obj) => Equals(obj as Coordinate3D);

        /// <summary>
        /// 获取哈希值。
        /// </summary>
        /// <returns>基于 x, y, z 的哈希值。</returns>
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
        /// 比较大小（用于排序）。
        /// </summary>
        /// <param name="other">用于比较的另一个 Coordinate3D。</param>
        /// <returns>比较结果，先比较 x，再比较 y，最后比较 z。</returns>
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
        /// 比较大小（实现非泛型 IComparable）。
        /// </summary>
        /// <param name="obj">要比较的对象，必须为 Coordinate3D。</param>
        /// <returns>比较结果。</returns>
        /// <exception cref="ArgumentException">当 obj 不是 Coordinate3D 类型时抛出。</exception>
        int IComparable.CompareTo(object obj)
        {
            if (ReferenceEquals(obj, null)) return 1;
            if (!(obj is Coordinate3D other)) throw new ArgumentException("对象必须为 Coordinate3D 类型。", nameof(obj));
            return CompareTo(other);
        }

        /// <summary>
        /// 等于运算符重载。
        /// </summary>
        /// <param name="left">左侧操作数。</param>
        /// <param name="right">右侧操作数。</param>
        /// <returns>当两个实例相等时返回 true。</returns>
        public static bool operator ==(Coordinate3D left, Coordinate3D right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;
            return left.Equals(right);
        }

        /// <summary>
        /// 不等于运算符重载。
        /// </summary>
        /// <param name="left">左侧操作数。</param>
        /// <param name="right">右侧操作数。</param>
        /// <returns>当两个实例不相等时返回 true。</returns>
        public static bool operator !=(Coordinate3D left, Coordinate3D right) => !(left == right);
    }
}
