using ReunionMovementDLL.Dungeon.Random;
using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Base;
using System;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;


namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// 简单的RogueLike地图生成器（基于二叉空间划分，BSP）。
    /// 名称已改为 RogueLikeBSP。
    /// </summary>
    public class RogueLikeBSP : RectBaseSimpleRogueLike<RogueLikeBSP>, IDrawer<int>
    {
        /// <summary>
        /// 存放 RogueLike 标识的结构（外墙/内墙/房间/入口/道路 等）。
        /// </summary>
        private RogueLikeList rogueLikeList = new RogueLikeList();

        /// <summary>
        /// 随机数生成器，用于生成分割、房间和道路的随机数。
        /// </summary>
        private RandomBase rand = new RandomBase();

        /// <summary>
        /// 表示按X方向分割的常量标识。
        /// </summary>
        private const int RL_COUNT_X = 0;

        /// <summary>
        /// 表示按Y方向分割的常量标识。
        /// </summary>
        private const int RL_COUNT_Y = 1;

        /// <summary>
        /// 设置绘制所用的 RogueLikeList 并同步 Room/Way 值。
        /// </summary>
        /// <param name="list">RogueLikeList 对象。</param>
        /// <returns>当前实例。</returns>
        public RogueLikeBSP SetValue(RogueLikeList list)
        {
            this.rogueLikeList = list;
            this.roomValue = list.roomId;
            this.roadValue = list.wayId;
            return this;
        }

        // Normal
        /// <summary>
        /// 在指定矩阵及范围内以常规算法生成RogueLike地图（房间 + 道路）。
        /// </summary>
        /// <param name="matrix_">目标矩阵（二维整型数组）。</param>
        /// <param name="endX_">结束X坐标（不包含）。</param>
        /// <param name="endY_">结束Y坐标（不包含）。</param>
        /// <returns>始终返回true，表示绘制完成（当前实现不返回错误信息）。</returns>
        private bool DrawNormal(int[,] matrix_, uint endX_, uint endY_)
        {
            // 防护: 当 divisionRandMax 为 0 时不要调用 rand.Next(0)（会抛出）。
            var mapDivCount = divisionMin + (divisionRandMax > 0 ? rand.Next(divisionRandMax) : 0u);

            var dungeonDivision = new uint[mapDivCount, 4];

            var dungeonRoom = new uint[mapDivCount, 4];

            var dungeonRoad = new uint[mapDivCount, 4];

            dungeonDivision[0, 0] = endY_ - 1;
            dungeonDivision[0, 1] = endX_ - 1;
            dungeonDivision[0, 2] = startX + 1;
            dungeonDivision[0, 3] = startY + 1;

            dungeonRoad[0, 0] = uint.MaxValue;
            dungeonRoad[0, 1] = uint.MaxValue;

            CreateDivision(dungeonRoad, dungeonDivision, mapDivCount);
            CreateRoom(dungeonRoom, dungeonDivision, mapDivCount);
            AssignRoom(dungeonRoom, matrix_, mapDivCount);
            CreateRoad(dungeonRoad, dungeonRoom, dungeonDivision, matrix_, mapDivCount);
            return true;
        }

        /// <summary>
        /// 创建分割（Binary Space Partitioning 风格），生成每个划分的边界并记录分割关系用于后续连通。
        /// </summary>
        /// <param name="dungeonRoad">用于存储分割后道路连接信息的数组（输出/输入）。</param>
        /// <param name="dungeonDivision">用于存储每个划分的边界信息的数组（输出/输入）。</param>
        /// <param name="mapDivCount">划分数量（元素个数）。</param>
        private void CreateDivision(uint[,] dungeonRoad, uint[,] dungeonDivision, uint mapDivCount)
        {
            uint divisionAfter = 0;
            int count = 0; // x = 0, y = 1

            for (int i = 1; i < mapDivCount; ++i)
            {
                divisionAfter = rand.Next((uint)i);

                if (dungeonDivision[divisionAfter, 0] - dungeonDivision[divisionAfter, 2] >
                    dungeonDivision[divisionAfter, 1] - dungeonDivision[divisionAfter, 3])
                {
                    count = RL_COUNT_X;
                }
                else
                {
                    count = RL_COUNT_Y;
                }


                if (dungeonDivision[divisionAfter, count] - dungeonDivision[divisionAfter, count + 2] <
                    divisionRandMax * 2 + 8)
                {
                    uint k = 0;
                    for (int j = 1; j < mapDivCount; ++j)
                    {
                        if (dungeonDivision[j, 0] - dungeonDivision[j, 2] > k)
                        {
                            k = dungeonDivision[j, 0] - dungeonDivision[j, 2];
                            divisionAfter = (uint)j;
                            count = RL_COUNT_X;
                        }

                        if (dungeonDivision[j, 1] - dungeonDivision[j, 3] > k)
                        {
                            k = dungeonDivision[j, 1] - dungeonDivision[j, 3];
                            divisionAfter = (uint)j;
                            count = RL_COUNT_Y;
                        }
                    }
                }

                dungeonRoad[i, 0] = divisionAfter;
                dungeonRoad[i, 1] = (uint)count;

                for (int j = 1; j < i; ++j)
                {
                    if (dungeonRoad[j, 0] == divisionAfter) dungeonRoad[j, 0] = (uint)i;
                }

                // 计算用于随机分割的区间长度并防护 rand.Next 的参数
                uint span = (dungeonDivision[divisionAfter, count] - dungeonDivision[divisionAfter, count + 2]) / 3;
                if (span <= 1)
                {
                    // 如果可用区间过小，退化为靠近起点的一个较小偏移，避免调用 rand.Next(1,1) 抛异常
                    dungeonDivision[i, count] = dungeonDivision[divisionAfter, count + 2] + 1;
                }
                else
                {
                    dungeonDivision[i, count] =
                        dungeonDivision[divisionAfter, count + 2]
                        + span
                        + (uint)rand.Next(1, span);
                }

                dungeonDivision[i, count + 2] = dungeonDivision[divisionAfter, count + 2];
                dungeonDivision[divisionAfter, count + 2] = dungeonDivision[i, count];

                dungeonDivision[i, Math.Abs(count - 1)] = dungeonDivision[divisionAfter, Math.Abs(count - 1)];
                dungeonDivision[i, Math.Abs(count - 1) + 2] = dungeonDivision[divisionAfter, Math.Abs(count - 1) + 2];
            }
        }

        /// <summary>
        /// 在每个划分内生成房间位置与大小，并对溢出/最小尺寸进行修正。
        /// </summary>
        /// <param name="dungeonRoom">输出房间数组（每个元素为房间边界）。</param>
        /// <param name="dungeonDivision">输入划分数组，提供可生成房间的范围。</param>
        /// <param name="mapDivCount">划分数量。</param>
        private void CreateRoom(uint[,] dungeonRoom, uint[,] dungeonDivision, uint mapDivCount)
        {
            for (int i = 0; i < mapDivCount; ++i)
            {
                dungeonRoom[i, 2] = dungeonDivision[i, 2];
                dungeonRoom[i, 3] = dungeonDivision[i, 3];

                dungeonRoom[i, 0] = dungeonDivision[i, 2] + roomMinY + (roomRandMaxX > 0 ? rand.Next(roomRandMaxX) : 0);

                if (dungeonDivision[i, 0] - dungeonDivision[i, 2] < dungeonRoom[i, 0] - dungeonRoom[i, 2] + 5)
                {
                    dungeonRoom[i, 0] = dungeonDivision[i, 0] - 4;
                    if (dungeonDivision[i, 0] - dungeonDivision[i, 2] < dungeonRoom[i, 0] - dungeonRoom[i, 2] + 5)
                    {
                        dungeonRoom[i, 0] = dungeonDivision[i, 2] + 1;
                    }
                }

                dungeonRoom[i, 1] = dungeonDivision[i, 3] + roomMinX + (roomRandMaxY > 0 ? rand.Next(roomRandMaxY) : 0);

                if (dungeonDivision[i, 1] - dungeonDivision[i, 3] < dungeonRoom[i, 1] - dungeonRoom[i, 3] + 5)
                {
                    dungeonRoom[i, 1] = dungeonDivision[i, 1] - 4;
                    if (dungeonDivision[i, 1] - dungeonDivision[i, 3] < dungeonRoom[i, 1] - dungeonRoom[i, 3] + 5)
                    {
                        dungeonRoom[i, 1] = dungeonDivision[i, 3] + 1;
                    }
                }

                if (dungeonRoom[i, 0] - dungeonDivision[i, 2] <= 1 || dungeonRoom[i, 1] - dungeonDivision[i, 3] <= 1)
                {
                    dungeonRoom[i, 0] = dungeonDivision[i, 2] + 1;
                    dungeonRoom[i, 1] = dungeonDivision[i, 3] + 1;
                }


                uint diffY = dungeonDivision[i, 0] - dungeonRoom[i, 0];
                // 修复：当 diffY == 6 时 diffY - 5 == 1，调用 rand.Next(1,1) 会抛异常。
                // 因此将阈值从 <=5 调整为 <=6，确保只有在足够大的区间才调用 rand.Next(min,max)。
                uint l = diffY <= 6 ? 2 : rand.Next(1, diffY - 5) + 2;
                uint diffX = dungeonDivision[i, 1] - dungeonRoom[i, 1];
                uint n = diffX <= 6 ? 2 : rand.Next(1, diffX - 5) + 2;

                dungeonRoom[i, 0] += l;
                dungeonRoom[i, 2] += l;
                dungeonRoom[i, 1] += n;
                dungeonRoom[i, 3] += n;
            }
        }

        /// <summary>
        /// 根据分割关系在房间间创建道路，连接相邻或父子划分的房间。
        /// </summary>
        /// <param name="dungeonRoad">包含分割关系和临时道路数据的数组。</param>
        /// <param name="dungeonRoom">房间数组，提供房间边界信息。</param>
        /// <param name="dungeonDivision">划分数组，提供分割线位置用于铺设道路。</param>
        /// <param name="matrix_">目标绘制矩阵，将道路标记为roadValue。</param>
        /// <param name="mapDivCount">划分数量。</param>
        private void CreateRoad(uint[,] dungeonRoad, uint[,] dungeonRoom, uint[,] dungeonDivision, int[,] matrix_, uint mapDivCount)
        {
            bool useDoor = rogueLikeList.entranceId >= 0;

            for (uint roomBefore = 0, roomAfter = 0; roomBefore < mapDivCount; ++roomBefore)
            {
                roomAfter = dungeonRoad[roomBefore, 0];
                switch (dungeonRoad[roomBefore, 1])
                {
                    case RL_COUNT_X:
                        {
                            uint wBefore = dungeonRoom[roomBefore, 1] - dungeonRoom[roomBefore, 3];
                            dungeonRoad[roomBefore, 2] = wBefore > 2 ? rand.Next(wBefore - 2) + 1 : 0;

                            uint wAfter = dungeonRoom[roomAfter, 1] - dungeonRoom[roomAfter, 3];
                            dungeonRoad[roomBefore, 3] = wAfter > 2 ? rand.Next(wAfter - 2) + 1 : 0;
                        }

                        for (uint j = dungeonRoom[roomBefore, 0]; j < dungeonDivision[roomBefore, 0]; ++j)
                            matrix_[j, dungeonRoad[roomBefore, 2] + dungeonRoom[roomBefore, 3]] = roadValue;

                        for (uint j = dungeonDivision[roomAfter, 2]; j < dungeonRoom[roomAfter, 2]; ++j)
                            matrix_[j, dungeonRoad[roomBefore, 3] + dungeonRoom[roomAfter, 3]] = roadValue;


                        for (uint j = dungeonRoad[roomBefore, 2] + dungeonRoom[roomBefore, 3]; j <= dungeonRoad[roomBefore, 3] + dungeonRoom[roomAfter, 3]; ++j)
                            matrix_[dungeonDivision[roomBefore, 0], j] = roadValue;
                        for (uint j = dungeonRoad[roomBefore, 3] + dungeonRoom[roomAfter, 3]; j <= dungeonRoad[roomBefore, 2] + dungeonRoom[roomBefore, 3]; ++j)
                            matrix_[dungeonDivision[roomBefore, 0], j] = roadValue;

                        if (useDoor)
                        {
                            // 房间底部门前 - 禁止放在角落；如可向内移动一格，否则不放门
                            if (dungeonRoom[roomBefore, 0] > 0)
                            {
                                int doorX = (int)(dungeonRoad[roomBefore, 2] + dungeonRoom[roomBefore, 3]);
                                int roomLeft = (int)dungeonRoom[roomBefore, 3];
                                int roomRight = (int)dungeonRoom[roomBefore, 1] - 1;
                                bool place = true;

                                // 如果门在左角，尝试向右移动一个位置
                                if (doorX == roomLeft)
                                {
                                    if (roomLeft + 1 <= roomRight - 1)
                                        doorX = roomLeft + 1;
                                    else
                                        place = false;
                                }
                                // 如果门在右拐角，试着向左移动一步
                                else if (doorX == roomRight)
                                {
                                    if (roomRight - 1 >= roomLeft + 1)
                                        doorX = roomRight - 1;
                                    else
                                        place = false;
                                }

                                if (place)
                                    matrix_[(int)dungeonRoom[roomBefore, 0] - 1, doorX] = rogueLikeList.entranceId;
                            }

                            // 顶部房间门后 - 禁止放在角落；如可向内移动一格，否则不放门
                            {
                                int doorX = (int)(dungeonRoad[roomBefore, 3] + dungeonRoom[roomAfter, 3]);
                                int roomLeft = (int)dungeonRoom[roomAfter, 3];
                                int roomRight = (int)dungeonRoom[roomAfter, 1] - 1;
                                bool place = true;

                                if (doorX == roomLeft)
                                {
                                    if (roomLeft + 1 <= roomRight - 1)
                                        doorX = roomLeft + 1;
                                    else
                                        place = false;
                                }
                                else if (doorX == roomRight)
                                {
                                    if (roomRight - 1 >= roomLeft + 1)
                                        doorX = roomRight - 1;
                                    else
                                        place = false;
                                }

                                if (place)
                                    matrix_[(int)dungeonRoom[roomAfter, 2], doorX] = rogueLikeList.entranceId;
                            }
                        }
                        break;
                    case RL_COUNT_Y:
                        {
                            uint hBefore = dungeonRoom[roomBefore, 0] - dungeonRoom[roomBefore, 2];
                            dungeonRoad[roomBefore, 2] = hBefore > 2 ? rand.Next(hBefore - 2) + 1 : 0;

                            uint hAfter = dungeonRoom[roomAfter, 0] - dungeonRoom[roomAfter, 2];
                            dungeonRoad[roomBefore, 3] = hAfter > 2 ? rand.Next(hAfter - 2) + 1 : 0;
                        }

                        for (uint j = dungeonRoom[roomBefore, 1]; j < dungeonDivision[roomBefore, 1]; ++j)
                            matrix_[dungeonRoad[roomBefore, 2] + dungeonRoom[roomBefore, 2], j] = roadValue;

                        for (uint j = dungeonDivision[roomAfter, 3]; j < dungeonRoom[roomAfter, 3]; ++j)
                            matrix_[dungeonRoad[roomBefore, 3] + dungeonRoom[roomAfter, 2], j] = roadValue;

                        for (uint j = dungeonRoad[roomBefore, 2] + dungeonRoom[roomBefore, 2]; j <= dungeonRoad[roomBefore, 3] + dungeonRoom[roomAfter, 2]; ++j)
                            matrix_[j, dungeonDivision[roomBefore, 1]] = roadValue;
                        for (uint j = dungeonRoad[roomBefore, 3] + dungeonRoom[roomAfter, 2]; j <= dungeonRoad[roomBefore, 2] + dungeonRoom[roomBefore, 2]; ++j)
                            matrix_[j, dungeonDivision[roomBefore, 1]] = roadValue;

                        if (useDoor)
                        {
                            // 右侧房间门前 - 禁止放在角落；如可向内移动一格，否则不放门
                            if (dungeonRoom[roomBefore, 1] > 0)
                            {
                                int doorY = (int)(dungeonRoad[roomBefore, 2] + dungeonRoom[roomBefore, 2]);
                                int roomTop = (int)dungeonRoom[roomBefore, 2];
                                int roomBottom = (int)dungeonRoom[roomBefore, 0] - 1;
                                int doorX = (int)dungeonRoom[roomBefore, 1] - 1;
                                bool place = true;

                                if (doorY == roomTop)
                                {
                                    if (roomTop + 1 <= roomBottom - 1)
                                        doorY = roomTop + 1;
                                    else
                                        place = false;
                                }
                                else if (doorY == roomBottom)
                                {
                                    if (roomBottom - 1 >= roomTop + 1)
                                        doorY = roomBottom - 1;
                                    else
                                        place = false;
                                }

                                if (place)
                                    matrix_[doorY, doorX] = rogueLikeList.entranceId;
                            }
                            // 房间左侧门后 - 禁止放在角落；如可向内移动一格，否则不放门
                            {
                                int doorY = (int)(dungeonRoad[roomBefore, 3] + dungeonRoom[roomAfter, 2]);
                                int roomTop = (int)dungeonRoom[roomAfter, 2];
                                int roomBottom = (int)dungeonRoom[roomAfter, 0] - 1;
                                int doorX = (int)dungeonRoom[roomAfter, 3];
                                bool place = true;

                                if (doorY == roomTop)
                                {
                                    if (roomTop + 1 <= roomBottom - 1)
                                        doorY = roomTop + 1;
                                    else
                                        place = false;
                                }
                                else if (doorY == roomBottom)
                                {
                                    if (roomBottom - 1 >= roomTop + 1)
                                        doorY = roomBottom - 1;
                                    else
                                        place = false;
                                }

                                if (place)
                                    matrix_[doorY, doorX] = rogueLikeList.entranceId;
                            }
                        }
                        break;

                }
            }
        }

        /// <summary>
        /// 将生成的房间填充到目标矩阵中，将房间区域设置为roomValue。
        /// </summary>
        /// <param name="dungeonRoom">房间数组，包含每个房间的边界信息。</param>
        /// <param name="matrix_">目标矩阵。</param>
        /// <param name="mapDivCount">划分/房间数量。</param>
        private void AssignRoom(uint[,] dungeonRoom, int[,] matrix_, uint mapDivCount)
        {
            int wallId = rogueLikeList.insideWallId;
            bool useWall = wallId >= 0;

            for (uint i = 0; i < mapDivCount; ++i)
                for (uint j = dungeonRoom[i, 2]; j < dungeonRoom[i, 0]; ++j)
                    for (uint k = dungeonRoom[i, 3]; k < dungeonRoom[i, 1]; ++k)
                    {
                        if (useWall && (j == dungeonRoom[i, 2] || j == dungeonRoom[i, 0] - 1 ||
                                        k == dungeonRoom[i, 3] || k == dungeonRoom[i, 1] - 1))
                        {
                            matrix_[j, k] = wallId;
                        }
                        else
                        {
                            matrix_[j, k] = roomValue;
                        }
                    }
        }

        /// <summary>
        /// 在矩阵上绘制地图（入口方法），会计算有效的结束坐标并调用DrawNormal。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <returns>返回DrawNormal的结果（通常为true）。</returns>
        public bool Draw(int[,] matrix)
        {
            return DrawNormal(
                matrix,
                (width == 0 || startX + width >= (matrix.Length == 0 ? 0 : (uint)(matrix.Length / matrix.GetLength(0)))) ? (uint)(matrix.Length / matrix.GetLength(0)) : startX + width,
                (height == 0 || startY + height >= matrix.GetLength(0)) ? (uint)(matrix.Length == 0 ? 0 : matrix.GetLength(0)) : startY + height);
        }

        /// <summary>
        /// 绘制并返回日志（未实现）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="log">输出日志字符串（未实现）。param>
        /// <returns>当前实现抛出NotImplementedException。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 在矩阵上生成地图并返回矩阵引用（调用Draw）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <returns>返回被修改的矩阵引用。</returns>
        public int[,] Create(int[,] matrix)
        {
            this.Draw(matrix);
            return matrix;
        }

        /* Constructors */
        /// <summary>
        /// 默认构造函数，使用基类默认参数初始化。
        /// </summary>
        public RogueLikeBSP() { }

        /// <summary>
        /// 使用指定房间值的构造函数。
        /// </summary>
        /// <param name="roomValue">房间在矩阵中使用的值。</param>
        public RogueLikeBSP(int roomValue) : base(roomValue) { }

        /// <summary>
        /// 使用指定房间值和道路值的构造函数。
        /// </summary>
        /// <param name="roomValue">房间在矩阵中使用的值。</param>
        /// <param name="roadValue">道路在矩阵中使用的值。</param>
        public RogueLikeBSP(int roomValue, int roadValue) : base(roomValue, roadValue) { }

        /// <summary>
        /// 完整参数的构造函数，允许自定义划分和房间尺寸参数。
        /// </summary>
        /// <param name="roomValue">房间值。</param>
        /// <param name="roadValue">道路值。</param>
        /// <param name="divisionMin">最小划分数量。</param>
        /// <param name="divisionRandMax">划分数量的随机上限。</param>
        /// <param name="roomMinX">房间最小X尺寸/边距。</param>
        /// <param name="roomRandMaxX">房间X方向随机尺寸上限。</param>
        /// <param name="roomMinY">房间最小Y尺寸/边距。</param>
        /// <param name="roomRandMaxY">房间Y方向随机尺寸上限。</param>
        public RogueLikeBSP(int roomValue, int roadValue, uint divisionMin,
            uint divisionRandMax, uint roomMinX, uint roomRandMaxX, uint roomMinY, uint roomRandMaxY) : base(roomValue, roadValue, divisionMin, divisionRandMax, roomMinX, roomRandMaxX, roomMinY, roomRandMaxY) { }

        /// <summary>
        /// 使用给定矩阵范围的构造函数（仅设置绘制范围）。
        /// </summary>
        /// <param name="matrixRange">矩阵范围（x,y,w,h）。</param>
        public RogueLikeBSP(MatrixRange matrixRange) : base(matrixRange) { }

        /// <summary>
        /// 使用矩阵范围和房间值的构造函数。
        /// </summary>
        /// <param name="matrixRange">矩阵范围。</param>
        /// <param name="roomValue">房间值。</param>
        public RogueLikeBSP(MatrixRange matrixRange, int roomValue) : base(matrixRange, roomValue) { }

        /// <summary>
        /// 使用矩阵范围、房间值和道路值的构造函数。
        /// </summary>
        /// <param name="matrixRange">矩阵范围。</param>
        /// <param name="roomValue">房间值。</param>
        /// <param name="roadValue">道路值。</param>
        public RogueLikeBSP(MatrixRange matrixRange, int roomValue, int roadValue) : base(matrixRange, roomValue, roadValue) { }

        /// <summary>
        /// 使用完整参数（范围、房间/道路值、划分与房间尺寸控制）构造。
        /// </summary>
        /// <param name="matrixRange">矩阵范围。</param>
        /// <param name="roomValue">房间值。</param>
        /// <param name="roadValue">道路值。</param>
        /// <param name="divisionMin">最小划分数量。</param>
        /// <param name="divisionRandMax">划分随机上限。</param>
        /// <param name="roomMinX">房间最小X尺寸。</param>
        /// <param name="roomRandMaxX">房间X方向随机尺寸上限。</param>
        /// <param name="roomMinY">房间最小Y尺寸。</param>
        /// <param name="roomRandMaxY">房间Y方向随机尺寸上限。</param>
        public RogueLikeBSP(MatrixRange matrixRange, int roomValue, int roadValue, uint divisionMin,
            uint divisionRandMax, uint roomMinX, uint roomRandMaxX, uint roomMinY, uint roomRandMaxY)
            : base(matrixRange, roomValue, roadValue, divisionMin, divisionRandMax, roomMinX, roomRandMaxX, roomMinY, roomRandMaxY) { }
    }
}