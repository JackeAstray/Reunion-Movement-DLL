using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Base
{
    /// <summary>
    /// 二维矩阵区域坐标，表示一个带有位置和尺寸的矩形区域（x,y,w,h）。
    /// </summary>
    public class Coordinate2DMatrix : IEquatable<Coordinate2DMatrix>, IComparable<Coordinate2DMatrix>, IComparable
    {
        /// <summary>
        /// X 坐标（列）。
        /// </summary>
        public int x { get; set; }

        /// <summary>
        /// Y 坐标（行）。
        /// </summary>
        public int y { get; set; }

        /// <summary>
        /// 宽度（沿 X 方向的大小）。
        /// </summary>
        public int w { get; set; }

        /// <summary>
        /// 高度（沿 Y 方向的大小）。
        /// </summary>
        public int h { get; set; }

        /// <summary>
        /// 初始化一个默认的 <see cref="Coordinate2DMatrix"/> 实例，所有分量为 0。
        /// </summary>
        public Coordinate2DMatrix()
        {
        }

        /// <summary>
        /// 使用指定的位置坐标初始化一个 <see cref="Coordinate2DMatrix"/>，宽高为 0。
        /// </summary>
        /// <param name="x">X 坐标。</param>
        /// <param name="y">Y 坐标。</param>
        public Coordinate2DMatrix(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// 使用指定的位置和宽度初始化一个 <see cref="Coordinate2DMatrix"/>，高度为 0。
        /// </summary>
        /// <param name="x">X 坐标。</param>
        /// <param name="y">Y 坐标。</param>
        /// <param name="w">宽度。</param>
        public Coordinate2DMatrix(int x, int y, int w) : this(x, y)
        {
            this.w = w;
        }

        /// <summary>
        /// 使用指定的位置和尺寸初始化一个 <see cref="Coordinate2DMatrix"/>。
        /// </summary>
        /// <param name="x">X 坐标。</param>
        /// <param name="y">Y 坐标。</param>
        /// <param name="w">宽度。</param>
        /// <param name="h">高度。</param>
        public Coordinate2DMatrix(int x, int y, int w, int h) : this(x, y, w)
        {
            this.h = h;
        }

        /// <summary>
        /// 使用指定的点和尺寸元组初始化一个 <see cref="Coordinate2DMatrix"/>。
        /// </summary>
        /// <param name="point">表示位置的二元组 (x, y)。</param>
        /// <param name="size">表示尺寸的二元组 (w, h)。</param>
        public Coordinate2DMatrix((int x, int y) point, (int w, int h) size)
        {
            this.x = point.x;
            this.y = point.y;
            this.w = size.w;
            this.h = size.h;
        }

        /// <summary>
        /// 复制构造函数，创建一个与指定实例相同值的新实例。
        /// </summary>
        /// <param name="other">要复制的实例。</param>
        public Coordinate2DMatrix(Coordinate2DMatrix other)
        {
            this.x = other.x;
            this.y = other.y;
            this.w = other.w;
            this.h = other.h;
        }

        //public Coordinate2DMatrix(int x, int y, int w, int h)
        //{
        //    this.x = x;
        //    this.y = y;
        //    this.w = w;
        //    this.h = h;
        //}

        /// <summary>
        /// 判断当前实例与另一个 <see cref="Coordinate2DMatrix"/> 是否值相等（比较 x,y,w,h）。
        /// </summary>
        /// <param name="other">要比较的实例。</param>
        /// <returns>如果相等则返回 true，否则返回 false。</returns>
        public bool Equals(Coordinate2DMatrix? other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return x == other.x && y == other.y && w == other.w && h == other.h;
        }

        /// <summary>
        /// 重写 <see cref="object.Equals(object?)"/>，用于与任意对象比较相等性。
        /// </summary>
        /// <param name="obj">要比较的对象。</param>
        /// <returns>如果 obj 是 <see cref="Coordinate2DMatrix"/> 并且值相等则返回 true，否则 false。</returns>
        public override bool Equals(object? obj) => Equals(obj as Coordinate2DMatrix);

        /// <summary>
        /// 获取当前实例的哈希码。使用 <see cref="HashCode.Combine"/> 生成基于字段的哈希值。
        /// </summary>
        /// <returns>哈希码。</returns>
        public override int GetHashCode()
        {
            // 使用 HashCode.Combine 在 .NET Standard 2.1 上获得更好的哈希分布和简洁实现。
            return HashCode.Combine(x, y, w, h);
        }

        /// <summary>
        /// 按照 x, y, w, h 的字典顺序比较当前实例与另一个 <see cref="Coordinate2DMatrix"/> 的大小。
        /// </summary>
        /// <param name="other">要比较的实例。</param>
        /// <returns>如果当前实例小于 other 返回负数，大于返回正数，相等返回 0。</returns>
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
        /// 按照 x, y, w, h 的字典顺序比较当前实例与另一个对象的大小。实现自 <see cref="IComparable"/>。
        /// </summary>
        /// <param name="obj">要比较的对象，必须为 <see cref="Coordinate2DMatrix"/>。</param>
        /// <returns>如果当前实例小于 obj 返回负数，大于返回正数，相等返回 0。</returns>
        /// <exception cref="ArgumentException">当 obj 不是 <see cref="Coordinate2DMatrix"/> 类型时抛出。</exception>
        int IComparable.CompareTo(object obj)
        {
            if (ReferenceEquals(obj, null)) return 1;
            if (!(obj is Coordinate2DMatrix other)) throw new ArgumentException("对象必须为 Coordinate2DMatrix 类型。", nameof(obj));
            return CompareTo(other);
        }

        /// <summary>
        /// 重载相等运算符，判断两个实例的值是否相等（允许为 null）。
        /// </summary>
        /// <param name="left">左操作数。</param>
        /// <param name="right">右操作数。</param>
        /// <returns>相等返回 true，否则 false。</returns>
        public static bool operator ==(Coordinate2DMatrix left, Coordinate2DMatrix right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;
            return left.Equals(right);
        }

        /// <summary>
        /// 重载不等运算符，判断两个实例的值是否不相等。
        /// </summary>
        /// <param name="left">左操作数。</param>
        /// <param name="right">右操作数。</param>
        /// <returns>不相等返回 true，否则 false。</returns>
        public static bool operator !=(Coordinate2DMatrix left, Coordinate2DMatrix right) => !(left == right);

        /// <summary>
        /// 返回表示当前实例的字符串，形式为 "(x,y,w,h)"，便于调试。
        /// </summary>
        /// <returns>表示当前实例的字符串。</returns>
        public override string ToString()
        {
            return $"({x},{y},{w},{h})";
        }
    }
}
