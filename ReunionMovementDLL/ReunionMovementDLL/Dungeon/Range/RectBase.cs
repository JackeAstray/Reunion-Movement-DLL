using System;
using System.Collections.Generic;
using System.Text;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Range
{
    /// <summary>
    /// 矩形基类
    /// </summary>
    /// <typeparam name="TDerived"></typeparam>
    public class RectBase<TDerived> : BasicRect<RectBase<TDerived>> where TDerived : RectBase<TDerived>
    {
        /// <summary>
        /// 获取X坐标（通过引用参数）
        /// </summary>
        /// <param name="value">输出 X 坐标</param>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived GetPointX(ref uint value)
        {
            base.GetPointX(ref value);
            return (TDerived)this;
        }
        /// <summary>
        /// 获取Y坐标（通过引用参数）
        /// </summary>
        /// <param name="value">输出 Y 坐标</param>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived GetPointY(ref uint value)
        {
            base.GetPointY(ref value);
            return (TDerived)this;
        }
        /// <summary>
        /// 获取高度（通过引用参数）
        /// </summary>
        /// <param name="value">输出 高度</param>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived GetHeight(ref uint value)
        {
            // 修复：应调用 base.GetHeight
            base.GetHeight(ref value);
            return (TDerived)this;
        }
        /// <summary>
        /// 获取宽度（通过引用参数）
        /// </summary>
        /// <param name="value">输出 宽度</param>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived GetWidth(ref uint value)
        {
            base.GetWidth(ref value);
            return (TDerived)this;
        }
        /// <summary>
        /// 获取点坐标（通过引用参数）
        /// </summary>
        /// <param name="value">输出 X</param>
        /// <param name="value2">输出 Y</param>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived GetPoint(ref uint value, ref uint value2)
        {
            base.GetPoint(ref value, ref value2);
            return (TDerived)this;
        }
        /// <summary>
        /// 获取范围（通过引用参数）
        /// </summary>
        /// <param name="value">输出 X</param>
        /// <param name="value2">输出 Y</param>
        /// <param name="value3">输出 宽度</param>
        /// <param name="value4">输出 高度</param>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived GetRange(ref uint value, ref uint value2, ref uint value3, ref uint value4)
        {
            base.GetRange(ref value, ref value2, ref value3, ref value4);
            return (TDerived)this;
        }
        /// <summary>
        /// 获取X坐标（返回值）
        /// </summary>
        /// <returns>X 坐标</returns>
        public new uint GetPointX()
        {
            return base.GetPointX();
        }
        /// <summary>
        /// 获取Y坐标（返回值）
        /// </summary>
        /// <returns>Y 坐标</returns>
        public new uint GetPointY()
        {
            // 修复：返回 Y 而非 X
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
        /// <summary>
        /// 清除起始X坐标
        /// </summary>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived ClearPointX()
        {
            base.ClearPointX();
            return (TDerived)this;
        }
        /// <summary>
        /// 清除起始Y坐标
        /// </summary>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived ClearPointY()
        {
            base.ClearPointY();
            return (TDerived)this;
        }
        /// <summary>
        /// 清除起始点坐标
        /// </summary>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived ClearPoint()
        {
            base.ClearPoint();
            return (TDerived)this;
        }
        /// <summary>
        /// 清除宽度
        /// </summary>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived ClearWidth()
        {
            base.ClearWidth();
            return (TDerived)this;
        }
        /// <summary>
        /// 清除高度
        /// </summary>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived ClearHeight()
        {
            base.ClearHeight();
            return (TDerived)this;
        }
        /// <summary>
        /// 清除长度（宽度和高度）
        /// </summary>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived ClearLength()
        {
            // 修复：调用 base.ClearLength() 以同时清除宽度和高度
            base.ClearLength();
            return (TDerived)this;
        }
        /// <summary>
        /// 清除范围（包含坐标和尺寸）
        /// </summary>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived ClearRange()
        {
            base.ClearRange();
            return (TDerived)this;
        }
        /// <summary>
        /// 清除所有设置（范围）
        /// </summary>
        /// <returns>当前实例（链式调用）</returns>
        public TDerived Clear()
        {
            this.ClearRange();
            return (TDerived)this;
        }
        /// <summary>
        /// 设置X坐标
        /// </summary>
        /// <param name="startX">起始 X 坐标</param>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived SetPointX(uint startX)
        {
            base.SetPointX(startX);
            return (TDerived)this;
        }
        /// <summary>
        /// 设置Y坐标
        /// </summary>
        /// <param name="startY">起始 Y 坐标</param>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived SetPointY(uint startY)
        {
            base.SetPointY(startY);
            return (TDerived)this;
        }
        /// <summary>
        /// 设置宽度
        /// </summary>
        /// <param name="width">宽度</param>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived SetWidth(uint width)
        {
            base.SetWidth(width);
            return (TDerived)this;
        }
        /// <summary>
        /// 设置高度
        /// </summary>
        /// <param name="height">高度</param>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived SetHeight(uint height)
        {
            base.SetHeight(height);
            return (TDerived)this;
        }
        /// <summary>
        /// 设置起始点（相同 X 和 Y）
        /// </summary>
        /// <param name="point">起始坐标</param>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived SetPoint(uint point)
        {
            base.SetPoint(point);
            return (TDerived)this;
        }
        /// <summary>
        /// 设置起始点
        /// </summary>
        /// <param name="startX">起始 X</param>
        /// <param name="startY">起始 Y</param>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived SetPoint(uint startX, uint startY)
        {
            base.SetPoint(startX, startY);
            return (TDerived)this;
        }
        /// <summary>
        /// 设置正方形范围
        /// </summary>
        /// <param name="startX">起始 X</param>
        /// <param name="startY">起始 Y</param>
        /// <param name="length">边长</param>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived SetRange(uint startX, uint startY, uint length)
        {
            base.SetRange(startX, startY, length);
            return (TDerived)this;
        }
        /// <summary>
        /// 设置范围（矩形）
        /// </summary>
        /// <param name="startX">起始 X</param>
        /// <param name="startY">起始 Y</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived SetRange(uint startX, uint startY, uint width, uint height)
        {
            base.SetRange(startX, startY, width, height);
            return (TDerived)this;
        }
        /// <summary>
        /// 设置范围（使用矩阵范围结构）
        /// </summary>
        /// <param name="matrixRange">矩阵范围</param>
        /// <returns>当前实例（链式调用）</returns>
        public new TDerived SetRange(MatrixRange matrixRange)
        {
            base.SetRange(matrixRange);
            return (TDerived)this;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public RectBase()
        {
        }
        /// <summary>
        /// 构造函数（使用矩阵范围）
        /// </summary>
        /// <param name="matrixRange">矩阵范围</param>
        public RectBase(MatrixRange matrixRange) : base(matrixRange)
        {
        }
        /// <summary>
        /// 构造函数（指定坐标与尺寸）
        /// </summary>
        /// <param name="startX">起始 X</param>
        /// <param name="startY">起始 Y</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public RectBase(uint startX, uint startY, uint width, uint height) : base(startX, startY, width, height)
        {
        }
    }
}