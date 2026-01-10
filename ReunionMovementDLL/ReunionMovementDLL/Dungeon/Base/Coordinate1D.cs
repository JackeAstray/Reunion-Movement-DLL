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
        /// <summary>
        /// X 坐标。
        /// </summary>
        public int x { get; set; }

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="other">要比较的另一个 Coordinate1D。</param>
        /// <returns>当两个实例的 x 相等时返回 true。</returns>
        public bool Equals(Coordinate1D? other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return x == other.x;
        }

        /// <summary>
        /// 判断是否相等（覆盖对象）。
        /// </summary>
        /// <param name="obj">要比较的对象。</param>
        /// <returns>如果 obj 是 Coordinate1D 且等于当前实例则返回 true。</returns>
        public override bool Equals(object? obj) => Equals(obj as Coordinate1D);

        /// <summary>
        /// 获取哈希值。
        /// </summary>
        /// <returns>基于 x 的哈希值。</returns>
        public override int GetHashCode() => x.GetHashCode();

        /// <summary>
        /// 比较大小（用于排序）。
        /// </summary>
        /// <param name="other">用于比较的另一个 Coordinate1D。</param>
        /// <returns>比较结果，负：小于；0：等于；正：大于。</returns>
        public int CompareTo(Coordinate1D other)
        {
            if (ReferenceEquals(other, null)) return 1;
            return x.CompareTo(other.x);
        }

        /// <summary>
        /// 比较大小（实现非泛型 IComparable）。
        /// </summary>
        /// <param name="obj">要比较的对象，必须为 Coordinate1D。</param>
        /// <returns>比较结果。</returns>
        /// <exception cref="ArgumentException">当 obj 不是 Coordinate1D 类型时抛出。</exception>
        int IComparable.CompareTo(object obj)
        {
            if (ReferenceEquals(obj, null)) return 1;
            if (!(obj is Coordinate1D other)) throw new ArgumentException("对象必须为 Coordinate1D 类型。", nameof(obj));
            return CompareTo(other);
        }

        /// <summary>
        /// 等于运算符重载。
        /// </summary>
        /// <param name="left">左侧操作数。</param>
        /// <param name="right">右侧操作数。</param>
        /// <returns>当两个实例相等时返回 true。</returns>
        public static bool operator ==(Coordinate1D left, Coordinate1D right)
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
        public static bool operator !=(Coordinate1D left, Coordinate1D right) => !(left == right);
    }
}
