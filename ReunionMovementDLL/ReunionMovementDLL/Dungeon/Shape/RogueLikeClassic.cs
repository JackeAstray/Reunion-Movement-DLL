using ReunionMovementDLL.Dungeon.Base;
using ReunionMovementDLL.Dungeon.Random;
using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Util;
using System.Collections.Generic;
using System.Linq;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;


namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// 存储房间或通路位置与尺寸的简单数据结构。
    /// </summary>
    public class RogueLikeOutputNumber
    {
        /// <summary>
        /// 矩形左上角 X 坐标（相对于矩阵起点）。
        /// </summary>
        public int x { get; set; }
        /// <summary>
        /// 矩形左上角 Y 坐标（相对于矩阵起点）。
        /// </summary>
        public int y { get; set; }
        /// <summary>
        /// 矩形宽度。
        /// </summary>
        public int w { get; set; }
        /// <summary>
        /// 矩形高度。
        /// </summary>
        public int h { get; set; }

        /// <summary>
        /// 默认构造函数，创建一个空的输出描述对象。
        /// </summary>
        public RogueLikeOutputNumber()
        {
        }

        /// <summary>
        /// 使用坐标与尺寸初始化输出描述对象。
        /// </summary>
        /// <param name="x">左上角 X 坐标。</param>
        /// <param name="y">左上角 Y 坐标。</param>
        /// <param name="w">宽度。</param>
        /// <param name="h">高度。</param>
        public RogueLikeOutputNumber(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
    }

    /// <summary>
    /// 生成类：基于矩形房间与通路生成类 RogueLike 地牢（经典实现）。
    /// 名称已改为 RogueLikeClassic。
    /// 继承自 RectBaseRogueLike 并实现 IDrawer<int> 接口。
    /// </summary>
    public sealed class RogueLikeClassic : RectBaseRogueLike<RogueLikeClassic>, IDrawer<int>
    {
        /// <summary>
        /// 地牢随机数生成器，用于房间大小、方向等随机选择。
        /// </summary>
        private DungeonRandom rand = new DungeonRandom();

        /// <summary>
        /// 方位枚举：表示北南东西四个方向。
        /// </summary>
        enum Direction : uint
        {
            North,
            South,
            West,
            East,
            Count,
        }

        /// <summary>
        /// 有效的方向数量（四个方向）。
        /// </summary>
        private readonly uint directionCount = 4;

        /// <summary>
        /// 将地牢绘制到目标矩阵。如果起始坐标超出矩阵范围则返回 false。
        /// </summary>
        /// <param name="matrix">目标整型矩阵。</param>
        /// <returns>绘制是否成功。</returns>
        public bool Draw(int[,] matrix)
        {
            return startX >= MatrixUtil.GetX(matrix) || startY >= MatrixUtil.GetY(matrix) ? false : DrawNormal(matrix);
        }

        /// <summary>
        /// 带日志输出的绘制方法（未实现）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="log">输出日志（out）。</param>
        /// <returns>抛出 NotImplementedException 表示未实现。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 在传入矩阵上创建地牢并返回该矩阵引用。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <returns>已填充的矩阵引用。</returns>
        public int[,] Create(int[,] matrix)
        {
            this.Draw(matrix);
            return matrix;
        }

        /// <summary>
        /// 执行主要的地牢生成逻辑：生成初始房间并迭代创建后续房间与通路。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <returns>生成是否成功。</returns>
        bool DrawNormal(int[,] matrix)
        {
            if (this.roomRange.w < 1 || this.roomRange.h < 1 || this.wayRange.w < 1 || this.wayRange.h < 1)
                return false;

            var endX = MatrixUtil.GetX(matrix);
            var endY = MatrixUtil.GetY(matrix);

            var sizeX = endX - this.startX;
            var sizeY = endY - this.startY;

            var roomRect = new List<RogueLikeOutputNumber>();
            var branchPoint = new List<RogueLikeOutputNumber>();
            var isWay = new List<bool>();

            // 生成最初的房间
            if (!MakeRoom(matrix, sizeX, sizeY, roomRect, branchPoint, isWay, (int)sizeX / 2, (int)sizeY / 2,
                (Direction)rand.Next(directionCount))) return false;

            // 迭代生成后续房间与通路
            for (uint i = 1; i < maxWay; ++i)
            {
                if (!CreateNext2(matrix, sizeX, sizeY, roomRect, branchPoint, isWay)) break;
            }

            return true;
        }

        /// <summary>
        /// 尝试从现有分支点随机选择一个并创建下一个房间或通路（有多次尝试计数机制）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="sizeX">矩阵宽度相对于起始 X 后的大小。</param>
        /// <param name="sizeY">矩阵高度相对于起始 Y 后的大小。</param>
        /// <param name="roomRect">已放置的房间列表（输出）。</param>
        /// <param name="branchPoint">可分支点列表（输入/输出）。</param>
        /// <param name="isWay">对应分支点是否为通路的布尔列表（输入/输出）。</param>
        /// <returns>是否成功创建了下一个要素。</returns>
        private bool CreateNext2(int[,] matrix, uint sizeX, uint sizeY, List<RogueLikeOutputNumber> roomRect,
            List<RogueLikeOutputNumber> branchPoint, List<bool> isWay)
        {
            // 多次尝试计数
            for (int i = 0, r = 0; i < 65535; ++i)
            {
                if (!branchPoint.Any()) break;

                r = rand.Next(branchPoint.Count());
                int x = rand.Next(branchPoint[r].x, branchPoint[r].x + branchPoint[r].w);
                int y = rand.Next(branchPoint[r].y, branchPoint[r].y + branchPoint[r].h);

                // 尝试所有方向
                for (int j = 0; j < (int)Direction.Count; ++j)
                {
                    if (!CreateNext(matrix, sizeX, sizeY, roomRect, branchPoint, isWay, isWay[r], x, y,
                        (Direction)j)) continue;
                    branchPoint.RemoveAt(r);
                    isWay.RemoveAt(r);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 在指定分支点和方向上尝试创建房间或通路。
        /// 根据是否为通路分支可能会创建房间或直接延伸通路。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="sizeX">相对宽度。</param>
        /// <param name="sizeY">相对高度。</param>
        /// <param name="roomRect">已放置房间列表。</param>
        /// <param name="branchPoint">分支点列表。</param>
        /// <param name="isWayList">分支点对应的是否为通路的列表。</param>
        /// <param name="isWay">当前分支点是否标记为通路。</param>
        /// <param name="x_">分支点内部随机选取的 X 坐标。</param>
        /// <param name="y_">分支点内部随机选取的 Y 坐标。</param>
        /// <param name="dir_">尝试创建的方向。</param>
        /// <returns>是否成功创建房间或通路。</returns>
        private bool CreateNext(int[,] matrix, uint sizeX, uint sizeY, List<RogueLikeOutputNumber> roomRect,
            List<RogueLikeOutputNumber> branchPoint, List<bool> isWayList, bool isWay, int x_, int y_, Direction dir_)
        {
            int dx = 0;
            int dy = 0;

            switch (dir_)
            {
                case Direction.North:
                    dy = 1;
                    break;
                case Direction.South:
                    dy = -1;
                    break;
                case Direction.West:
                    dx = 1;
                    break;
                case Direction.East:
                    dx = -1;
                    break;
            }

            // 检查边界与现有房间/通路冲突
            if (startX + x_ + dx < 0 || startX + x_ + dx >= sizeX || startY + y_ + dy < 0 ||
                startY + y_ + dy >= sizeY)
            {
                return false;
            }

            // 当前位置必须是房间或通路
            if (matrix[startY + y_ + dy, startX + x_ + dx] != rogueLikeList.roomId &&
                matrix[startY + y_ + dy, startX + x_ + dx] != rogueLikeList.wayId) return false;

            if (!isWay)
            {
                if (!MakeWay(matrix, sizeX, sizeY, branchPoint, isWayList, x_, y_, dir_)) return false;
                if (matrix[startY + y_ + dy, startX + x_ + dx] == rogueLikeList.roomId)
                    matrix[y_, x_] = rogueLikeList.entranceId;
                else matrix[y_, x_] = rogueLikeList.wayId;
                return true;
            }

            // 随机决定是创建房间还是通路
            if (rand.Probability(0.5))
            {
                if (!MakeRoom(matrix, sizeX, sizeY, roomRect, branchPoint, isWayList, x_, y_, dir_)) return false;
                matrix[y_, x_] = rogueLikeList.entranceId;
                return true;
            }

            // 创建通路
            if (!MakeWay(matrix, sizeX, sizeY, branchPoint, isWayList, x_, y_, dir_)) return false;
            if (matrix[startY + y_ + dy, startX + x_ + dx] == rogueLikeList.roomId)
            {
                matrix[y_, x_] = rogueLikeList.entranceId;
            }
            else
            {
                matrix[y_, x_] = rogueLikeList.wayId;
            }

            return true;
        }

        /// <summary>
        /// 在指定位置创建一段通路并将结果写入矩阵，同时向分支点列表中添加新的分支点（如果合适）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="sizeX">相对宽度。</param>
        /// <param name="sizeY">相对高度。</param>
        /// <param name="branchPoint">分支点列表（用于添加新分支）。</param>
        /// <param name="isWay">对应分支点是否为通路列表（用于添加）。</param>
        /// <param name="x_">起点 X。</param>
        /// <param name="y_">起点 Y。</param>
        /// <param name="dir_">通路方向。</param>
        /// <returns>是否成功放置通路。</returns>
        private bool MakeWay(int[,] matrix, uint sizeX, uint sizeY, List<RogueLikeOutputNumber> branchPoint,
            List<bool> isWay,
            int x_, int y_, Direction dir_)
        {
            var way_ = new RogueLikeOutputNumber();
            way_.x = x_;
            way_.y = y_;

            // 左右方向（横向通路）
            if (rand.Probability(0.5))
            {
                way_.w = rand.Next(wayRange.x, wayRange.x + wayRange.w);
                way_.h = 1;
                switch (dir_)
                {
                    case Direction.North:
                        way_.y = y_ - 1;
                        if (rand.Probability(0.5)) way_.x = x_ - way_.w + 1;
                        break;
                    case Direction.South:
                        way_.y = y_ + 1;
                        if (rand.Probability(0.5)) way_.x = x_ - way_.w + 1;
                        break;
                    case Direction.West:
                        way_.x = x_ - way_.w;
                        break;
                    case Direction.East:
                        way_.x = x_ + 1;
                        break;
                }
            }
            // 上下方向（纵向通路）
            else
            {
                way_.w = 1;
                way_.h = rand.Next(wayRange.y, wayRange.y + wayRange.h);

                switch (dir_)
                {
                    case Direction.North:
                        way_.y = y_ - way_.h;
                        break;
                    case Direction.South:
                        way_.y = y_ + 1;
                        break;
                    case Direction.West:
                        way_.x = x_ - 1;
                        if (rand.Probability(0.5)) way_.y = y_ - way_.h + 1;
                        break;
                    case Direction.East:
                        way_.x = x_ + 1;
                        if (rand.Probability(0.5)) way_.y = y_ - way_.h + 1;
                        break;
                }
            }

            if (!PlaceOutputNumber(matrix, sizeX, sizeY, way_, rogueLikeList.wayId)) return false;
            if (dir_ != Direction.South && way_.w != 1)
            {
                branchPoint.Add(new RogueLikeOutputNumber(way_.x, way_.y - 1, way_.w, 1));
                isWay.Add(true);
            }

            if (dir_ != Direction.North && way_.w != 1)
            {
                branchPoint.Add(new RogueLikeOutputNumber(way_.x, way_.y + way_.h, way_.w, 1));
                isWay.Add(true);
            }

            if (dir_ != Direction.East && way_.h != 1)
            {
                branchPoint.Add(new RogueLikeOutputNumber(way_.x - 1, way_.y, 1, way_.h));
                isWay.Add(true);
            }

            if (dir_ != Direction.West && way_.h != 1)
            {
                branchPoint.Add(new RogueLikeOutputNumber(way_.x + way_.w, way_.y, 1, way_.h));
                isWay.Add(true);
            }

            return true;
        }

        /// <summary>
        /// 在指定位置创建一个房间并将其写入矩阵，同时向分支点列表添加新的可扩展分支点。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="sizeX">相对宽度。</param>
        /// <param name="sizeY">相对高度。</param>
        /// <param name="roomRect">已放置房间列表（用于添加）。</param>
        /// <param name="branchPoint">分支点列表（用于添加）。</param>
        /// <param name="isWay">对应分支点是否为通路列表（用于添加）。</param>
        /// <param name="x_">基准 X 坐标。</param>
        /// <param name="y_">基准 Y 坐标。</param>
        /// <param name="dir_">房间相对于基准点的方向。</param>
        /// <param name="firstRoom">是否为最初的房间（影响分支点添加规则）。</param>
        /// <returns>是否成功放置房间。</returns>
        private bool MakeRoom(int[,] matrix, uint sizeX, uint sizeY, List<RogueLikeOutputNumber> roomRect,
            List<RogueLikeOutputNumber> branchPoint, List<bool> isWay, int x_, int y_, Direction dir_,
            bool firstRoom = false)
        {
            var room = new RogueLikeOutputNumber();
            room.w = rand.Next(roomRange.x, roomRange.x + roomRange.w);
            room.h = rand.Next(roomRange.y, roomRange.y + roomRange.h);

            switch (dir_)
            {
                case Direction.North:
                    room.x = x_ - room.w / 2;
                    room.y = y_ - room.h;
                    break;
                case Direction.South:
                    room.x = x_ - room.w / 2;
                    room.y = y_ + 1;
                    break;
                case Direction.West:
                    room.x = x_ - room.w;
                    room.y = y_; // 注释：修复方向为 West 或 East 时基点 Y 偏移的问题（保留原作者的备注）
                    break;
                case Direction.East:
                    room.x = x_ + 1;
                    room.y = y_;
                    break;
            }

            if (PlaceOutputNumber(matrix, sizeX, sizeY, room, rogueLikeList.roomId))
            {
                roomRect.Add(room);
                if (dir_ != Direction.South || firstRoom)
                {
                    branchPoint.Add(new RogueLikeOutputNumber(room.x, room.y - 1, room.w, 1));
                    isWay.Add(false);
                }

                if (dir_ != Direction.North || firstRoom)
                {
                    branchPoint.Add(new RogueLikeOutputNumber(room.x, room.y + room.h, room.w, 1));
                    isWay.Add(false);
                }

                if (dir_ != Direction.East || firstRoom)
                {
                    branchPoint.Add(new RogueLikeOutputNumber(room.x - 1, room.y, 1, room.h));
                    isWay.Add(false);
                }

                if (dir_ != Direction.West || firstRoom)
                {
                    branchPoint.Add(new RogueLikeOutputNumber(room.x + room.w, room.y, 1, room.h));
                    isWay.Add(false);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 将指定的矩形（房间或通路）放置到矩阵上并设置对应的墙与地块编号。
        /// 该方法会检查边界并确保目标区域是可放置的（原始为 outsideWallId）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="sizeX">相对宽度。</param>
        /// <param name="sizeY">相对高度。</param>
        /// <param name="rect">要放置的矩形描述对象。</param>
        /// <param name="tile">用于填充房间内部的瓦片 ID（例如 roomId 或 wayId）。</param>
        /// <returns>是否成功放置。</returns>
        private bool PlaceOutputNumber(int[,] matrix, uint sizeX, uint sizeY, RogueLikeOutputNumber rect, int tile)
        {
            if (rect.x < 1 || rect.y < 1 || rect.x + rect.w > sizeX - 1 || rect.y + rect.h > sizeY - 1)
            {
                return false;
            }

            for (int y = rect.y; y < rect.y + rect.h; ++y)
            {
                for (int x = rect.x; x < rect.x + rect.w; ++x)
                {
                    if (matrix[startY + y, startX + x] != rogueLikeList.outsideWallId)
                    {
                        return false;
                    }
                }
            }

            for (int y = rect.y - 1; y < rect.y + rect.h + 1; ++y)
            {
                for (int x = rect.x - 1; x < rect.x + rect.w + 1; ++x)
                {
                    if (y == rect.y - 1 || x == rect.x - 1 || y == rect.y + rect.h || x == rect.x + rect.w)
                    {
                        matrix[y, x] = rogueLikeList.insideWallId;
                    }
                    else
                    {
                        matrix[y, x] = tile;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 默认构造函数。
        /// </summary>
        public RogueLikeClassic()
        {
        }

        /// <summary>
        /// 使用矩阵范围的构造函数。
        /// </summary>
        /// <param name="matrixRange">矩阵范围对象。</param>
        public RogueLikeClassic(MatrixRange matrixRange) : base(matrixRange)
        {
        }

        /// <summary>
        /// 使用起始坐标与尺寸初始化的构造函数。
        /// </summary>
        public RogueLikeClassic(uint startX, uint startY, uint width, uint height) : base(startX, startY, width, height)
        {
        }

        /// <summary>
        /// 使用绘制值列表初始化的构造函数。
        /// </summary>
        /// <param name="drawValue">包含各类瓦片 ID 的列表。</param>
        public RogueLikeClassic(RogueLikeList drawValue) : base(drawValue)
        {
        }

        /// <summary>
        /// 初始化并指定最大通路数的构造函数。
        /// </summary>
        public RogueLikeClassic(RogueLikeList drawValue, uint maxWay) : base(drawValue, maxWay)
        {
        }

        /// <summary>
        /// 初始化并指定最大通路数与房间范围的构造函数。
        /// </summary>
        public RogueLikeClassic(RogueLikeList drawValue, uint maxWay, MatrixRange roomRange) : base(drawValue, maxWay,
            roomRange)
        {
        }

        /// <summary>
        /// 初始化并指定绘制范围与通路范围的构造函数。
        /// </summary>
        public RogueLikeClassic(RogueLikeList drawValue, uint maxWay, MatrixRange roomRange, MatrixRange wayRange) : base(
            drawValue, maxWay, roomRange, wayRange)
        {
        }

        /// <summary>
        /// 使用各类瓦片 ID 初始化的构造函数。
        /// </summary>
        public RogueLikeClassic(int outsideWallId, int insideWallId, int roomId, int entranceId, int wayId) : base(
            outsideWallId, insideWallId, roomId, entranceId, wayId)
        {
        }

        /// <summary>
        /// 使用各类瓦片 ID 并指定最大通路数的构造函数。
        /// </summary>
        public RogueLikeClassic(int outsideWallId, int insideWallId, int roomId, int entranceId, int wayId, uint maxWay) :
            base(outsideWallId, insideWallId, roomId, entranceId, wayId, maxWay)
        {
        }

        /// <summary>
        /// 使用瓦片 ID、最大通路与房间范围初始化的构造函数。
        /// </summary>
        public RogueLikeClassic(int outsideWallId, int insideWallId, int roomId, int entranceId, int wayId, uint maxWay,
            MatrixRange roomRange) : base(outsideWallId, insideWallId, roomId, entranceId, wayId, maxWay, roomRange)
        {
        }

        /// <summary>
        /// 使用瓦片 ID、最大通路、房间范围与通路范围初始化的构造函数。
        /// </summary>
        public RogueLikeClassic(int outsideWallId, int insideWallId, int roomId, int entranceId, int wayId, uint maxWay,
            MatrixRange roomRange, MatrixRange wayRange) : base(outsideWallId, insideWallId, roomId, entranceId, wayId,
            maxWay, roomRange, wayRange)
        {
        }

        /// <summary>
        /// 使用矩阵范围、绘制列表与最大通路数初始化的构造函数。
        /// </summary>
        public RogueLikeClassic(MatrixRange matrixRange, RogueLikeList drawValue, uint maxWay) : base(matrixRange, drawValue,
            maxWay)
        {
        }

        /// <summary>
        /// 使用矩阵范围、绘制列表、最大通路数与房间范围初始化的构造函数。
        /// </summary>
        public RogueLikeClassic(MatrixRange matrixRange, RogueLikeList drawValue, uint maxWay, MatrixRange roomRange) : base(
            matrixRange, drawValue, maxWay, roomRange)
        {
        }

        /// <summary>
        /// 使用矩阵范围、绘制列表、最大通路数、房间范围与通路范围初始化的构造函数。
        /// </summary>
        public RogueLikeClassic(MatrixRange matrixRange, RogueLikeList drawValue, uint maxWay, MatrixRange roomRange,
            MatrixRange wayRange) : base(matrixRange, drawValue, maxWay, roomRange, wayRange)
        {
        }

        /// <summary>
        /// 使用坐标、尺寸、绘制列表与最大通路数初始化的构造函数。
        /// </summary>
        public RogueLikeClassic(uint startX, uint startY, uint width, uint height, RogueLikeList drawValue,
            uint maxWay) : base(startX, startY, width, height, drawValue, maxWay)
        {
        }

        /// <summary>
        /// 使用坐标、尺寸、绘制列表、最大通路数与房间范围初始化的构造函数。
        /// </summary>
        public RogueLikeClassic(uint startX, uint startY, uint width, uint height, RogueLikeList drawValue,
            uint maxWay, MatrixRange roomRange) : base(startX, startY, width, height, drawValue, maxWay, roomRange)
        {
        }

        /// <summary>
        /// 使用坐标、尺寸、绘制列表、最大通路数、房间范围与通路范围初始化的构造函数。
        /// </summary>
        public RogueLikeClassic(uint startX, uint startY, uint width, uint height, RogueLikeList drawValue,
            uint maxWay, MatrixRange roomRange, MatrixRange wayRange) : base(startX, startY, width, height, drawValue,
            maxWay, roomRange, wayRange)
        {
        }
    }
}