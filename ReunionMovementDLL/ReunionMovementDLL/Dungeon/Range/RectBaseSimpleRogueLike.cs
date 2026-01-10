using System;
using System.Collections.Generic;
using System.Text;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Range
{
    /// <summary>
    /// 简化的 RogueLike 矩形基类。
    /// 该类扩展自 BasicRect，并提供房间与道路值及用于生成的简单参数（例如分裂最小值、房间最小尺寸等）。
    /// TDerived 使用 CRTP 模式以便链式调用返回具体派生类型。
    /// </summary>
    /// <typeparam name="TDerived">派生类型，必须继承自 RectBaseSimpleRogueLike&lt;TDerived&gt;</typeparam>
    public class RectBaseSimpleRogueLike<TDerived> : BasicRect<RectBaseSimpleRogueLike<TDerived>> where TDerived : RectBaseSimpleRogueLike<TDerived>
    {
        /// <summary>
        /// 房间值（用于表示房间区域的整型标记）。
        /// </summary>
        public int roomValue { get; protected set; }

        /// <summary>
        /// 道路值（用于表示道路/连通区域的整型标记）。
        /// </summary>
        public int roadValue { get; protected set; }

        /// <summary>
        /// 分割时的最小分割数（用于简化的分割算法）。默认 3。
        /// </summary>
        public uint divisionMin { get; protected set; } = 3;

        /// <summary>
        /// 分割时随机偏移的最大值（用于简化的分割算法）。默认 4。
        /// </summary>
        public uint divisionRandMax { get; protected set; } = 4;

        /// <summary>
        /// 房间在 X 方向的最小尺寸。默认 5。
        /// </summary>
        public uint roomMinX { get; protected set; } = 5;

        /// <summary>
        /// 房间在 X 方向随机额外尺寸的最大值。默认 2。
        /// </summary>
        public uint roomRandMaxX { get; protected set; } = 2;

        /// <summary>
        /// 房间在 Y 方向的最小尺寸。默认 5。
        /// </summary>
        public uint roomMinY { get; protected set; } = 5;

        /// <summary>
        /// 房间在 Y 方向随机额外尺寸的最大值。默认 2。
        /// </summary>
        public uint roomRandMaxY { get; protected set; } = 2;

        /* Getter */

        /// <summary>
        /// 通过引用参数获取房间值。
        /// </summary>
        /// <param name="value">输出参数，返回当前的 roomValue。</param>
        /// <returns>返回当前实例以便链式调用（TDerived）。</returns>
        public TDerived GetRoom(ref int value)
        {
            value = roomValue;
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用参数获取道路值。
        /// </summary>
        /// <param name="value">输出参数，返回当前的 roadValue。</param>
        /// <returns>返回当前实例以便链式调用（TDerived）。</returns>
        public TDerived GetRoad(ref int value)
        {
            value = roadValue;
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用参数获取道路值（别名：Way）。
        /// </summary>
        /// <param name="value">输出参数，返回当前的 roadValue。</param>
        /// <returns>返回当前实例以便链式调用（TDerived）。</returns>
        public TDerived GetWay(ref int value)
        {
            value = roadValue;
            return (TDerived)this;
        }

        /// <summary>
        /// 获取道路值（值语义）。
        /// </summary>
        /// <returns>返回当前的 roadValue。</returns>
        public int GetWay()
        {
            return this.roadValue;
        }

        /// <summary>
        /// 获取房间值（值语义）。
        /// </summary>
        /// <returns>返回当前的 roomValue。</returns>
        public int GetRoom()
        {
            return this.roomValue;
        }

        /// <summary>
        /// 获取道路值（值语义）。
        /// </summary>
        /// <returns>返回当前的 roadValue。</returns>
        public int GetRoad()
        {
            return this.roadValue;
        }

        /// <summary>
        /// 获取值（房间别名，返回 roomValue）。
        /// </summary>
        /// <returns>返回当前的 roomValue。</returns>
        public int GetValue()
        {
            return this.roomValue;
        }

        /// <summary>
        /// 获取矩形起点 X（通过引用参数）。调用基类实现并返回当前实例。
        /// </summary>
        /// <param name="value">输出参数，返回 X。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived GetPointX(ref uint value)
        {
            base.GetPointX(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取矩形起点 Y（通过引用参数）。调用基类实现并返回当前实例。
        /// </summary>
        /// <param name="value">输出参数，返回 Y。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived GetPointY(ref uint value)
        {
            base.GetPointY(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取矩形高度（通过引用参数）。调用基类实现并返回当前实例。
        /// </summary>
        /// <param name="value">输出参数，返回高度。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived GetHeight(ref uint value)
        {
            base.GetHeight(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取矩形宽度（通过引用参数）。调用基类实现并返回当前实例。
        /// </summary>
        /// <param name="value">输出参数，返回宽度。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived GetWidth(ref uint value)
        {
            base.GetWidth(ref value);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取矩形起点（通过引用返回 X 和 Y）。调用基类实现并返回当前实例。
        /// </summary>
        /// <param name="value">输出参数，返回 X。</param>
        /// <param name="value2">输出参数，返回 Y。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived GetPoint(ref uint value, ref uint value2)
        {
            base.GetPoint(ref value, ref value2);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取矩形范围（通过四个引用参数返回 startX, startY, width, height）。调用基类实现并返回当前实例。
        /// </summary>
        /// <param name="value">输出参数，返回 startX。</param>
        /// <param name="value2">输出参数，返回 startY。</param>
        /// <param name="value3">输出参数，返回 width。</param>
        /// <param name="value4">输出参数，返回 height。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived GetRange(ref uint value, ref uint value2, ref uint value3, ref uint value4)
        {
            base.GetRange(ref value, ref value2, ref value3, ref value4);
            return (TDerived)this;
        }

        /// <summary>
        /// 获取矩形起点 X（值语义）。
        /// </summary>
        /// <returns>返回 X。</returns>
        public new uint GetPointX()
        {
            return base.GetPointX();
        }

        /// <summary>
        /// 获取矩形起点 Y（值语义）。
        /// </summary>
        /// <returns>返回 Y。</returns>
        public new uint GetPointY()
        {
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

        // 消去 (clear) //

        /// <summary>
        /// 清除房间值（roomValue 设为 0）。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived ClearRoom()
        {
            roomValue = 0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除道路值（roadValue 设为 0）。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived ClearWay()
        {
            roadValue = 0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除道路值（别名 ClearRoad）。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived ClearRoad()
        {
            roadValue = 0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除房间与道路的值（分别调用 ClearRoom 与 ClearRoad）。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived ClearValue()
        {
            ClearRoom();
            ClearRoad();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除所有设置（包括范围与值）。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived Clear()
        {
            ClearRange();
            ClearValue();
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

        // Setter //

        /// <summary>
        /// 设置房间值。
        /// </summary>
        /// <param name="roomValue">要设置的房间值。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived SetRoom(int roomValue)
        {
            this.roomValue = roomValue;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置道路值（Way）。
        /// </summary>
        /// <param name="roadValue">要设置的道路值。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived SetWay(int roadValue)
        {
            this.roadValue = roadValue;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置道路值（别名 SetRoad）。
        /// </summary>
        /// <param name="roadValue">要设置的道路值。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived SetRoad(int roadValue)
        {
            this.roadValue = roadValue;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置简化 RogueLike 算法的参数（分割与房间尺寸相关）。
        /// </summary>
        /// <param name="divisionMin">分割最小值。</param>
        /// <param name="divisionRandMax">分割随机最大偏差。</param>
        /// <param name="roomMinX">房间 X 最小值。</param>
        /// <param name="roomRandMaxX">房间 X 随机最大偏差。</param>
        /// <param name="roomMinY">房间 Y 最小值。</param>
        /// <param name="roomRandMaxY">房间 Y 随机最大偏差。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived SetRogueLike(uint divisionMin, uint divisionRandMax, uint roomMinX,
            uint roomRandMaxX, uint roomMinY, uint roomRandMaxY)
        {
            this.divisionMin = divisionMin;
            this.divisionRandMax = divisionRandMax;
            this.roomMinX = roomMinX;
            this.roomRandMaxX = roomRandMaxX;
            this.roomMinY = roomMinY;
            this.roomRandMaxY = roomRandMaxY;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置矩形起点 X 并返回当前实例。
        /// </summary>
        /// <param name="startX">起点 X。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived SetPointX(uint startX)
        {
            base.SetPointX(startX);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置矩形起点 Y 并返回当前实例。
        /// </summary>
        /// <param name="startY">起点 Y。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived SetPointY(uint startY)
        {
            base.SetPointY(startY);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置矩形宽度并返回当前实例。
        /// </summary>
        /// <param name="width">宽度。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived SetWidth(uint width)
        {
            base.SetWidth(width);
            return (TDerived)this;
        }

        /// <summary>
        /// 设置矩形高度并返回当前实例。
        /// </summary>
        /// <param name="height">高度。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public new TDerived SetHeight(uint height)
        {
            base.SetHeight(height);
            return (TDerived)this;
        }

        /// <summary>
        /// 使用单点索引设置起点并返回当前实例。
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
        /// 默认构造函数，初始化为未设置的矩形范围与默认参数。
        /// </summary>
        public RectBaseSimpleRogueLike()
        {
        } // = default();

        /// <summary>
        /// 使用指定房间值构造对象。
        /// </summary>
        /// <param name="roomValue">要初始化的房间值。</param>
        public RectBaseSimpleRogueLike(int roomValue)
        {
            this.roomValue = roomValue;
        }

        /// <summary>
        /// 使用指定房间值与道路值构造对象。
        /// </summary>
        /// <param name="roomValue">房间值。</param>
        /// <param name="roadValue">道路值。</param>
        public RectBaseSimpleRogueLike(int roomValue, int roadValue)
        {
            this.roomValue = roomValue;
            this.roadValue = roadValue;
        }

        /// <summary>
        /// 使用完整参数构造对象（房间/道路值与生成参数）。
        /// </summary>
        /// <param name="roomValue">房间值。</param>
        /// <param name="roadValue">道路值。</param>
        /// <param name="divisionMin">分割最小值。</param>
        /// <param name="divisionRandMax">分割随机最大偏差。</param>
        /// <param name="roomMinX">房间 X 最小值。</param>
        /// <param name="roomRandMaxX">房间 X 随机最大偏差。</param>
        /// <param name="roomMinY">房间 Y 最小值。</param>
        /// <param name="roomRandMaxY">房间 Y 随机最大偏差。</param>
        public RectBaseSimpleRogueLike(int roomValue, int roadValue, uint divisionMin,
            uint divisionRandMax, uint roomMinX, uint roomRandMaxX, uint roomMinY, uint roomRandMaxY)
        {
            this.roomValue = roomValue;
            this.roadValue = roadValue;
            this.divisionMin = divisionMin;
            this.divisionRandMax = divisionRandMax;
            this.roomMinX = roomMinX;
            this.roomRandMaxX = roomRandMaxX;
            this.roomMinY = roomMinY;
            this.roomRandMaxY = roomRandMaxY;
        }

        /// <summary>
        /// 使用矩阵范围初始化矩形的起点与尺寸（从 MatrixRange 提取 x,y,w,h）。
        /// </summary>
        /// <param name="matrixRange">矩阵范围对象，包含 x, y, w, h。</param>
        public RectBaseSimpleRogueLike(MatrixRange matrixRange)
        {
            this.startX = (uint)matrixRange.x;
            this.startY = (uint)matrixRange.y;
            this.width = (uint)matrixRange.w;
            this.height = (uint)matrixRange.h;
        }

        /// <summary>
        /// 使用矩阵范围与房间值构造对象。
        /// </summary>
        /// <param name="matrixRange">矩阵范围对象。</param>
        /// <param name="roomValue">房间值。</param>
        public RectBaseSimpleRogueLike(MatrixRange matrixRange, int roomValue) : this(matrixRange)
        {
            this.roomValue = roomValue;
        }

        /// <summary>
        /// 使用矩阵范围、房间值与道路值构造对象。
        /// </summary>
        /// <param name="matrixRange">矩阵范围对象。</param>
        /// <param name="roomValue">房间值。</param>
        /// <param name="roadValue">道路值。</param>
        public RectBaseSimpleRogueLike(MatrixRange matrixRange, int roomValue, int roadValue) : this(matrixRange,
            roomValue)
        {
            this.roadValue = roadValue;
        }

        /// <summary>
        /// 使用矩阵范围、房间/道路值与生成参数构造对象。
        /// </summary>
        /// <param name="matrixRange">矩阵范围对象。</param>
        /// <param name="roomValue">房间值。</param>
        /// <param name="roadValue">道路值。</param>
        /// <param name="divisionMin">分割最小值。</param>
        /// <param name="divisionRandMax">分割随机最大偏差。</param>
        /// <param name="roomMinX">房间 X 最小值。</param>
        /// <param name="roomRandMaxX">房间 X 随机最大偏差。</param>
        /// <param name="roomMinY">房间 Y 最小值。</param>
        /// <param name="roomRandMaxY">房间 Y 随机最大偏差。</param>
        public RectBaseSimpleRogueLike(MatrixRange matrixRange, int roomValue, int roadValue, uint divisionMin,
            uint divisionRandMax, uint roomMinX, uint roomRandMaxX, uint roomMinY, uint roomRandMaxY)
            : this(matrixRange, roomValue, roadValue)
        {
            this.divisionMin = divisionMin;
            this.divisionRandMax = divisionRandMax;
            this.roomMinX = roomMinX;
            this.roomRandMaxX = roomRandMaxX;
            this.roomMinY = roomMinY;
            this.roomRandMaxY = roomRandMaxY;
        }
    }
}
