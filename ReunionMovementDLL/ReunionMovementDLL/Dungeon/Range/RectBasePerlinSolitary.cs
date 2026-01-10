using System;
using System.Collections.Generic;
using System.Text;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Range
{
    /// <summary>
    /// 单峰 Perlin 噪声矩形基类
    /// </summary>
    /// <typeparam name="TDerived">派生类型，应为自身泛型类型</typeparam>
    public class RectBasePerlinSolitary<TDerived> : BasicRect<RectBasePerlinSolitary<TDerived>> where TDerived : RectBasePerlinSolitary<TDerived>
    {
        /// <summary>
        /// 截断比（用于控制噪声截断的比值）
        /// </summary>
        public double truncatedProportion { get; protected set; }

        /// <summary>
        /// 山体比重（用于控制山体部分的权重）
        /// </summary>
        public double mountainProportion { get; protected set; }

        /// <summary>
        /// 频率（或值别名），控制噪声缩放
        /// </summary>
        public double frequency { get; protected set; }

        /// <summary>
        /// 倍频数（octaves），影响分形噪声的层数
        /// </summary>
        public uint octaves { get; protected set; }

        /// <summary>
        /// 最小高度，用于高度范围约束
        /// </summary>
        public int minHeight { get; protected set; }

        /// <summary>
        /// 最大高度，用于高度范围约束
        /// </summary>
        public int maxHeight { get; protected set; }

        /* Getter */

        /// <summary>
        /// 获取截断比
        /// </summary>
        /// <returns>当前的截断比</returns>
        public double GetTruncatedProportion()
        {
            return this.truncatedProportion;
        }

        /// <summary>
        /// 获取山体比重
        /// </summary>
        /// <returns>当前的山体比重</returns>
        public double GetMountainProportion()
        {
            return this.mountainProportion;
        }

        /// <summary>
        /// 获取频率
        /// </summary>
        /// <returns>当前频率</returns>
        public double GetFrequency()
        {
            return this.frequency;
        }

        /// <summary>
        /// 获取倍频数（以 double 返回以兼容旧接口）
        /// </summary>
        /// <returns>当前 octaves 的数值（转换为 double）</returns>
        public double GetOctaves()
        {
            return this.octaves;
        }

        /// <summary>
        /// 获取最小高度
        /// </summary>
        /// <returns>当前最小高度</returns>
        public int GetMinHeight()
        {
            return this.minHeight;
        }

        /// <summary>
        /// 获取最大高度
        /// </summary>
        /// <returns>当前最大高度</returns>
        public int GetMaxHeight()
        {
            return this.maxHeight;
        }

        /// <summary>
        /// 获取值（频率别名）
        /// </summary>
        /// <returns>当前频率</returns>
        public double GetValue()
        {
            return this.frequency;
        }

        /// <summary>
        /// 获取起点 X（通过引用传入并由基类设置）——支持链式调用
        /// </summary>
        /// <param name="value">引用传入的 uint 值，用于接收 X</param>
        /// <returns>链式返回当前实例</returns>
        public new TDerived GetPointX(ref uint value)
        {
            base.GetPointX(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取起点 Y（通过引用传入并由基类设置）——支持链式调用
        /// </summary>
        /// <param name="value">引用传入的 uint 值，用于接收 Y</param>
        /// <returns>链式返回当前实例</returns>
        public new TDerived GetPointY(ref uint value)
        {
            base.GetPointY(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取高度（通过引用传入并由基类设置）——支持链式调用
        /// </summary>
        /// <param name="value">引用传入的 uint 值，用于接收高度</param>
        /// <returns>链式返回当前实例</returns>
        public new TDerived GetHeight(ref uint value)
        {
            // 调用基类实现并返回自身以支持链式调用
            base.GetHeight(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取宽度（通过引用传入并由基类设置）——支持链式调用
        /// </summary>
        /// <param name="value">引用传入的 uint 值，用于接收宽度</param>
        /// <returns>链式返回当前实例</returns>
        public new TDerived GetWidth(ref uint value)
        {
            base.GetWidth(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取点坐标（通过引用传入并由基类设置）——支持链式调用
        /// </summary>
        /// <param name="value">引用传入的 X</param>
        /// <param name="value2">引用传入的 Y</param>
        /// <returns>链式返回当前实例</returns>
        public new TDerived GetPoint(ref uint value, ref uint value2)
        {
            base.GetPoint(ref value, ref value2);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取矩形范围（通过引用传入并由基类设置）——支持链式调用
        /// </summary>
        /// <param name="value">引用传入的 startX</param>
        /// <param name="value2">引用传入的 startY</param>
        /// <param name="value3">引用传入的 width</param>
        /// <param name="value4">引用传入的 height</param>
        /// <returns>链式返回当前实例</returns>
        public new TDerived GetRange(ref uint value, ref uint value2, ref uint value3, ref uint value4)
        {
            base.GetRange(ref value, ref value2, ref value3, ref value4);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取起点 X（返回值）
        /// </summary>
        /// <returns>起点 X</returns>
        public new uint GetPointX()
        {
            return base.GetPointX();
        }

        /// <summary>
        /// 获取起点 Y（返回值）
        /// </summary>
        /// <returns>起点 Y</returns>
        public new uint GetPointY()
        {
            // 返回 Y
            return base.GetPointY();
        }

        /// <summary>
        /// 获取宽度（返回值）
        /// </summary>
        /// <returns>宽度</returns>
        public new uint GetWidth()
        {
            return base.GetWidth();
        }

        /// <summary>
        /// 获取高度（返回值）
        /// </summary>
        /// <returns>高度</returns>
        public new uint GetHeight()
        {
            return base.GetHeight();
        }

        /* Setter */

        /// <summary>
        /// 设置截断比，并返回自身以支持链式调用
        /// </summary>
        /// <param name="truncatedProportion">截断比值</param>
        /// <returns>链式返回当前实例</returns>
        public TDerived SetTruncatedProportion(double truncatedProportion)
        {
            this.truncatedProportion = truncatedProportion;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置山体比重，并返回自身以支持链式调用
        /// </summary>
        /// <param name="mountainProportion">山体比重</param>
        /// <returns>链式返回当前实例</returns>
        public TDerived SetMountainProportion(double mountainProportion)
        {
            this.mountainProportion = mountainProportion;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置频率（值别名），并返回自身以支持链式调用
        /// </summary>
        /// <param name="frequency">频率</param>
        /// <returns>链式返回当前实例</returns>
        public TDerived SetValue(double frequency)
        {
            this.frequency = frequency;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置频率，并返回自身以支持链式调用
        /// </summary>
        /// <param name="frequency">频率</param>
        /// <returns>链式返回当前实例</returns>
        public TDerived SetFrequency(double frequency)
        {
            this.frequency = frequency;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置倍频数，并返回自身以支持链式调用
        /// </summary>
        /// <param name="octaves">倍频数</param>
        /// <returns>链式返回当前实例</returns>
        public TDerived SetOctaves(uint octaves)
        {
            this.octaves = octaves;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置最小高度，并返回自身以支持链式调用
        /// </summary>
        /// <param name="minHeight">最小高度</param>
        /// <returns>链式返回当前实例</returns>
        public TDerived SetMinHeight(int minHeight)
        {
            this.minHeight = minHeight;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置最大高度，并返回自身以支持链式调用
        /// </summary>
        /// <param name="maxHeight">最大高度</param>
        /// <returns>链式返回当前实例</returns>
        public TDerived SetMaxHeight(int maxHeight)
        {
            this.maxHeight = maxHeight;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置起点 X 并返回自身以支持链式调用
        /// </summary>
        /// <param name="startX">起点 X</param>
        /// <returns>链式返回当前实例</returns>
        public new TDerived SetPointX(uint startX)
        {
            base.SetPointX(startX);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置起点 Y 并返回自身以支持链式调用
        /// </summary>
        /// <param name="startY">起点 Y</param>
        /// <returns>链式返回当前实例</returns>
        public new TDerived SetPointY(uint startY)
        {
            base.SetPointY(startY);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置宽度并返回自身以支持链式调用
        /// </summary>
        /// <param name="width">宽度</param>
        /// <returns>链式返回当前实例</returns>
        public new TDerived SetWidth(uint width)
        {
            base.SetWidth(width);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置高度并返回自身以支持链式调用
        /// </summary>
        /// <param name="height">高度</param>
        /// <returns>链式返回当前实例</returns>
        public new TDerived SetHeight(uint height)
        {
            base.SetHeight(height);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置单一点（以整型表示）并返回自身以支持链式调用
        /// </summary>
        /// <param name="point">点</param>
        /// <returns>链式返回当前实例</returns>
        public new TDerived SetPoint(uint point)
        {
            base.SetPoint(point);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置起点坐标并返回自身以支持链式调用
        /// </summary>
        /// <param name="startX">起点 X</param>
        /// <param name="startY">起点 Y</param>
        /// <returns>链式返回当前实例</returns>
        public new TDerived SetPoint(uint startX, uint startY)
        {
            base.SetPoint(startX, startY);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置范围（startX, startY, length）并返回自身以支持链式调用
        /// </summary>
        /// <param name="startX">起点 X</param>
        /// <param name="startY">起点 Y</param>
        /// <param name="length">长度</param>
        /// <returns>链式返回当前实例</returns>
        public new TDerived SetRange(uint startX, uint startY, uint length)
        {
            base.SetRange(startX, startY, length);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置矩形范围并返回自身以支持链式调用
        /// </summary>
        /// <param name="startX">起点 X</param>
        /// <param name="startY">起点 Y</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>链式返回当前实例</returns>
        public new TDerived SetRange(uint startX, uint startY, uint width, uint height)
        {
            base.SetRange(startX, startY, width, height);
            return (TDerived)this;
        }

        /// <summary>
        /// 使用 MatrixRange 设置范围并返回自身以支持链式调用
        /// </summary>
        /// <param name="matrixRange">矩阵范围对象</param>
        /// <returns>链式返回当前实例</returns>
        public new TDerived SetRange(MatrixRange matrixRange)
        {
            base.SetRange(matrixRange);
            return (TDerived)this;
        }


        /* clear */

        /// <summary>
        /// 清除与噪声相关的所有值（频率、截断比、山体比重、倍频数、最小/最大高度）
        /// </summary>
        /// <returns>链式返回当前实例</returns>
        public TDerived ClearValue()
        {
            this.frequency = 0.0;
            this.truncatedProportion = 0.0;
            this.mountainProportion = 0.0;
            this.octaves = 0;
            this.minHeight = 0;
            this.maxHeight = 0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除截断比
        /// </summary>
        /// <returns>链式返回当前实例</returns>
        public TDerived ClearTruncatedProportion()
        {
            this.truncatedProportion = 0.0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除山体比重
        /// </summary>
        /// <returns>链式返回当前实例</returns>
        public TDerived ClearMountainProportion()
        {
            this.mountainProportion = 0.0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除频率
        /// </summary>
        /// <returns>链式返回当前实例</returns>
        public TDerived ClearFrequency()
        {
            this.frequency = 0.0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除倍频数
        /// </summary>
        /// <returns>链式返回当前实例</returns>
        public TDerived ClearOctaves()
        {
            this.octaves = 0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除最小高度
        /// </summary>
        /// <returns>链式返回当前实例</returns>
        public TDerived ClearMinHeight()
        {
            this.minHeight = 0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除最大高度
        /// </summary>
        /// <returns>链式返回当前实例</returns>
        public TDerived ClearMaxHeight()
        {
            this.maxHeight = 0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除所有设置（值与范围）并返回自身
        /// </summary>
        /// <returns>链式返回当前实例</returns>
        public TDerived Clear()
        {
            this.ClearValue();
            this.ClearRange();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除起点 X 并返回自身
        /// </summary>
        /// <returns>链式返回当前实例</returns>
        public new TDerived ClearPointX()
        {
            base.ClearPointX();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除起点 Y 并返回自身
        /// </summary>
        /// <returns>链式返回当前实例</returns>
        public new TDerived ClearPointY()
        {
            base.ClearPointY();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除点并返回自身
        /// </summary>
        /// <returns>链式返回当前实例</returns>
        public new TDerived ClearPoint()
        {
            base.ClearPoint();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除宽度并返回自身
        /// </summary>
        /// <returns>链式返回当前实例</returns>
        public new TDerived ClearWidth()
        {
            base.ClearWidth();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除高度并返回自身
        /// </summary>
        /// <returns>链式返回当前实例</returns>
        public new TDerived ClearHeight()
        {
            base.ClearHeight();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除长度并返回自身
        /// </summary>
        /// <returns>链式返回当前实例</returns>
        public new TDerived ClearLength()
        {
            base.ClearLength();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除范围并返回自身
        /// </summary>
        /// <returns>链式返回当前实例</returns>
        public new TDerived ClearRange()
        {
            base.ClearRange();
            return (TDerived)this;
        }

        /* constructors */

        /// <summary>
        /// 默认构造函数，创建空实例
        /// </summary>
        public RectBasePerlinSolitary()
        {
        } // = default()

        /// <summary>
        /// 使用坐标和尺寸构造矩形范围
        /// </summary>
        /// <param name="startX">起点 X</param>
        /// <param name="startY">起点 Y</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public RectBasePerlinSolitary(uint startX, uint startY, uint width, uint height) : base(startX, startY, width,
            height)
        {
        }

        /// <summary>
        /// 使用截断比构造
        /// </summary>
        /// <param name="truncatedProportion">截断比</param>
        public RectBasePerlinSolitary(double truncatedProportion)
        {
            this.truncatedProportion = truncatedProportion;
        }

        /// <summary>
        /// 使用截断比和山体比重构造
        /// </summary>
        /// <param name="truncatedProportion">截断比</param>
        /// <param name="mountainProportion">山体比重</param>
        public RectBasePerlinSolitary(double truncatedProportion, double mountainProportion)
        {
            this.truncatedProportion = truncatedProportion;
            this.mountainProportion = mountainProportion;
        }

        /// <summary>
        /// 使用截断比、山体比重和频率构造
        /// </summary>
        /// <param name="truncatedProportion">截断比</param>
        /// <param name="mountainProportion">山体比重</param>
        /// <param name="frequency">频率</param>
        public RectBasePerlinSolitary(double truncatedProportion, double mountainProportion, double frequency)
        {
            this.truncatedProportion = truncatedProportion;
            this.mountainProportion = mountainProportion;
            this.frequency = frequency;
        }

        /// <summary>
        /// 使用截断比、山体比重、频率和倍频数构造
        /// </summary>
        /// <param name="truncatedProportion">截断比</param>
        /// <param name="mountainProportion">山体比重</param>
        /// <param name="frequency">频率</param>
        /// <param name="octaves">倍频数</param>
        public RectBasePerlinSolitary(double truncatedProportion, double mountainProportion, double frequency,
            uint octaves)
        {
            this.truncatedProportion = truncatedProportion;
            this.mountainProportion = mountainProportion;
            this.frequency = frequency;
            this.octaves = octaves;
        }

        /// <summary>
        /// 使用截断比、山体比重、频率、倍频数和最大高度构造
        /// </summary>
        /// <param name="truncatedProportion">截断比</param>
        /// <param name="mountainProportion">山体比重</param>
        /// <param name="frequency">频率</param>
        /// <param name="octaves">倍频数</param>
        /// <param name="maxHeight">最大高度</param>
        public RectBasePerlinSolitary(double truncatedProportion, double mountainProportion, double frequency,
            uint octaves, int maxHeight)
        {
            this.truncatedProportion = truncatedProportion;
            this.mountainProportion = mountainProportion;
            this.frequency = frequency;
            this.octaves = octaves;
            this.maxHeight = maxHeight;
        }

        /// <summary>
        /// 使用截断比、山体比重、频率、倍频数、最大高度和最小高度构造
        /// </summary>
        /// <param name="truncatedProportion">截断比</param>
        /// <param name="mountainProportion">山体比重</param>
        /// <param name="frequency">频率</param>
        /// <param name="octaves">倍频数</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="minHeight">最小高度</param>
        public RectBasePerlinSolitary(double truncatedProportion, double mountainProportion, double frequency,
            uint octaves, int maxHeight, int minHeight)
        {
            this.truncatedProportion = truncatedProportion;
            this.mountainProportion = mountainProportion;
            this.frequency = frequency;
            this.octaves = octaves;
            this.maxHeight = maxHeight;
            this.minHeight = minHeight;
        }

        /// <summary>
        /// 使用 MatrixRange 和截断比构造
        /// </summary>
        /// <param name="matrixRange">矩阵范围</param>
        /// <param name="truncatedProportion">截断比</param>
        public RectBasePerlinSolitary(MatrixRange matrixRange, double truncatedProportion) : base(matrixRange)
        {
            this.truncatedProportion = truncatedProportion;
        }

        /// <summary>
        /// 使用 MatrixRange、截断比和山体比重构造
        /// </summary>
        /// <param name="matrixRange">矩阵范围</param>
        /// <param name="truncatedProportion">截断比</param>
        /// <param name="mountainProportion">山体比重</param>
        public RectBasePerlinSolitary(MatrixRange matrixRange, double truncatedProportion, double mountainProportion) :
            base(matrixRange)
        {
            this.truncatedProportion = truncatedProportion;
            this.mountainProportion = mountainProportion;
        }

        /// <summary>
        /// 使用 MatrixRange、截断比、山体比重和频率构造
        /// </summary>
        /// <param name="matrixRange">矩阵范围</param>
        /// <param name="truncatedProportion">截断比</param>
        /// <param name="mountainProportion">山体比重</param>
        /// <param name="frequency">频率</param>
        public RectBasePerlinSolitary(MatrixRange matrixRange, double truncatedProportion, double mountainProportion,
            double frequency) : base(matrixRange)
        {
            this.truncatedProportion = truncatedProportion;
            this.mountainProportion = mountainProportion;
            this.frequency = frequency;
        }

        /// <summary>
        /// 使用 MatrixRange、截断比、山体比重、频率和倍频数构造
        /// </summary>
        /// <param name="matrixRange">矩阵范围</param>
        /// <param name="truncatedProportion">截断比</param>
        /// <param name="mountainProportion">山体比重</param>
        /// <param name="frequency">频率</param>
        /// <param name="octaves">倍频数</param>
        public RectBasePerlinSolitary(MatrixRange matrixRange, double truncatedProportion, double mountainProportion,
            double frequency, uint octaves) : base(matrixRange)
        {
            this.truncatedProportion = truncatedProportion;
            this.mountainProportion = mountainProportion;
            this.frequency = frequency;
            this.octaves = octaves;
        }

        /// <summary>
        /// 使用 MatrixRange、截断比、山体比重、频率、倍频数和最大高度构造
        /// </summary>
        /// <param name="matrixRange">矩阵范围</param>
        /// <param name="truncatedProportion">截断比</param>
        /// <param name="mountainProportion">山体比重</param>
        /// <param name="frequency">频率</param>
        /// <param name="octaves">倍频数</param>
        /// <param name="maxHeight">最大高度</param>
        public RectBasePerlinSolitary(MatrixRange matrixRange, double truncatedProportion, double mountainProportion,
            double frequency, uint octaves, int maxHeight) : base(matrixRange)
        {
            this.truncatedProportion = truncatedProportion;
            this.mountainProportion = mountainProportion;
            this.frequency = frequency;
            this.octaves = octaves;
            this.maxHeight = maxHeight;
        }

        /// <summary>
        /// 使用 MatrixRange、截断比、山体比重、频率、倍频数、最大高度和最小高度构造
        /// </summary>
        /// <param name="matrixRange">矩阵范围</param>
        /// <param name="truncatedProportion">截断比</param>
        /// <param name="mountainProportion">山体比重</param>
        /// <param name="frequency">频率</param>
        /// <param name="octaves">倍频数</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="minHeight">最小高度</param>
        public RectBasePerlinSolitary(MatrixRange matrixRange, double truncatedProportion, double mountainProportion,
            double frequency, uint octaves, int maxHeight, int minHeight) : base(matrixRange)
        {
            this.truncatedProportion = truncatedProportion;
            this.mountainProportion = mountainProportion;
            this.frequency = frequency;
            this.octaves = octaves;
            this.maxHeight = maxHeight;
            this.minHeight = minHeight;
        }

        /// <summary>
        /// 使用坐标、尺寸和截断比构造
        /// </summary>
        /// <param name="startX">起点 X</param>
        /// <param name="startY">起点 Y</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="truncatedProportion">截断比</param>
        public RectBasePerlinSolitary(uint startX, uint startY, uint width, uint height, double truncatedProportion) :
            base(startX, startY, width, height)
        {
            this.truncatedProportion = truncatedProportion;
        }

        /// <summary>
        /// 使用坐标、尺寸、截断比和山体比重构造
        /// </summary>
        /// <param name="startX">起点 X</param>
        /// <param name="startY">起点 Y</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="truncatedProportion">截断比</param>
        /// <param name="mountainProportion">山体比重</param>
        public RectBasePerlinSolitary(uint startX, uint startY, uint width, uint height, double truncatedProportion,
            double mountainProportion) : base(startX, startY, width, height)
        {
            this.truncatedProportion = truncatedProportion;
            this.mountainProportion = mountainProportion;
        }

        /// <summary>
        /// 使用坐标、尺寸、截断比、山体比重和频率构造
        /// </summary>
        /// <param name="startX">起点 X</param>
        /// <param name="startY">起点 Y</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="truncatedProportion">截断比</param>
        /// <param name="mountainProportion">山体比重</param>
        /// <param name="frequency">频率</param>
        public RectBasePerlinSolitary(uint startX, uint startY, uint width, uint height, double truncatedProportion,
            double mountainProportion, double frequency) : base(startX, startY, width, height)
        {
            this.truncatedProportion = truncatedProportion;
            this.mountainProportion = mountainProportion;
            this.frequency = frequency;
        }

        /// <summary>
        /// 使用坐标、尺寸、截断比、山体比重、频率和倍频数构造
        /// </summary>
        /// <param name="startX">起点 X</param>
        /// <param name="startY">起点 Y</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="truncatedProportion">截断比</param>
        /// <param name="mountainProportion">山体比重</param>
        /// <param name="frequency">频率</param>
        /// <param name="octaves">倍频数</param>
        public RectBasePerlinSolitary(uint startX, uint startY, uint width, uint height, double truncatedProportion,
            double mountainProportion, double frequency, uint octaves) : base(startX, startY, width, height)
        {
            this.truncatedProportion = truncatedProportion;
            this.mountainProportion = mountainProportion;
            this.frequency = frequency;
            this.octaves = octaves;
        }

        /// <summary>
        /// 使用坐标、尺寸、截断比、山体比重、频率、倍频数和最大高度构造
        /// </summary>
        /// <param name="startX">起点 X</param>
        /// <param name="startY">起点 Y</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="truncatedProportion">截断比</param>
        /// <param name="mountainProportion">山体比重</param>
        /// <param name="frequency">频率</param>
        /// <param name="octaves">倍频数</param>
        /// <param name="maxHeight">最大高度</param>
        public RectBasePerlinSolitary(uint startX, uint startY, uint width, uint height, double truncatedProportion,
            double mountainProportion, double frequency, uint octaves, int maxHeight) : base(startX, startY, width,
            height)
        {
            this.truncatedProportion = truncatedProportion;
            this.mountainProportion = mountainProportion;
            this.frequency = frequency;
            this.octaves = octaves;
            this.maxHeight = maxHeight;
        }

        /// <summary>
        /// 使用坐标、尺寸、截断比、山体比重、频率、倍频数、最大高度和最小高度构造
        /// </summary>
        /// <param name="startX">起点 X</param>
        /// <param name="startY">起点 Y</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="truncatedProportion">截断比</param>
        /// <param name="mountainProportion">山体比重</param>
        /// <param name="frequency">频率</param>
        /// <param name="octaves">倍频数</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="minHeight">最小高度</param>
        public RectBasePerlinSolitary(uint startX, uint startY, uint width, uint height, double truncatedProportion,
            double mountainProportion, double frequency, uint octaves, int maxHeight, int minHeight) : base(startX,
            startY, width, height)
        {
            this.truncatedProportion = truncatedProportion;
            this.mountainProportion = mountainProportion;
            this.frequency = frequency;
            this.octaves = octaves;
            this.maxHeight = maxHeight;
            this.minHeight = minHeight;
        }
    }
}
