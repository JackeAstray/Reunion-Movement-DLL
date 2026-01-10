using System;
using System.Collections.Generic;
using System.Text;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Range
{
    /// <summary>
    /// 基本矩形范围类
    /// </summary>
    /// <typeparam name="TDerived"></typeparam>
    public class BasicRect<TDerived> where TDerived : BasicRect<TDerived>
    {
        public uint startX { get; protected set; }
        public uint startY { get; protected set; }
        public uint width { get; protected set; }
        public uint height { get; protected set; }

        /// <summary>
        /// 计算结束 X 坐标（不超过 maxX）。当 width 为 0 时视为到达边界（返回 maxX）。
        /// </summary>
        /// <param name="maxX">矩阵最大 X（排除或包含视用法而定）。</param>
        /// <returns>计算得到的结束 X 坐标。</returns>
        protected uint CalcEndX(uint maxX)
        {
            return (this.width == 0 || this.startX + this.width >= maxX) ? maxX : this.startX + this.width;
        }

        /// <summary>
        /// 计算结束 Y 坐标（不超过 maxY）。当 height 为 0 时视为到达边界（返回 maxY）。
        /// </summary>
        /// <param name="maxY">矩阵最大 Y。</param>
        /// <returns>计算得到的结束 Y 坐标。</returns>
        protected uint CalcEndY(uint maxY)
        {
            return (this.height == 0 || this.startY + this.height >= maxY) ? maxY : this.startY + this.height;
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public BasicRect()
        {
        }
        /// <summary>
        /// 根据矩阵范围构造 BasicRect
        /// </summary>
        /// <param name="matrixRange">矩阵范围（x,y,w,h）</param>
        public BasicRect(MatrixRange matrixRange)
        {
            this.startX = (uint)matrixRange.x;
            this.startY = (uint)matrixRange.y;
            this.width = (uint)matrixRange.w;
            this.height = (uint)matrixRange.h;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="startX">起始 X</param>
        /// <param name="startY">起始 Y</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public BasicRect(uint startX, uint startY, uint width, uint height)
        {
            this.startX = startX;
            this.startY = startY;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// 清除起始 X（设置为 0）。
        /// </summary>
        /// <returns>当前实例以便链式调用。</returns>
        public TDerived ClearPointX()
        {
            this.startX = 0;
            return (TDerived)this;
        }
        /// <summary>
        /// 清除起始 Y（设置为 0）。
        /// </summary>
        /// <returns>当前实例以便链式调用。</returns>
        public TDerived ClearPointY()
        {
            this.startY = 0;
            return (TDerived)this;
        }
        /// <summary>
        /// 清除起始点（X 与 Y 都置 0）。
        /// </summary>
        /// <returns>当前实例以便链式调用。</returns>
        public TDerived ClearPoint()
        {
            this.startX = 0;
            this.startY = 0;
            return (TDerived)this;
        }
        /// <summary>
        /// 清除宽度（设置为 0）。
        /// </summary>
        /// <returns>当前实例以便链式调用。</returns>
        public TDerived ClearWidth()
        {
            this.width = 0;
            return (TDerived)this;
        }
        /// <summary>
        /// 清除高度（设置为 0）。
        /// </summary>
        /// <returns>当前实例以便链式调用。</returns>
        public TDerived ClearHeight()
        {
            this.height = 0;
            return (TDerived)this;
        }
        /// <summary>
        /// 清除长度（宽度与高度均置 0）。
        /// </summary>
        /// <returns>当前实例以便链式调用。</returns>
        public TDerived ClearLength()
        {
            ClearWidth();
            ClearHeight();
            return (TDerived)this;
        }
        /// <summary>
        /// 清除整个范围（起始点与长度）。
        /// </summary>
        /// <returns>当前实例以便链式调用。</returns>
        public TDerived ClearRange()
        {
            ClearLength();
            ClearPointX();
            ClearPointY();
            return (TDerived)this;
        }
        /// <summary>
        /// 通过引用获取起始 X。
        /// </summary>
        /// <param name="value">输出参数，用于接收起始 X。</param>
        /// <returns>当前实例以便链式调用。</returns>
        public TDerived GetPointX(ref uint value)
        {
            value = this.startX;
            return (TDerived)this;
        }
        /// <summary>
        /// 通过引用获取起始 Y。
        /// </summary>
        /// <param name="value">输出参数，用于接收起始 Y。</param>
        /// <returns>当前实例以便链式调用。</returns>
        public TDerived GetPointY(ref uint value)
        {
            value = this.startY;
            return (TDerived)this;
        }
        /// <summary>
        /// 通过引用获取高度。
        /// </summary>
        /// <param name="value">输出参数，用于接收高度。</param>
        /// <returns>当前实例以便链式调用。</returns>
        public TDerived GetHeight(ref uint value)
        {
            value = this.height;
            return (TDerived)this;
        }
        /// <summary>
        /// 通过引用获取宽度。
        /// </summary>
        /// <param name="value">输出参数，用于接收宽度。</param>
        /// <returns>当前实例以便链式调用。</returns>
        public TDerived GetWidth(ref uint value)
        {
            value = this.width;
            return (TDerived)this;
        }
        /// <summary>
        /// 通过引用获取起始点坐标。
        /// </summary>
        /// <param name="value">输出 X。</param>
        /// <param name="value2">输出 Y。</param>
        /// <returns>当前实例以便链式调用。</returns>
        public TDerived GetPoint(ref uint value, ref uint value2)
        {
            value = this.startX;
            value2 = this.startY;
            return (TDerived)this;
        }
        /// <summary>
        /// 通过引用获取范围（起始点与尺寸）。
        /// </summary>
        /// <param name="value">输出 X。</param>
        /// <param name="value2">输出 Y。</param>
        /// <param name="value3">输出 宽度。</param>
        /// <param name="value4">输出 高度。</param>
        /// <returns>当前实例以便链式调用。</returns>
        public TDerived GetRange(ref uint value, ref uint value2, ref uint value3, ref uint value4)
        {
            value = this.startX;
            value2 = this.startY;
            value3 = this.width;
            value4 = this.height;
            return (TDerived)this;
        }
        /// <summary>
        /// 获取起始 X（返回值）。
        /// </summary>
        /// <returns>起始 X。</returns>
        public uint GetPointX()
        {
            return this.startX;
        }
        /// <summary>
        /// 获取起始 Y（返回值）。
        /// </summary>
        /// <returns>起始 Y。</returns>
        public uint GetPointY()
        {
            return this.startY;
        }
        /// <summary>
        /// 获取宽度（返回值）。
        /// </summary>
        /// <returns>宽度。</returns>
        public uint GetWidth()
        {
            return this.width;
        }
        /// <summary>
        /// 获取高度（返回值）。
        /// </summary>
        /// <returns>高度。</returns>
        public uint GetHeight()
        {
            return this.height;
        }
        /// <summary>
        /// 设置起始 X 并返回当前实例。
        /// </summary>
        public TDerived SetPointX(uint startX)
        {
            this.startX = startX;
            return (TDerived)this;
        }
        /// <summary>
        /// 设置起始 Y 并返回当前实例。
        /// </summary>
        public TDerived SetPointY(uint startY)
        {
            this.startY = startY;
            return (TDerived)this;
        }
        /// <summary>
        /// 设置宽度并返回当前实例。
        /// </summary>
        public TDerived SetWidth(uint width)
        {
            this.width = width;
            return (TDerived)this;
        }
        /// <summary>
        /// 设置高度并返回当前实例。
        /// </summary>
        public TDerived SetHeight(uint height)
        {
            this.height = height;
            return (TDerived)this;
        }
        /// <summary>
        /// 将起始点设置为相同的 X/Y 值并返回当前实例。
        /// </summary>
        public TDerived SetPoint(uint point)
        {
            this.startX = point;
            this.startY = point;
            return (TDerived)this;
        }
        /// <summary>
        /// 设置起始点坐标并返回当前实例。
        /// </summary>
        public TDerived SetPoint(uint startX, uint startY)
        {
            this.startX = startX;
            this.startY = startY;
            return (TDerived)this;
        }
        /// <summary>
        /// 设置正方形范围（width = height = length）。
        /// </summary>
        public TDerived SetRange(uint startX, uint startY, uint length)
        {
            this.startX = startX;
            this.startY = startY;
            this.width = length;
            this.height = length;
            return (TDerived)this;
        }
        /// <summary>
        /// 设置矩形范围并返回当前实例。
        /// </summary>
        public TDerived SetRange(uint startX, uint startY, uint width, uint height)
        {
            this.startX = startX;
            this.startY = startY;
            this.width = width;
            this.height = height;
            return (TDerived)this;
        }
        /// <summary>
        /// 使用矩阵范围设置范围并返回当前实例。
        /// </summary>
        public TDerived SetRange(MatrixRange matrixRange)
        {
            this.startX = (uint)matrixRange.x;
            this.startY = (uint)matrixRange.y;
            this.width = (uint)matrixRange.w;
            this.height = (uint)matrixRange.h;
            return (TDerived)this;
        }
    }
}