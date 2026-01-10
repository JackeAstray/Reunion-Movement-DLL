using System;
using System.Collections.Generic;
using System.Text;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Range
{
    /// <summary>
    /// 带单一整数值的矩形基类。封装了矩形的范围信息并携带一个整型的绘制值。
    /// TDerived 为派生类类型，用于实现流式 API（返回派生类型以便链式调用）。
    /// </summary>
    /// <typeparam name="TDerived">派生类类型，必须继承自 RectBaseWithValue&lt;TDerived&gt;</typeparam>
    public class RectBaseWithValue<TDerived> : BasicRect<RectBaseWithValue<TDerived>> where TDerived : RectBaseWithValue<TDerived>
    {
        /// <summary>
        /// 绘制时使用的整型值（例如用于表示类型或权重）。受保护以便派生类可以读取但外部只能通过方法操作。
        /// </summary>
        public int drawValue { get; protected set; }

        /* Getter */

        /// <summary>
        /// 通过引用获取当前对象的 drawValue 值并返回当前对象用于链式调用。
        /// </summary>
        /// <param name="value">输出参数，调用后包含当前的 drawValue。</param>
        /// <returns>返回当前对象（TDerived）以便链式调用。</returns>
        public TDerived GetValue(ref int value)
        {
            value = this.drawValue;
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用获取起始 X 并返回当前对象用于链式调用。
        /// </summary>
        /// <param name="value">输出参数，调用后包含起始 X。</param>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived GetPointX(ref uint value)
        {
            base.GetPointX(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用获取起始 Y 并返回当前对象用于链式调用。
        /// </summary>
        /// <param name="value">输出参数，调用后包含起始 Y。</param>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived GetPointY(ref uint value)
        {
            base.GetPointY(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用获取高度并返回当前对象用于链式调用。
        /// </summary>
        /// <param name="value">输出参数，调用后包含高度。</param>
        /// <returns>返回当前对象（TDerived）。returns</returns>
        public new TDerived GetHeight(ref uint value)
        {
            // 调用基类实现以填充引用参数
            base.GetHeight(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用获取宽度并返回当前对象用于链式调用。
        /// </summary>
        /// <param name="value">输出参数，调用后包含宽度。</param>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived GetWidth(ref uint value)
        {
            base.GetWidth(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用获取起始点（X、Y）并返回当前对象用于链式调用。
        /// </summary>
        /// <param name="value">输出参数，调用后包含起始 X。</param>
        /// <param name="value2">输出参数，调用后包含起始 Y。</param>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived GetPoint(ref uint value, ref uint value2)
        {
            base.GetPoint(ref value, ref value2);
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用获取矩形范围（startX, startY, width, height）并返回当前对象用于链式调用。
        /// </summary>
        /// <param name="value">输出参数，调用后包含 startX。</param>
        /// <param name="value2">输出参数，调用后包含 startY。</param>
        /// <param name="value3">输出参数，调用后包含 width。</param>
        /// <param name="value4">输出参数，调用后包含 height。</param>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived GetRange(ref uint value, ref uint value2, ref uint value3, ref uint value4)
        {
            base.GetRange(ref value, ref value2, ref value3, ref value4);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取起始 X 的拷贝值。
        /// </summary>
        /// <returns>起始 X。</returns>
        public new uint GetPointX()
        {
            return base.GetPointX();
        }

        /// <summary>
        /// 获取起始 Y 的拷贝值。
        /// </summary>
        /// <returns>起始 Y。</returns>
        public new uint GetPointY()
        {
            return base.GetPointY();
        }

        /// <summary>
        /// 获取宽度的拷贝值。
        /// </summary>
        /// <returns>宽度。</returns>
        public new uint GetWidth()
        {
            return base.GetWidth();
        }

        /// <summary>
        /// 获取高度的拷贝值。
        /// </summary>
        /// <returns>高度。</returns>
        public new uint GetHeight()
        {
            return base.GetHeight();
        }

        /* Clear */

        /// <summary>
        /// 清除起始 X（将其重置为基类定义的未设置状态），并返回当前对象用于链式调用。
        /// </summary>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived ClearPointX()
        {
            base.ClearPointX();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除起始 Y 并返回当前对象用于链式调用。
        /// </summary>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived ClearPointY()
        {
            base.ClearPointY();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除起始点（X、Y）并返回当前对象用于链式调用。
        /// </summary>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived ClearPoint()
        {
            base.ClearPoint();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除宽度并返回当前对象用于链式调用。
        /// </summary>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived ClearWidth()
        {
            base.ClearWidth();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除高度并返回当前对象用于链式调用。
        /// </summary>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived ClearHeight()
        {
            base.ClearHeight();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除表示长度的字段（如基类中定义）并返回当前对象用于链式调用。
        /// </summary>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived ClearLength()
        {
            // 调用基类实现以保持行为一致
            base.ClearLength();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除完整的范围信息（起始点与尺寸）并返回当前对象用于链式调用。
        /// </summary>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived ClearRange()
        {
            base.ClearRange();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除 drawValue（将其重置为 0）并返回当前对象用于链式调用。
        /// </summary>
        /// <returns>返回当前对象（TDerived）。</returns>
        public TDerived ClearValue()
        {
            drawValue = 0;
            return (TDerived)this;
        }

        /// <summary>
        /// 同时清除范围信息与 drawValue，等价于 ClearRange() + ClearValue()。
        /// </summary>
        /// <returns>返回当前对象（TDerived）。</returns>
        public TDerived Clear()
        {
            this.ClearRange();
            this.ClearValue();
            return (TDerived)this;
        }

        /* Setter */

        /// <summary>
        /// 设置 drawValue 的值并返回当前对象用于链式调用。
        /// </summary>
        /// <param name="drawValue">要设置的整数值。</param>
        /// <returns>返回当前对象（TDerived）。</returns>
        public TDerived SetValue(int drawValue)
        {
            this.drawValue = drawValue;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置起始 X 并返回当前对象用于链式调用。
        /// </summary>
        /// <param name="startX">起始 X。</param>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived SetPointX(uint startX)
        {
            base.SetPointX(startX);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置起始 Y 并返回当前对象用于链式调用。
        /// </summary>
        /// <param name="startY">起始 Y。</param>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived SetPointY(uint startY)
        {
            base.SetPointY(startY);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置宽度并返回当前对象用于链式调用。
        /// </summary>
        /// <param name="width">宽度。</param>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived SetWidth(uint width)
        {
            base.SetWidth(width);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置高度并返回当前对象用于链式调用。
        /// </summary>
        /// <param name="height">高度。</param>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived SetHeight(uint height)
        {
            base.SetHeight(height);
            return (TDerived)this;
        }

        /// <summary>
        /// 以单个 uint 值设置起始坐标（如果基类支持此语义），并返回当前对象用于链式调用。
        /// </summary>
        /// <param name="point">表示点的 uint 值（基类语义）。</param>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived SetPoint(uint point)
        {
            base.SetPoint(point);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置起始坐标（startX, startY）并返回当前对象用于链式调用。
        /// </summary>
        /// <param name="startX">起始 X。</param>
        /// <param name="startY">起始 Y。</param>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived SetPoint(uint startX, uint startY)
        {
            base.SetPoint(startX, startY);
            return (TDerived)this;
        }

        /// <summary>
        /// 使用起始坐标与长度（视为一维长度）设置范围并返回当前对象用于链式调用。
        /// </summary>
        /// <param name="startX">起始 X。</param>
        /// <param name="startY">起始 Y。</param>
        /// <param name="length">单一维度的长度。</param>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived SetRange(uint startX, uint startY, uint length)
        {
            base.SetRange(startX, startY, length);
            return (TDerived)this;
        }

        /// <summary>
        /// 使用起始坐标与宽高设置范围并返回当前对象用于链式调用。
        /// </summary>
        /// <param name="startX">起始 X。</param>
        /// <param name="startY">起始 Y。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived SetRange(uint startX, uint startY, uint width, uint height)
        {
            base.SetRange(startX, startY, width, height);
            return (TDerived)this;
        }

        /// <summary>
        /// 使用基类的 MatrixRange 结构设置范围并返回当前对象用于链式调用。
        /// </summary>
        /// <param name="matrixRange">基类定义的矩形范围结构。</param>
        /// <returns>返回当前对象（TDerived）。</returns>
        public new TDerived SetRange(MatrixRange matrixRange)
        {
            base.SetRange(matrixRange);
            return (TDerived)this;
        }


        // constructors

        /// <summary>
        /// 默认构造函数。不会设置任何范围或 drawValue（drawValue 默认为 0）。
        /// </summary>
        public RectBaseWithValue()
        {
        }

        /// <summary>
        /// 使用指定的 drawValue 和 MatrixRange 初始化矩形。
        /// </summary>
        /// <param name="drawValue">要设置的 drawValue。</param>
        /// <param name="matrixRange">用于初始化基类范围的 MatrixRange。</param>
        public RectBaseWithValue(int drawValue, MatrixRange matrixRange) : base(matrixRange)
        {
            this.drawValue = drawValue;
        }

        /// <summary>
        /// 使用指定的 drawValue 及起始坐标与尺寸初始化矩形。
        /// </summary>
        /// <param name="drawValue">要设置的 drawValue。</param>
        /// <param name="startX">起始 X。</param>
        /// <param name="startY">起始 Y。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        public RectBaseWithValue(int drawValue, uint startX, uint startY, uint width, uint height) : base(startX,
            startY, width,
            height)
        {
            this.drawValue = drawValue;
        }

        /// <summary>
        /// 仅使用 drawValue 初始化对象，不设置范围信息。
        /// </summary>
        /// <param name="drawValue">要设置的 drawValue。</param>
        public RectBaseWithValue(int drawValue)
        {
            this.drawValue = drawValue;
        }
    }
}
