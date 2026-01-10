using System;
using System.Collections.Generic;
using System.Text;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Range
{
    /// <summary>
    /// 带 IList 值的矩形基类。
    /// 该类继承自 BasicRect，并在矩形范围外额外携带一个整数列表（drawValue），通常用于绘制或标记用途。
    /// TDerived 使用 CRTP 模式以便链式调用返回具体派生类型。
    /// </summary>
    /// <typeparam name="TDerived">派生类型，必须继承自 RectBaseWithIList&lt;TDerived&gt;</typeparam>
    public class RectBaseWithIList<TDerived> : BasicRect<RectBaseWithIList<TDerived>> where TDerived : RectBaseWithIList<TDerived>
    {
        /// <summary>
        /// 存放额外整数值的列表，默认初始化为空 List&lt;int&gt;。
        /// 派生类或外部代码可以通过 SetValue/ GetValue 进行访问或替换。
        /// </summary>
        public IList<int> drawValue { get; protected set; } = new List<int>();

        /* Getter */

        /// <summary>
        /// 通过引用参数获取当前的 drawValue 列表引用。
        /// </summary>
        /// <param name="value">输出参数，返回当前 drawValue 引用。</param>
        /// <returns>返回当前实例以便链式调用（TDerived）。</returns>
        public TDerived GetValue(ref IList<int> value)
        {
            value = this.drawValue;
            return (TDerived)this;
        }

        /// <summary>
        /// 获取矩形起点 X（通过引用参数）。
        /// 调用基类实现并返回当前实例以便链式调用。
        /// </summary>
        /// <param name="value">输出参数，返回 X 坐标。</param>
        /// <returns>返回当前实例（TDerived）。</returns>
        public new TDerived GetPointX(ref uint value)
        {
            base.GetPointX(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取矩形起点 Y（通过引用参数）。
        /// 调用基类实现并返回当前实例以便链式调用。
        /// </summary>
        /// <param name="value">输出参数，返回 Y 坐标。</param>
        /// <returns>返回当前实例（TDerived）。</returns>
        public new TDerived GetPointY(ref uint value)
        {
            base.GetPointY(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取矩形高度（通过引用参数）。
        /// 调用基类实现并返回当前实例以便链式调用。
        /// </summary>
        /// <param name="value">输出参数，返回高度。</param>
        /// <returns>返回当前实例（TDerived）。</returns>
        public new TDerived GetHeight(ref uint value)
        {
            // 调用基类实现以获取高度
            base.GetHeight(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取矩形宽度（通过引用参数）。
        /// 调用基类实现并返回当前实例以便链式调用。
        /// </summary>
        /// <param name="value">输出参数，返回宽度。</param>
        /// <returns>返回当前实例（TDerived）。</returns>
        public new TDerived GetWidth(ref uint value)
        {
            base.GetWidth(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取矩形起点（通过两个引用参数返回 X 和 Y）。
        /// 调用基类实现并返回当前实例以便链式调用。
        /// </summary>
        /// <param name="value">输出参数，返回 X 坐标。</param>
        /// <param name="value2">输出参数，返回 Y 坐标。</param>
        /// <returns>返回当前实例（TDerived）。</returns>
        public new TDerived GetPoint(ref uint value, ref uint value2)
        {
            base.GetPoint(ref value, ref value2);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取矩形范围（通过四个引用参数返回 startX, startY, width, height）。
        /// 调用基类实现并返回当前实例以便链式调用。
        /// </summary>
        /// <param name="value">输出参数，返回 startX。</param>
        /// <param name="value2">输出参数，返回 startY。</param>
        /// <param name="value3">输出参数，返回 width。</param>
        /// <param name="value4">输出参数，返回 height。</param>
        /// <returns>返回当前实例（TDerived）。</returns>
        public new TDerived GetRange(ref uint value, ref uint value2, ref uint value3, ref uint value4)
        {
            base.GetRange(ref value, ref value2, ref value3, ref value4);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取矩形起点 X（值语义）。
        /// </summary>
        /// <returns>返回 X 坐标。</returns>
        public new uint GetPointX()
        {
            return base.GetPointX();
        }

        /// <summary>
        /// 获取矩形起点 Y（值语义）。
        /// </summary>
        /// <returns>返回 Y 坐标。</returns>
        public new uint GetPointY()
        {
            // 直接返回基类实现的 Y 值
            return base.GetPointY();
        }

        /// <summary>
        /// 获取矩形宽度（值语义）。
        /// </summary>
        /// <returns>返回宽度。</returns>
        public new uint GetWidth()
        {
            return base.GetWidth();
        }

        /// <summary>
        /// 获取矩形高度（值语义）。
        /// </summary>
        /// <returns>返回高度。</returns>
        public new uint GetHeight()
        {
            return base.GetHeight();
        }

        /// <summary>
        /// 清空 drawValue 列表中的所有元素（安全的空检查）。
        /// </summary>
        /// <returns>返回当前实例以便链式调用（TDerived）。</returns>
        public TDerived ClearValue()
        {
            this.drawValue?.Clear();
            return (TDerived)this;
        }

        /* Clear */

        /// <summary>
        /// 清空 drawValue 和 矩形范围（通过 ClearRange）。
        /// </summary>
        /// <returns>返回当前实例以便链式调用（TDerived）。returns>
        public TDerived Clear()
        {
            this.ClearValue();
            this.ClearRange();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除起点 X 的设置并返回当前实例。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived ClearPointX()
        {
            base.ClearPointX();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除起点 Y 的设置并返回当前实例。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived ClearPointY()
        {
            base.ClearPointY();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除起点（X,Y）的设置并返回当前实例。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived ClearPoint()
        {
            base.ClearPoint();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除宽度设置并返回当前实例。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived ClearWidth()
        {
            base.ClearWidth();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除高度设置并返回当前实例。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived ClearHeight()
        {
            base.ClearHeight();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除长度相关设置并返回当前实例。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived ClearLength()
        {
            base.ClearLength();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除整个范围设置并返回当前实例。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived ClearRange()
        {
            base.ClearRange();
            return (TDerived)this;
        }

        /* Setter */

        /// <summary>
        /// 用指定列表替换当前的 drawValue。若传入 null，则使用空列表替代，保证 drawValue 不为 null。
        /// </summary>
        /// <param name="drawValue">用于设置的 IList&lt;int&gt;，若为 null 则使用空列表。</param>
        /// <returns>当前实例以便链式调用（TDerived）。</returns>
        public TDerived SetValue(IList<int> drawValue)
        {
            this.drawValue = drawValue ?? new List<int>();
            return (TDerived)this;
        }

        /// <summary>
        /// 设置起点 X 并返回当前实例。
        /// </summary>
        /// <param name="startX">起点 X。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived SetPointX(uint startX)
        {
            base.SetPointX(startX);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置起点 Y 并返回当前实例。
        /// </summary>
        /// <param name="startY">起点 Y。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived SetPointY(uint startY)
        {
            base.SetPointY(startY);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置宽度并返回当前实例。
        /// </summary>
        /// <param name="width">宽度。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived SetWidth(uint width)
        {
            base.SetWidth(width);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置高度并返回当前实例。
        /// </summary>
        /// <param name="height">高度。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived SetHeight(uint height)
        {
            base.SetHeight(height);
            return (TDerived)this;
        }

        /// <summary>
        /// 使用单一点值设置起点（点索引）并返回当前实例。
        /// </summary>
        /// <param name="point">点索引。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived SetPoint(uint point)
        {
            base.SetPoint(point);
            return (TDerived)this;
        }

        /// <summary>
        /// 使用 X 和 Y 设置起点并返回当前实例。
        /// </summary>
        /// <param name="startX">起点 X。</param>
        /// <param name="startY">起点 Y。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived SetPoint(uint startX, uint startY)
        {
            base.SetPoint(startX, startY);
            return (TDerived)this;
        }

        /// <summary>
        /// 使用 startX, startY, length 设置范围（长度方式）并返回当前实例。
        /// </summary>
        /// <param name="startX">起点 X。</param>
        /// <param name="startY">起点 Y。</param>
        /// <param name="length">长度。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived SetRange(uint startX, uint startY, uint length)
        {
            base.SetRange(startX, startY, length);
            return (TDerived)this;
        }

        /// <summary>
        /// 使用 startX, startY, width, height 设置范围并返回当前实例。
        /// </summary>
        /// <param name="startX">起点 X。</param>
        /// <param name="startY">起点 Y。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived SetRange(uint startX, uint startY, uint width, uint height)
        {
            base.SetRange(startX, startY, width, height);
            return (TDerived)this;
        }

        /// <summary>
        /// 使用另一个矩阵范围对象设置当前范围并返回当前实例。
        /// </summary>
        /// <param name="matrixRange">用于设置的矩阵范围对象。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived SetRange(MatrixRange matrixRange)
        {
            base.SetRange(matrixRange);
            return (TDerived)this;
        }

        /* Constructors */

        /// <summary>
        /// 默认构造函数，初始化为默认空 drawValue 列表与未设置的矩形范围。
        /// </summary>
        public RectBaseWithIList() { } // default

        /// <summary>
        /// 使用指定的 drawValue 列表构造对象。
        /// 若传入 null，则会使用空列表替代，确保 drawValue 不为 null。
        /// </summary>
        /// <param name="drawValue">用于初始化的 drawValue 列表，可为 null。</param>
        public RectBaseWithIList(IList<int> drawValue)
        {
            this.drawValue = drawValue ?? new List<int>();
        }

        /// <summary>
        /// 使用指定的矩阵范围和 drawValue 列表构造对象。
        /// 若传入 drawValue 为 null，则会使用空列表替代。
        /// </summary>
        /// <param name="matrixRange">用于初始化的矩阵范围。</param>
        /// <param name="drawValue">用于初始化的 drawValue 列表，可为 null。</param>
        public RectBaseWithIList(MatrixRange matrixRange, IList<int> drawValue) : base(matrixRange)
        {
            this.drawValue = drawValue ?? new List<int>();
        }
    }
}
