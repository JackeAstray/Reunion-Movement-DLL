using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Base
{
    /// <summary>
    /// 三维矩阵区域坐标
    /// </summary>
    public class Coordinate3DMatrix : IEquatable<Coordinate3DMatrix>, IComparable<Coordinate3DMatrix>, IComparable
    {
        /// <summary>
        /// 区域起始 X 坐标
        /// </summary>
        public int x { get; set; }

        /// <summary>
        /// 区域起始 Y 坐标
        /// </summary>
        public int y { get; set; }

        /// <summary>
        /// 区域起始 Z 坐标
        /// </summary>
        public int z { get; set; }

        /// <summary>
        /// 区域宽度（X 方向长度）
        /// </summary>
        public int w { get; set; }

        /// <summary>
        /// 区域高度（Y 方向长度）
        /// </summary>
        public int h { get; set; }

        /// <summary>
        /// 区域深度（Z 方向长度）
        /// </summary>
        public int d { get; set; }

        /// <summary>
        /// 默认构造函数，构造一个所有分量为 0 的矩阵区域坐标。
        /// </summary>
        public Coordinate3DMatrix() { }

        /// <summary>
        /// 构造函数，使用指定的分量值初始化矩阵区域坐标。
        /// </summary>
        /// <param name="x">起始 X 坐标</param>
        /// <param name="y">起始 Y 坐标</param>
        /// <param name="z">起始 Z 坐标</param>
        /// <param name="w">宽度（X 方向长度）</param>
        /// <param name="h">高度（Y 方向长度）</param>
        /// <param name="d">深度（Z 方向长度）</param>
        public Coordinate3DMatrix(int x, int y, int z, int w, int h, int d)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
            this.h = h;
            this.d = d;
        }

        /// <summary>
        /// 判断当前实例是否与另一个 <see cref="Coordinate3DMatrix"/> 相等。
        /// 两个实例的所有分量都相等时认为相等。
        /// </summary>
        /// <param name="other">要比较的另一个实例。</param>
        /// <returns>若相等则返回 true，否则返回 false。</returns>
        public bool Equals(Coordinate3DMatrix? other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return x == other.x && y == other.y && z == other.z && w == other.w && h == other.h && d == other.d;
        }

        /// <summary>
        /// 判断当前实例是否与指定对象相等。
        /// </summary>
        /// <param name="obj">要比较的对象。</param>
        /// <returns>若相等则返回 true，否则返回 false。</returns>
        public override bool Equals(object? obj) => Equals(obj as Coordinate3DMatrix);

        /// <summary>
        /// 获取当前实例的哈希值。
        /// 使用所有分量参与计算以减少哈希冲突。
        /// </summary>
        /// <returns>哈希值。</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + x;
                hash = hash * 31 + y;
                hash = hash * 31 + z;
                hash = hash * 31 + w;
                hash = hash * 31 + h;
                hash = hash * 31 + d;
                return hash;
            }
        }

        /// <summary>
        /// 与另一个 <see cref="Coordinate3DMatrix"/> 逐分量比较顺序，按 x, y, z, w, h, d 的顺序比较。
        /// 当当前实例大于 other 时返回正数，等于返回 0，小于返回负数。
        /// </summary>
        /// <param name="other">要比较的目标实例。</param>
        /// <returns>比较结果。</returns>
        public int CompareTo(Coordinate3DMatrix other)
        {
            if (ReferenceEquals(other, null)) return 1;
            int c = x.CompareTo(other.x);
            if (c != 0) return c;
            c = y.CompareTo(other.y);
            if (c != 0) return c;
            c = z.CompareTo(other.z);
            if (c != 0) return c;
            c = w.CompareTo(other.w);
            if (c != 0) return c;
            c = h.CompareTo(other.h);
            if (c != 0) return c;
            return d.CompareTo(other.d);
        }

        /// <summary>
        /// 非泛型比较实现，要求 obj 为 <see cref="Coordinate3DMatrix"/> 类型。
        /// </summary>
        /// <param name="obj">要比较的对象。</param>
        /// <returns>比较结果。</returns>
        /// <exception cref="ArgumentException">当 obj 不是 <see cref="Coordinate3DMatrix"/> 时抛出。</exception>
        int IComparable.CompareTo(object obj)
        {
            if (ReferenceEquals(obj, null)) return 1;
            if (!(obj is Coordinate3DMatrix other)) throw new ArgumentException("对象必须为 Coordinate3DMatrix 类型。", nameof(obj));
            return CompareTo(other);
        }

        /// <summary>
        /// 相等运算符重载，处理 null 情况。
        /// </summary>
        /// <param name="left">左操作数。</param>
        /// <param name="right">右操作数。</param>
        /// <returns>若相等则返回 true，否则返回 false。</returns>
        public static bool operator ==(Coordinate3DMatrix left, Coordinate3DMatrix right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;
            return left.Equals(right);
        }

        /// <summary>
        /// 不相等运算符重载。
        /// </summary>
        /// <param name="left">左操作数。</param>
        /// <param name="right">右操作数。</param>
        /// <returns>若不相等则返回 true，否则返回 false。</returns>
        public static bool operator !=(Coordinate3DMatrix left, Coordinate3DMatrix right) => !(left == right);
    }
}
