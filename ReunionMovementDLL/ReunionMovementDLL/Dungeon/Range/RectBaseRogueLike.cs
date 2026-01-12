using ReunionMovementDLL.Dungeon.Base;
using System;
using System.Collections.Generic;
using System.Text;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Range
{
    /// <summary>
    /// RogueLike 风格矩形基类。
    /// 该类包含用于 RogueLike 地图生成的标识列表（RogueLikeList）、最大道路数以及房间/道路的推荐范围。
    /// TDerived 使用 CRTP 模式以便链式调用返回具体派生类型。
    /// </summary>
    /// <typeparam name="TDerived">派生类型，必须继承自 RectBaseRogueLike&lt;TDerived&gt;</typeparam>
    public class RectBaseRogueLike<TDerived> : BasicRect<RectBaseRogueLike<TDerived>> where TDerived : RectBaseRogueLike<TDerived>
    {
        /// <summary>
        /// 存放 RogueLike 标识的结构（外墙/内墙/房间/入口/道路 等）。
        /// </summary>
        public RogueLikeList rogueLikeList { get; protected set; } = new RogueLikeList();

        /// <summary>
        /// 最大道路数量的建议值，默认 20。
        /// </summary>
        public uint maxWay { get; protected set; } = 20;

        /// <summary>
        /// 房间大小建议范围（x,y,w,h）。可用于房间生成限制。
        /// </summary>
        protected MatrixRange roomRange = new MatrixRange(3, 3, 3, 3);

        /// <summary>
        /// 道路大小建议范围（x,y,w,h）。可用于道路生成限制。
        /// </summary>
        protected MatrixRange wayRange = new MatrixRange(3, 3, 12, 12);

        /* Get Member Value */

        /// <summary>
        /// 外墙 ID（代理至 rogueLikeList.outsideWallId）。
        /// </summary>
        public int outsideWall
        {
            get { return this.rogueLikeList.outsideWallId; }
            protected set { this.rogueLikeList.outsideWallId = value; }
        }

        /// <summary>
        /// 内墙 ID（代理至 rogueLikeList.insideWallId）。
        /// </summary>
        public int insideWall
        {
            get { return this.rogueLikeList.insideWallId; }
            protected set { this.rogueLikeList.insideWallId = value; }
        }

        /// <summary>
        /// 房间 ID（代理至 rogueLikeList.roomId）。
        /// </summary>
        public int room
        {
            get { return this.rogueLikeList.roomId; }
            protected set { this.rogueLikeList.roomId = value; }
        }

        /// <summary>
        /// 入口 ID（代理至 rogueLikeList.entranceId）。
        /// </summary>
        public int entrance
        {
            get { return this.rogueLikeList.entranceId; }
            protected set { this.rogueLikeList.entranceId = value; }
        }

        /// <summary>
        /// 道路 ID（代理至 rogueLikeList.wayId）。
        /// </summary>
        public int way
        {
            get { return this.rogueLikeList.wayId; }
            protected set { this.rogueLikeList.wayId = value; }
        }

        /// <summary>
        /// 墙 ID（当前等同于外墙 ID 的别名）。
        /// </summary>
        public int wall
        {
            get { return this.rogueLikeList.outsideWallId; }
            protected set { this.rogueLikeList.outsideWallId = value; }
        }

        /* Getter */

        /// <summary>
        /// 通过引用参数获取外墙 ID。
        /// </summary>
        /// <param name="value">输出参数，返回外墙 ID。</param>
        /// <returns>当前实例以方便链式调用（TDerived）。</returns>
        public TDerived GetOutsideWall(ref int value)
        {
            value = this.outsideWall;
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用参数获取内墙 ID。
        /// </summary>
        /// <param name="value">输出参数，返回内墙 ID。</param>
        /// <returns>当前实例以方便链式调用（TDerived）。</returns>
        public TDerived GetInsideWall(ref int value)
        {
            value = this.insideWall;
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用参数获取房间 ID。
        /// </summary>
        /// <param name="value">输出参数，返回房间 ID。</param>
        /// <returns>当前实例以方便链式调用（TDerived）。</returns>
        public TDerived GetRoom(ref int value)
        {
            value = this.room;
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用参数获取入口 ID。
        /// </summary>
        /// <param name="value">输出参数，返回入口 ID。</param>
        /// <returns>当前实例以方便链式调用（TDerived）。</returns>
        public TDerived GetEntrance(ref int value)
        {
            value = this.entrance;
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用参数获取道路 ID。
        /// </summary>
        /// <param name="value">输出参数，返回道路 ID。</param>
        /// <returns>当前实例以方便链式调用（TDerived）。</returns>
        public TDerived GetWay(ref int value)
        {
            value = this.way;
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用参数获取墙 ID（墙为外墙别名）。
        /// </summary>
        /// <param name="value">输出参数，返回墙 ID。</param>
        /// <returns>当前实例以方便链式调用（TDerived）。</returns>
        public TDerived GetWall(ref int value)
        {
            value = this.wall;
            return (TDerived)this;
        }

        /// <summary>
        /// 通过引用参数获取最大道路数。
        /// </summary>
        /// <param name="value">输出参数，返回 maxWay。</param>
        /// <returns>当前实例以方便链式调用（TDerived）。</returns>
        public TDerived GetMaxWay(ref uint value)
        {
            value = this.maxWay;
            return (TDerived)this;
        }

        /// <summary>
        /// 获取当前的 RogueLikeList 值。
        /// </summary>
        /// <returns>返回 rogueLikeList 实例（可能为默认实例）。</returns>
        public RogueLikeList GetValue()
        {
            return this.rogueLikeList;
        }

        public new TDerived GetPointX(ref uint value)
        {
            base.GetPointX(ref value);
            return (TDerived)this;
        }

        public new TDerived GetPointY(ref uint value)
        {
            base.GetPointY(ref value);
            return (TDerived)this;
        }

        public new TDerived GetHeight(ref uint value)
        {
            // 调用基类实现以获取高度
            base.GetHeight(ref value);
            return (TDerived)this;
        }

        public new TDerived GetWidth(ref uint value)
        {
            base.GetWidth(ref value);
            return (TDerived)this;
        }

        public new TDerived GetPoint(ref uint value, ref uint value2)
        {
            base.GetPoint(ref value, ref value2);
            return (TDerived)this;
        }

        public new TDerived GetRange(ref uint value, ref uint value2, ref uint value3, ref uint value4)
        {
            base.GetRange(ref value, ref value2, ref value3, ref value4);
            return (TDerived)this;
        }

        public new uint GetPointX()
        {
            return base.GetPointX();
        }

        public new uint GetPointY()
        {
            // 返回基类 Y
            return base.GetPointY();
        }

        public new uint GetWidth()
        {
            return base.GetWidth();
        }

        public new uint GetHeight()
        {
            return base.GetHeight();
        }

        /* Setter */

        /// <summary>
        /// 设置外墙 ID。
        /// </summary>
        /// <param name="value">外墙 ID。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived SetOutsideWall(int value)
        {
            this.outsideWall = value;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置内墙 ID。
        /// </summary>
        /// <param name="value">内墙 ID。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived SetInsideWall(int value)
        {
            this.insideWall = value;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置房间 ID。
        /// </summary>
        /// <param name="value">房间 ID。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived SetRoom(int value)
        {
            this.room = value;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置入口 ID。
        /// </summary>
        /// <param name="value">入口 ID。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived SetEntrance(int value)
        {
            this.entrance = value;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置道路 ID。
        /// </summary>
        /// <param name="value">道路 ID。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived SetWay(int value)
        {
            this.way = value;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置墙 ID（等同于设置外墙 ID）。
        /// </summary>
        /// <param name="value">墙 ID。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived SetWall(int value)
        {
            this.wall = value;
            return (TDerived)this;
        }

        /// <summary>
        /// 设置最大道路数。
        /// </summary>
        /// <param name="value">最大道路数。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived SetMaxWay(uint value)
        {
            this.maxWay = value;
            return (TDerived)this;
        }

        /// <summary>
        /// 使用指定的 RogueLikeList 替换当前值，如果传入 null 则使用默认实例替代以确保非空。
        /// </summary>
        /// <param name="rogueLikeList">新的 RogueLikeList，允许为 null。</param>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived SetValue(RogueLikeList rogueLikeList)
        {
            this.rogueLikeList = rogueLikeList ?? new RogueLikeList();
            return (TDerived)this;
        }

        public new TDerived SetPointX(uint startX)
        {
            base.SetPointX(startX);
            return (TDerived)this;
        }

        public new TDerived SetPointY(uint startY)
        {
            base.SetPointY(startY);
            return (TDerived)this;
        }

        public new TDerived SetWidth(uint width)
        {
            base.SetWidth(width);
            return (TDerived)this;
        }

        public new TDerived SetHeight(uint height)
        {
            base.SetHeight(height);
            return (TDerived)this;
        }

        public new TDerived SetPoint(uint point)
        {
            base.SetPoint(point);
            return (TDerived)this;
        }

        public new TDerived SetPoint(uint startX, uint startY)
        {
            base.SetPoint(startX, startY);
            return (TDerived)this;
        }

        public new TDerived SetRange(uint startX, uint startY, uint length)
        {
            base.SetRange(startX, startY, length);
            return (TDerived)this;
        }

        public new TDerived SetRange(uint startX, uint startY, uint width, uint height)
        {
            base.SetRange(startX, startY, width, height);
            return (TDerived)this;
        }

        public new TDerived SetRange(MatrixRange matrixRange)
        {
            base.SetRange(matrixRange);
            return (TDerived)this;
        }

        /* Clear */

        /// <summary>
        /// 清除外墙 ID（设为 0）。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived ClearOutsideWall()
        {
            this.rogueLikeList.outsideWallId = 0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除内墙 ID（设为 0）。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived ClearInsideWall()
        {
            this.rogueLikeList.insideWallId = 0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除房间 ID（设为 0）。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived ClearRoom()
        {
            this.rogueLikeList.roomId = 0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除入口 ID（设为 0）。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived ClearEntrance()
        {
            this.rogueLikeList.entranceId = 0;
            return (TDerived)this;
        }

        /// <summary>
        /// 清除墙（内外墙都清零）。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived ClearWall()
        {
            ClearInsideWall();
            ClearOutsideWall();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除最大道路数（设为 0）。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived ClearMaxWay()
        {
            this.maxWay = 0;
            return (TDerived)this;
        }

        /// <summary>
        /// 重置 rogueLikeList 为默认实例并清除最大道路数。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived ClearValue()
        {
            this.rogueLikeList = new RogueLikeList();
            ClearMaxWay();
            return (TDerived)this;
        }

        /// <summary>
        /// 清除范围与值（调用 ClearRange 与 ClearValue）。
        /// </summary>
        /// <returns>当前实例（TDerived）。</returns>
        public TDerived Clear()
        {
            this.ClearRange();
            this.ClearValue();
            return (TDerived)this;
        }

        public new TDerived ClearPointX()
        {
            base.ClearPointX();
            return (TDerived)this;
        }

        public new TDerived ClearPointY()
        {
            base.ClearPointY();
            return (TDerived)this;
        }

        public new TDerived ClearPoint()
        {
            base.ClearPoint();
            return (TDerived)this;
        }

        public new TDerived ClearWidth()
        {
            base.ClearWidth();
            return (TDerived)this;
        }

        public new TDerived ClearHeight()
        {
            base.ClearHeight();
            return (TDerived)this;
        }

        public new TDerived ClearLength()
        {
            base.ClearLength();
            return (TDerived)this;
        }

        public new TDerived ClearRange()
        {
            base.ClearRange();
            return (TDerived)this;
        }

        /* Constructors */

        /// <summary>
        /// 默认构造函数，使用默认的 RogueLikeList 与参数。
        /// </summary>
        public RectBaseRogueLike()
        {
        } // = default();

        /// <summary>
        /// 使用矩阵范围构造，转发给基类构造函数。
        /// </summary>
        /// <param name="matrixRange">用于初始化的矩阵范围。</param>
        public RectBaseRogueLike(MatrixRange matrixRange) : base(matrixRange)
        {
        }

        /// <summary>
        /// 使用起点和尺寸构造，转发给基类构造函数。
        /// </summary>
        /// <param name="startX">起点 X。</param>
        /// <param name="startY">起点 Y。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        public RectBaseRogueLike(uint startX, uint startY, uint width, uint height) : base(startX, startY, width,
            height)
        {
        }

        /// <summary>
        /// 使用 RogueLikeList 构造对象。
        /// </summary>
        /// <param name="drawValue">用于初始化的 RogueLikeList。</param>
        public RectBaseRogueLike(RogueLikeList drawValue)
        {
            this.rogueLikeList = drawValue;
        }

        /// <summary>
        /// 使用 RogueLikeList 与最大道路数构造对象。
        /// </summary>
        /// <param name="drawValue">用于初始化的 RogueLikeList。</param>
        /// <param name="maxWay">最大道路数。</param>
        public RectBaseRogueLike(RogueLikeList drawValue, uint maxWay)
        {
            this.rogueLikeList = drawValue;
            this.maxWay = maxWay;
        }

        /// <summary>
        /// 使用 RogueLikeList、最大道路数与房间范围构造对象。
        /// </summary>
        /// <param name="drawValue">用于初始化的 RogueLikeList。</param>
        /// <param name="maxWay">最大道路数。</param>
        /// <param name="roomRange">房间范围。</param>
        public RectBaseRogueLike(RogueLikeList drawValue, uint maxWay, MatrixRange roomRange)
        {
            this.rogueLikeList = drawValue;
            this.maxWay = maxWay;
            this.roomRange = roomRange;
        }

        /// <summary>
        /// 构造（指定 drawValue、最大道路数、房间范围与道路范围）。
        /// </summary>
        /// <param name="drawValue">RogueLikeList。</param>
        /// <param name="maxWay">最大道路数。</param>
        /// <param name="roomRange">房间范围。</param>
        /// <param name="wayRange">道路范围。</param>
        public RectBaseRogueLike(RogueLikeList drawValue, uint maxWay, MatrixRange roomRange, MatrixRange wayRange)
        {
            this.rogueLikeList = drawValue;
            this.maxWay = maxWay;
            this.roomRange = roomRange;
            this.wayRange = wayRange;
        }

        /// <summary>
        /// 使用单独的 ID 值构造 RogueLikeList （outsideWall, insideWall, room, entrance, way）。
        /// </summary>
        /// <param name="outsideWallId">外墙 ID。</param>
        /// <param name="insideWallId">内墙 ID。</param>
        /// <param name="roomId">房间 ID。</param>
        /// <param name="entranceId">入口 ID。</param>
        /// <param name="wayId">道路 ID。</param>
        public RectBaseRogueLike(int outsideWallId, int insideWallId, int roomId, int entranceId, int wayId)
        {
            this.rogueLikeList = new RogueLikeList(outsideWallId, insideWallId, roomId, entranceId, wayId);
        }

        /// <summary>
        /// 使用单独的 ID 值与最大道路数构造对象。
        /// </summary>
        /// <param name="outsideWallId">外墙 ID。</param>
        /// <param name="insideWallId">内墙 ID。</param>
        /// <param name="roomId">房间 ID。</param>
        /// <param name="entranceId">入口 ID。</param>
        /// <param name="wayId">道路 ID。</param>
        /// <param name="maxWay">最大道路数。</param>
        public RectBaseRogueLike(int outsideWallId, int insideWallId, int roomId, int entranceId, int wayId,
            uint maxWay)
        {
            this.rogueLikeList = new RogueLikeList(outsideWallId, insideWallId, roomId, entranceId, wayId);
            this.maxWay = maxWay;
        }


        /// <summary>
        /// 使用单独的 ID 值、最大道路数与房间范围构造对象。
        /// </summary>
        /// <param name="outsideWallId">外墙 ID。</param>
        /// <param name="insideWallId">内墙 ID。</param>
        /// <param name="roomId">房间 ID。</param>
        /// <param name="entranceId">入口 ID。</param>
        /// <param name="wayId">道路 ID。</param>
        /// <param name="maxWay">最大道路数。</param>
        /// <param name="roomRange">房间范围。</param>
        public RectBaseRogueLike(int outsideWallId, int insideWallId, int roomId, int entranceId, int wayId,
            uint maxWay, MatrixRange roomRange)
        {
            this.rogueLikeList = new RogueLikeList(outsideWallId, insideWallId, roomId, entranceId, wayId);
            this.maxWay = maxWay;
            this.roomRange = roomRange;
        }

        /// <summary>
        /// 使用单独的 ID 值、最大道路数、房间范围与道路范围构造对象。
        /// </summary>
        /// <param name="outsideWallId">外墙 ID。</param>
        /// <param name="insideWallId">内墙 ID。</param>
        /// <param name="roomId">房间 ID。</param>
        /// <param name="entranceId">入口 ID。</param>
        /// <param name="wayId">道路 ID。</param>
        /// <param name="maxWay">最大道路数。</param>
        /// <param name="roomRange">房间范围。</param>
        /// <param name="wayRange">道路范围。</param>
        public RectBaseRogueLike(int outsideWallId, int insideWallId, int roomId, int entranceId, int wayId,
            uint maxWay, MatrixRange roomRange, MatrixRange wayRange)
        {
            this.rogueLikeList = new RogueLikeList(outsideWallId, insideWallId, roomId, entranceId, wayId);
            this.maxWay = maxWay;
            this.roomRange = roomRange;
            this.wayRange = wayRange;
        }

        /// <summary>
        /// 使用矩阵范围、RogueLikeList 与最大道路数构造对象（转发到基类用于设置范围）。
        /// </summary>
        /// <param name="matrixRange">矩阵范围。</param>
        /// <param name="drawValue">RogueLikeList。</param>
        /// <param name="maxWay">最大道路数。</param>
        public RectBaseRogueLike(MatrixRange matrixRange, RogueLikeList drawValue, uint maxWay) : base(matrixRange)
        {
            this.rogueLikeList = drawValue;
            this.maxWay = maxWay;
        }

        /// <summary>
        /// 使用矩阵范围、RogueLikeList、最大道路数与房间范围构造对象（转发到基类用于设置范围）。
        /// </summary>
        /// <param name="matrixRange">矩阵范围。</param>
        /// <param name="drawValue">RogueLikeList。</param>
        /// <param name="maxWay">最大道路数。</param>
        /// <param name="roomRange">房间范围。</param>
        public RectBaseRogueLike(MatrixRange matrixRange, RogueLikeList drawValue, uint maxWay, MatrixRange roomRange) :
            base(matrixRange)
        {
            this.rogueLikeList = drawValue;
            this.roomRange = roomRange;
            this.maxWay = maxWay;
        }

        /// <summary>
        /// 使用矩阵范围、RogueLikeList、最大道路数、房间范围与道路范围构造对象（转发到基类用于设置范围）。
        /// </summary>
        /// <param name="matrixRange">矩阵范围。</param>
        /// <param name="drawValue">RogueLikeList。</param>
        /// <param name="maxWay">最大道路数。</param>
        /// <param name="roomRange">房间范围。</param>
        /// <param name="wayRange">道路范围。</param>
        public RectBaseRogueLike(MatrixRange matrixRange, RogueLikeList drawValue, uint maxWay, MatrixRange roomRange,
            MatrixRange wayRange) : base(matrixRange)
        {
            this.rogueLikeList = drawValue;
            this.roomRange = roomRange;
            this.wayRange = wayRange;
            this.maxWay = maxWay;
        }

        /// <summary>
        /// 使用起点/尺寸、RogueLikeList 与最大道路数构造对象（转发到基类用于设置范围）。
        /// </summary>
        /// <param name="startX">起点 X。</param>
        /// <param name="startY">起点 Y。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <param name="drawValue">RogueLikeList。</param>
        /// <param name="maxWay">最大道路数。</param>
        public RectBaseRogueLike(uint startX, uint startY, uint width, uint height, RogueLikeList drawValue,
            uint maxWay) : base(startX, startY, width, height)
        {
            this.rogueLikeList = drawValue;
            this.maxWay = maxWay;
        }

        /// <summary>
        /// 使用起点/尺寸、RogueLikeList、最大道路数与房间范围构造对象（转发到基类用于设置范围）。
        /// </summary>
        /// <param name="startX">起点 X。</param>
        /// <param name="startY">起点 Y。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <param name="drawValue">RogueLikeList。</param>
        /// <param name="maxWay">最大道路数。</param>
        /// <param name="roomRange">房间范围。</param>
        public RectBaseRogueLike(uint startX, uint startY, uint width, uint height, RogueLikeList drawValue,
            uint maxWay, MatrixRange roomRange) : base(startX, startY, width, height)
        {
            this.rogueLikeList = drawValue;
            this.maxWay = maxWay;
            this.roomRange = roomRange;
        }

        /// <summary>
        /// 使用起点/尺寸、RogueLikeList、最大道路数、房间范围与道路范围构造对象（转发到基类用于设置范围）。
        /// </summary>
        /// <param name="startX">起点 X。</param>
        /// <param name="startY">起点 Y。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <param name="drawValue">RogueLikeList。</param>
        /// <param name="maxWay">最大道路数。</param>
        /// <param name="roomRange">房间范围。</param>
        /// <param name="wayRange">道路范围。</param>
        public RectBaseRogueLike(uint startX, uint startY, uint width, uint height, RogueLikeList drawValue,
            uint maxWay, MatrixRange roomRange, MatrixRange wayRange) : base(startX, startY, width, height)
        {
            this.rogueLikeList = drawValue;
            this.maxWay = maxWay;
            this.roomRange = roomRange;
            this.wayRange = wayRange;
        }
    }
}
