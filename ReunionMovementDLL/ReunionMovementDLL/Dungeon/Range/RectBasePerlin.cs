using System;
using System.Collections.Generic;
using System.Text;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Range
{
    /// <summary>
    /// Perlin 噪声矩形基类。
    /// 表示一个带有 Perlin 噪声参数（频率、倍频数、最小/最大高度）并继承自矩形范围的可配置对象基类。
    /// TDerived 用于流式接口返回派生类型。
    /// </summary>
    /// <typeparam name="TDerived">派生类型，用于流式调用返回具体类型。</typeparam>
    public class RectBasePerlin<TDerived> : BasicRect<RectBasePerlin<TDerived>> where TDerived : RectBasePerlin<TDerived>
    {
        /// <summary>噪声频率（基频）。</summary>
        public double frequency { get; protected set; }

        /// <summary>倍频数（octaves）。</summary>
        public uint octaves { get; protected set; }

        /// <summary>最小高度（用于噪声输出裁剪/映射）。</summary>
        public int minHeight { get; protected set; }

        /// <summary>最大高度（用于噪声输出裁剪/映射）。</summary>
        public int maxHeight { get; protected set; }

        /* Getter */

        /// <summary>
        /// 获取频率（基频）。
        /// </summary>
        /// <returns>当前频率值。</returns>
        public double GetFrequency()
        {
            return this.frequency;
        }

        /// <summary>
        /// 获取倍频数（octaves）。
        /// </summary>
        /// <returns>当前倍频数（uint）。</returns>
        public uint GetOctaves()
        {
            return this.octaves;
        }

        /// <summary>
        /// 获取最小高度。
        /// </summary>
        /// <returns>当前最小高度（int）。</returns>
        public int GetMinHeight()
        {
            return this.minHeight;
        }

        /// <summary>
        /// 获取最大高度。
        /// </summary>
        /// <returns>当前最大高度（int）。</returns>
        public int GetMaxHeight()
        {
            return this.maxHeight;
        }

        /// <summary>
        /// 获取值（频率的别名，便于与其他带 'Value' 命名的 API 互操作）。
        /// </summary>
        /// <returns>当前频率值（double）。</returns>
        public double GetValue()
        {
            return this.frequency;
        }

        /// <summary>
        /// 通过引用获取 X 坐标（起始点 X）。
        /// </summary>
        /// <param name="value">引用参数，接收 X 值。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived GetPointX(ref uint value)
        {
            base.GetPointX(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用获取 Y 坐标（起始点 Y）。
        /// </summary>
        /// <param name="value">引用参数，接收 Y 值。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived GetPointY(ref uint value)
        {
            base.GetPointY(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用获取高度（height）。
        /// </summary>
        /// <param name="value">引用参数，接收高度值。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived GetHeight(ref uint value)
        {
            base.GetHeight(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用获取宽度（width）。
        /// </summary>
        /// <param name="value">引用参数，接收宽度值。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived GetWidth(ref uint value)
        {
            base.GetWidth(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用获取点（X, Y）。
        /// </summary>
        /// <param name="value">引用参数，接收 X 值。</param>
        /// <param name="value2">引用参数，接收 Y 值。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived GetPoint(ref uint value, ref uint value2)
        {
            base.GetPoint(ref value, ref value2);
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用获取矩形范围（startX, startY, width, height）。
        /// </summary>
        /// <param name="value">引用参数，接收 startX。</param>
        /// <param name="value2">引用参数，接收 startY。</param>
        /// <param name="value3">引用参数，接收 width。</param>
        /// <param name="value4">引用参数，接收 height。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived GetRange(ref uint value, ref uint value2, ref uint value3, ref uint value4)
        {
            base.GetRange(ref value, ref value2, ref value3, ref value4);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取 X（返回值）。
        /// </summary>
        /// <returns>起始 X 值（uint）。</returns>
        public new uint GetPointX()
        {
            return base.GetPointX();
        }

        /// <summary>
        /// 获取 Y（返回值）。
        /// </summary>
        /// <returns>起始 Y 值（uint）。</returns>
        public new uint GetPointY()
        {
            return base.GetPointY();
        }

        /// <summary>
        /// 获取宽度（返回值）。
        /// </summary>
        /// <returns>宽度（uint）。</returns>
        public new uint GetWidth()
        {
            return base.GetWidth();
        }

        /// <summary>
        /// 获取高度（返回值）。
        /// </summary>
        /// <returns>高度（uint）。</returns>
        public new uint GetHeight()
        {
            return base.GetHeight();
        }

        /* Setter */

        /// <summary>
        /// 设置频率（别名，等同于 SetFrequency）。
        /// </summary>
        /// <param name="frequency">要设置的频率值。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public TDerived SetValue(double frequency)
        {
            this.frequency = frequency;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置频率。
        /// </summary>
        /// <param name="frequency">要设置的频率值。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public TDerived SetFrequency(double frequency)
        {
            this.frequency = frequency;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置倍频数（octaves）。
        /// </summary>
        /// <param name="octaves">要设置的倍频数（uint）。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public TDerived SetOctaves(uint octaves)
        {
            this.octaves = octaves;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置最小高度。
        /// </summary>
        /// <param name="minHeight">要设置的最小高度（int）。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public TDerived SetMinHeight(int minHeight)
        {
            this.minHeight = minHeight;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置最大高度。
        /// </summary>
        /// <param name="maxHeight">要设置的最大高度（int）。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public TDerived SetMaxHeight(int maxHeight)
        {
            this.maxHeight = maxHeight;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置起始 X（链式）。
        /// </summary>
        /// <param name="startX">起始 X 值。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived SetPointX(uint startX)
        {
            base.SetPointX(startX);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置起始 Y（链式）。
        /// </summary>
        /// <param name="startY">起始 Y 值。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived SetPointY(uint startY)
        {
            base.SetPointY(startY);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置宽度（链式）。
        /// </summary>
        /// <param name="width">宽度值。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived SetWidth(uint width)
        {
            base.SetWidth(width);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置高度（链式）。
        /// </summary>
        /// <param name="height">高度值。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived SetHeight(uint height)
        {
            base.SetHeight(height);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置单个点（将 point 作为线性索引或单值处理，具体行为由 BasicRect 决定）。
        /// </summary>
        /// <param name="point">点值。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived SetPoint(uint point)
        {
            base.SetPoint(point);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置起始点（X, Y）。
        /// </summary>
        /// <param name="startX">起始 X。</param>
        /// <param name="startY">起始 Y。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived SetPoint(uint startX, uint startY)
        {
            base.SetPoint(startX, startY);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置范围（起始点 + 长度）。
        /// </summary>
        /// <param name="startX">起始 X。</param>
        /// <param name="startY">起始 Y。</param>
        /// <param name="length">线性长度或其它由 BasicRect 定义的长度。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived SetRange(uint startX, uint startY, uint length)
        {
            base.SetRange(startX, startY, length);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置范围（起始点 + 宽度 + 高度）。
        /// </summary>
        /// <param name="startX">起始 X。</param>
        /// <param name="startY">起始 Y。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived SetRange(uint startX, uint startY, uint width, uint height)
        {
            base.SetRange(startX, startY, width, height);
            return (TDerived)this;
        }

        /// <summary>
        /// 使用 MatrixRange 设置范围。
        /// </summary>
        /// <param name="matrixRange">用于设置的 MatrixRange 对象。</param>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived SetRange(MatrixRange matrixRange)
        {
            base.SetRange(matrixRange);
            return (TDerived)this;
        }

        /* clear */

        /// <summary>
        /// 清除所有与噪声相关的值（frequency, octaves, maxHeight, minHeight）。
        /// </summary>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public TDerived ClearValue()
        {
            this.frequency = 0.0;
            this.octaves = 0;
            this.maxHeight = 0;
            this.minHeight = 0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除频率（frequency）。
        /// </summary>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public TDerived ClearFrequency()
        {
            this.frequency = 0.0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除倍频数（octaves）。
        /// </summary>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public TDerived ClearOctaves()
        {
            this.octaves = 0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除最小高度（minHeight）。
        /// </summary>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public TDerived ClearMinHeight()
        {
            this.minHeight = 0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除最大高度（maxHeight）。
        /// </summary>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public TDerived ClearMaxHeight()
        {
            this.maxHeight = 0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除所有设置（范围 + 值）。
        /// </summary>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public TDerived Clear()
        {
            this.ClearRange();
            this.ClearValue();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除起始 X（链式）。
        /// </summary>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived ClearPointX()
        {
            base.ClearPointX();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除起始 Y（链式）。
        /// </summary>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived ClearPointY()
        {
            base.ClearPointY();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除点设置（链式）。
        /// </summary>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived ClearPoint()
        {
            base.ClearPoint();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除宽度（链式）。
        /// </summary>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived ClearWidth()
        {
            base.ClearWidth();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除高度（链式）。
        /// </summary>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived ClearHeight()
        {
            base.ClearHeight();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除长度/线性范围（链式）。
        /// </summary>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived ClearLength()
        {
            base.ClearLength();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除矩形范围（链式）。
        /// </summary>
        /// <returns>返回当前对象的派生类型以支持链式调用。</returns>
        public new TDerived ClearRange()
        {
            base.ClearRange();
            return (TDerived)this;
        }

        /// <summary>
        /// 默认构造函数，创建一个空的 RectBasePerlin 实例。
        /// </summary>
        public RectBasePerlin()
        {
        }

        /// <summary>
        /// 使用起始坐标和尺寸构造 RectBasePerlin（不设置噪声参数）。
        /// </summary>
        /// <param name="startX">起始 X。</param>
        /// <param name="startY">起始 Y。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        public RectBasePerlin(uint startX, uint startY, uint width, uint height) : base(startX, startY, width, height)
        {
        }

        /// <summary>
        /// 使用频率构造 RectBasePerlin。
        /// </summary>
        /// <param name="frequency">频率值。</param>
        public RectBasePerlin(double frequency)
        {
            this.frequency = frequency;
        }

        /// <summary>
        /// 使用频率和倍频数构造 RectBasePerlin。
        /// </summary>
        /// <param name="frequency">频率值。</param>
        /// <param name="octaves">倍频数。</param>
        public RectBasePerlin(double frequency, uint octaves)
        {
            this.frequency = frequency;
            this.octaves = octaves;
        }

        /// <summary>
        /// 使用频率、倍频数和最大高度构造 RectBasePerlin。
        /// </summary>
        /// <param name="frequency">频率值。</param>
        /// <param name="octaves">倍频数。</param>
        /// <param name="maxHeight">最大高度。</param>
        public RectBasePerlin(double frequency, uint octaves, int maxHeight)
        {
            this.frequency = frequency;
            this.octaves = octaves;
            this.maxHeight = maxHeight;
        }

        /// <summary>
        /// 使用频率、倍频数、最大高度和最小高度构造 RectBasePerlin。
        /// </summary>
        /// <param name="frequency">频率值。</param>
        /// <param name="octaves">倍频数。</param>
        /// <param name="maxHeight">最大高度。</param>
        /// <param name="minHeight">最小高度。</param>
        public RectBasePerlin(double frequency, uint octaves, int maxHeight, int minHeight)
        {
            this.frequency = frequency;
            this.octaves = octaves;
            this.maxHeight = maxHeight;
            this.minHeight = minHeight;
        }

        /// <summary>
        /// 使用 MatrixRange 和频率构造 RectBasePerlin。
        /// </summary>
        /// <param name="matrixRange">用于初始化范围的 MatrixRange。</param>
        /// <param name="frequency">频率值。</param>
        public RectBasePerlin(MatrixRange matrixRange, double frequency) : base(matrixRange)
        {
            this.frequency = frequency;
        }

        /// <summary>
        /// 使用 MatrixRange、频率和倍频数构造 RectBasePerlin。
        /// </summary>
        /// <param name="matrixRange">用于初始化范围的 MatrixRange。</param>
        /// <param name="frequency">频率值。</param>
        /// <param name="octaves">倍频数。</param>
        public RectBasePerlin(MatrixRange matrixRange, double frequency, uint octaves) : base(matrixRange)
        {
            this.frequency = frequency;
            this.octaves = octaves;
        }

        /// <summary>
        /// 使用 MatrixRange、频率、倍频数和最大高度构造 RectBasePerlin。
        /// </summary>
        /// <param name="matrixRange">用于初始化范围的 MatrixRange。</param>
        /// <param name="frequency">频率值。</param>
        /// <param name="octaves">倍频数。</param>
        /// <param name="maxHeight">最大高度。</param>
        public RectBasePerlin(MatrixRange matrixRange, double frequency, uint octaves, int maxHeight) : base(
            matrixRange)
        {
            this.frequency = frequency;
            this.octaves = octaves;
            this.maxHeight = maxHeight;
        }

        /// <summary>
        /// 使用 MatrixRange、频率、倍频数、最大高度和最小高度构造 RectBasePerlin。
        /// </summary>
        /// <param name="matrixRange">用于初始化范围的 MatrixRange。</param>
        /// <param name="frequency">频率值。</param>
        /// <param name="octaves">倍频数。</param>
        /// <param name="maxHeight">最大高度。</param>
        /// <param name="minHeight">最小高度。</param>
        public RectBasePerlin(MatrixRange matrixRange, double frequency, uint octaves, int maxHeight, int minHeight) :
            base(matrixRange)
        {
            this.frequency = frequency;
            this.octaves = octaves;
            this.maxHeight = maxHeight;
            this.minHeight = minHeight;
        }

        /// <summary>
        /// 使用范围和频率构造 RectBasePerlin。
        /// </summary>
        /// <param name="startX">起始 X。</param>
        /// <param name="startY">起始 Y。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <param name="frequency">频率值。</param>
        public RectBasePerlin(uint startX, uint startY, uint width, uint height, double frequency) : base(startX,
            startY, width, height)
        {
            this.frequency = frequency;
        }

        /// <summary>
        /// 使用范围、频率和倍频数构造 RectBasePerlin。
        /// </summary>
        /// <param name="startX">起始 X。</param>
        /// <param name="startY">起始 Y。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <param name="frequency">频率值。</param>
        /// <param name="octaves">倍频数。</param>
        public RectBasePerlin(uint startX, uint startY, uint width, uint height, double frequency, uint octaves) : base(
            startX, startY, width, height)
        {
            this.frequency = frequency;
            this.octaves = octaves;
        }

        /// <summary>
        /// 使用范围、频率、倍频数和最大高度构造 RectBasePerlin。
        /// </summary>
        /// <param name="startX">起始 X。</param>
        /// <param name="startY">起始 Y。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <param name="frequency">频率值。</param>
        /// <param name="octaves">倍频数。</param>
        /// <param name="maxHeight">最大高度。</param>
        public RectBasePerlin(uint startX, uint startY, uint width, uint height, double frequency, uint octaves,
            int maxHeight) : base(startX, startY, width, height)
        {
            this.frequency = frequency;
            this.octaves = octaves;
            this.maxHeight = maxHeight;
        }

        /// <summary>
        /// 使用范围、频率、倍频数、最大高度和最小高度构造 RectBasePerlin。
        /// </summary>
        /// <param name="startX">起始 X。</param>
        /// <param name="startY">起始 Y。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <param name="frequency">频率值。</param>
        /// <param name="octaves">倍频数。</param>
        /// <param name="maxHeight">最大高度。</param>
        /// <param name="minHeight">最小高度。</param>
        public RectBasePerlin(uint startX, uint startY, uint width, uint height, double frequency, uint octaves,
            int maxHeight, int minHeight) : base(startX, startY, width, height)
        {
            this.frequency = frequency;
            this.octaves = octaves;
            this.maxHeight = maxHeight;
            this.minHeight = minHeight;
        }
    }
}
