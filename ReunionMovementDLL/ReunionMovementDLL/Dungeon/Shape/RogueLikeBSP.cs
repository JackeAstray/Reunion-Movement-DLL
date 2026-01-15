using ReunionMovementDLL.Dungeon.Random;
using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Base;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// 设置随机数种子。
        /// </summary>
        /// <param name="seed">种子值。</param>
        /// <returns>当前实例。</returns>
        public RogueLikeBSP SetSeed(uint seed)
        {
            this.rand = new RandomBase(new ReunionMovementDLL.Dungeon.Random.XorShift128(seed));
            return this;
        }

        /// <summary>
        /// 设置自定义随机数生成器。
        /// </summary>
        /// <param name="random">随机数生成器。</param>
        /// <returns>当前实例。</returns>
        public RogueLikeBSP SetRandom(RandomBase random)
        {
            this.rand = random ?? new RandomBase();
            return this;
        }

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
        /// <returns>始终返回true，表示绘制完成。</returns>
        private bool DrawNormal(int[,] matrix_, uint endX_, uint endY_)
        {
            int w = (int)(endX_ - startX);
            int h = (int)(endY_ - startY);

            // 基础尺寸检查
            if (w < roomMinX || h < roomMinY) return true;

            // 计算目标房间数
            int targetRooms = (int)divisionMin;
            if (divisionRandMax > 0) targetRooms += (int)rand.Next(divisionRandMax);
            if (targetRooms < 1) targetRooms = 1;

            BspNode root = new BspNode((int)startX, (int)startY, w, h);
            List<BspNode> leaves = new List<BspNode> { root };

            // 1. BSP 分割
            while (leaves.Count < targetRooms)
            {
                // 优先分割大节点
                var candidate = leaves.OrderByDescending(n => n.W * n.H).FirstOrDefault(n => CanSplit(n));
                if (candidate == null) break;

                leaves.Remove(candidate);
                Split(candidate);
                leaves.Add(candidate.Left);
                leaves.Add(candidate.Right);
            }

            // 2. 生成房间
            foreach (var leaf in leaves) CreateRoomInNode(leaf);

            // 3. 绘制并记录房间
            FillRooms(matrix_, leaves);

            // 4. 生成走廊连接
            ConnectNodes(root, matrix_);

            // 5. 生成入口
            if (rogueLikeList.entranceId >= 0) GenerateEntrances(matrix_, leaves);

            return true;
        }

        private bool CanSplit(BspNode node)
        {
            int minW = (int)roomMinX + 2;
            int minH = (int)roomMinY + 2;
            return node.W >= minW * 2 || node.H >= minH * 2;
        }

        private void Split(BspNode node)
        {
            int minW = (int)roomMinX + 2;
            int minH = (int)roomMinY + 2;

            bool canSplitX = node.W >= minW * 2;
            bool canSplitY = node.H >= minH * 2;
            bool splitH;

            if (canSplitX && canSplitY)
            {
                if (node.W > node.H * 1.5) splitH = false;
                else if (node.H > node.W * 1.5) splitH = true;
                else splitH = rand.Next(2u) == 0;
            }
            else splitH = !canSplitX;

            if (splitH)
            {
                int splitSize = (int)rand.Next((uint)minH, (uint)(node.H - minH + 1));
                node.Left = new BspNode(node.X, node.Y, node.W, splitSize);
                node.Right = new BspNode(node.X, node.Y + splitSize, node.W, node.H - splitSize);
            }
            else
            {
                int splitSize = (int)rand.Next((uint)minW, (uint)(node.W - minW + 1));
                node.Left = new BspNode(node.X, node.Y, splitSize, node.H);
                node.Right = new BspNode(node.X + splitSize, node.Y, node.W - splitSize, node.H);
            }

            node.Left.Parent = node;
            node.Right.Parent = node;
        }

        private void CreateRoomInNode(BspNode node)
        {
            int w = (int)roomMinX + (roomRandMaxX > 0 ? (int)rand.Next(roomRandMaxX) : 0);
            int h = (int)roomMinY + (roomRandMaxY > 0 ? (int)rand.Next(roomRandMaxY) : 0);

            if (w > node.W - 2) w = node.W - 2;
            if (h > node.H - 2) h = node.H - 2;

            if (w < 1 || h < 1) return;

            int x = node.X + 1 + (int)rand.Next((uint)(node.W - w - 1));
            int y = node.Y + 1 + (int)rand.Next((uint)(node.H - h - 1));

            node.RoomX = x; node.RoomY = y; node.RoomW = w; node.RoomH = h;
        }

        private void FillRooms(int[,] matrix, List<BspNode> leaves)
        {
            int wallId = rogueLikeList.insideWallId >= 0 ? rogueLikeList.insideWallId : rogueLikeList.outsideWallId;
            bool useWall = wallId >= 0;

            foreach (var leaf in leaves)
            {
                if (!leaf.HasRoom) continue;

                for (int y = leaf.RoomY; y < leaf.RoomY + leaf.RoomH; ++y)
                {
                    for (int x = leaf.RoomX; x < leaf.RoomX + leaf.RoomW; ++x)
                    {
                        if (y < 0 || y >= matrix.GetLength(0) || x < 0 || x >= matrix.GetLength(1)) continue;

                        bool isBorder = x == leaf.RoomX || x == leaf.RoomX + leaf.RoomW - 1 ||
                                        y == leaf.RoomY || y == leaf.RoomY + leaf.RoomH - 1;
                        
                         matrix[y, x] = (useWall && isBorder) ? wallId : roomValue;
                    }
                }
            }
        }

        private void ConnectNodes(BspNode node, int[,] matrix)
        {
            if (node.IsLeaf) return;
            
            ConnectNodes(node.Left, matrix);
            ConnectNodes(node.Right, matrix);

            var leaf1 = GetRandomLeaf(node.Left);
            var leaf2 = GetRandomLeaf(node.Right);

            var conn = GetSafeConnection(leaf1, leaf2);
            DrawCorridor(matrix, conn.p1.x, conn.p1.y, conn.p2.x, conn.p2.y, conn.hFirst);
        }

        private ((int x, int y) p1, (int x, int y) p2, bool hFirst) GetSafeConnection(BspNode n1, BspNode n2)
        {
            var c1 = GetLeafCenter(n1);
            var c2 = GetLeafCenter(n2);

            if (!n1.HasRoom || !n2.HasRoom) return (c1, c2, rand.Next(2u) == 0);

            // Helpers
            List<int> GetInner(int start, int size)
            {
                var list = new List<int>();
                if (size <= 2) { for (int i = 0; i < size; i++) list.Add(start + i); }
                else { for (int i = 1; i < size - 1; i++) list.Add(start + i); }
                return list;
            }

            int PickBest(List<int> candidates, int preferred, HashSet<int> avoid)
            {
                var good = candidates.Where(v => !avoid.Contains(v)).ToList();
                if (good.Count == 0) return -1;
                return good.OrderBy(v => Math.Abs(v - preferred)).First();
            }

            var n1InnerY = GetInner(n1.RoomY, n1.RoomH);
            var n2InnerY = GetInner(n2.RoomY, n2.RoomH);
            var n1InnerX = GetInner(n1.RoomX, n1.RoomW);
            var n2InnerX = GetInner(n2.RoomX, n2.RoomW);

            var n1WallsY = new HashSet<int> { n1.RoomY, n1.RoomY + n1.RoomH - 1 };
            var n2WallsY = new HashSet<int> { n2.RoomY, n2.RoomY + n2.RoomH - 1 };
            var n1WallsX = new HashSet<int> { n1.RoomX, n1.RoomX + n1.RoomW - 1 };
            var n2WallsX = new HashSet<int> { n2.RoomX, n2.RoomX + n2.RoomW - 1 };

            // H-First: p1.y (from n1) drives horizontal path; p2.x (from n2) drives vertical path.
            // Check collisions with OPPOSITE walls.
            int hy = PickBest(n1InnerY, c1.y, n2WallsY);
            int hx = PickBest(n2InnerX, c2.x, n1WallsX);
            bool hPossible = (hy != -1 && hx != -1);

            // V-First: p1.x (from n1) drives vertical path; p2.y (from n2) drives horizontal path.
            int vx = PickBest(n1InnerX, c1.x, n2WallsX);
            int vy = PickBest(n2InnerY, c2.y, n1WallsY);
            bool vPossible = (vx != -1 && vy != -1);

            if (hPossible && !vPossible) return ((c1.x, hy), (hx, c2.y), true);
            if (!hPossible && vPossible) return ((vx, c1.y), (c2.x, vy), false);
            if (hPossible && vPossible)
            {
                if (rand.Next(2u) == 0) return ((c1.x, hy), (hx, c2.y), true);
                else return ((vx, c1.y), (c2.x, vy), false);
            }

            return (c1, c2, rand.Next(2u) == 0);
        }

        private BspNode GetRandomLeaf(BspNode node)
        {
            if (node.IsLeaf) return node;
            return rand.Next(2u) == 0 ? GetRandomLeaf(node.Left) : GetRandomLeaf(node.Right);
        }

        private (int x, int y) GetLeafCenter(BspNode node)
        {
            if (node.IsLeaf)
            {
                if (node.HasRoom)
                    return (node.RoomX + node.RoomW / 2, node.RoomY + node.RoomH / 2);
                else
                    return (node.X + node.W / 2, node.Y + node.H / 2);
            }
            // Fallback if called on non-leaf (should not happen with current usage)
            return GetLeafCenter(GetRandomLeaf(node));
        }

        private void DrawCorridor(int[,] matrix, int x1, int y1, int x2, int y2, bool? preferHFirst = null)
        {
            int wallId = rogueLikeList.insideWallId >= 0 ? rogueLikeList.insideWallId : rogueLikeList.outsideWallId;
            bool useWall = wallId >= 0;

            int midX, midY;
            bool hFirst = preferHFirst ?? (rand.Next(2u) == 0);

            if (hFirst) { midX = x2; midY = y1; }
            else { midX = x1; midY = y2; }

            DrawLine(matrix, x1, y1, midX, midY, wallId, useWall);
            DrawLine(matrix, midX, midY, x2, y2, wallId, useWall);
        }

        private void DrawLine(int[,] matrix, int x1, int y1, int x2, int y2, int wallId, bool useWall)
        {
            int dx = Math.Sign(x2 - x1);
            int dy = Math.Sign(y2 - y1);
            int x = x1, y = y1;

            while (x != x2 || y != y2)
            {
                DrawRoadCell(matrix, x, y, wallId, useWall);
                if (x != x2) x += dx;
                if (y != y2) y += dy;
            }
            DrawRoadCell(matrix, x2, y2, wallId, useWall);
        }

        private void DrawRoadCell(int[,] matrix, int x, int y, int wallId, bool useWall)
        {
            int rows = matrix.GetLength(0), cols = matrix.GetLength(1);
            if (x < 0 || x >= cols || y < 0 || y >= rows) return;

            matrix[y, x] = roadValue;

            if (useWall)
            {
                // 检查8个方向以确保拐角处没有缺口
                int[] dx = { 0, 0, -1, 1, -1, 1, -1, 1 };
                int[] dy = { -1, 1, 0, 0, -1, -1, 1, 1 };
                for (int i = 0; i < 8; ++i)
                {
                    int nx = x + dx[i], ny = y + dy[i];
                    if (nx >= 0 && nx < cols && ny >= 0 && ny < rows)
                    {
                        int val = matrix[ny, nx];
                        if (val != roadValue && val != roomValue && val != rogueLikeList.entranceId && val != wallId)
                            matrix[ny, nx] = wallId;
                    }
                }
            }
        }

        private void GenerateEntrances(int[,] matrix, List<BspNode> leaves)
        {
            foreach (var leaf in leaves)
            {
                if (!leaf.HasRoom) continue;
                for (int x = leaf.RoomX; x < leaf.RoomX + leaf.RoomW; ++x)
                {
                    CheckEntrance(matrix, x, leaf.RoomY);
                    CheckEntrance(matrix, x, leaf.RoomY + leaf.RoomH - 1);
                }
                for (int y = leaf.RoomY; y < leaf.RoomY + leaf.RoomH; ++y)
                {
                    CheckEntrance(matrix, leaf.RoomX, y);
                    CheckEntrance(matrix, leaf.RoomX + leaf.RoomW - 1, y);
                }
            }
        }

        private void CheckEntrance(int[,] matrix, int x, int y)
        {
            if (x < 0 || x >= matrix.GetLength(1) || y < 0 || y >= matrix.GetLength(0)) return;
            if (matrix[y, x] == roadValue) matrix[y, x] = rogueLikeList.entranceId;
        }

        private class BspNode
        {
            public int X, Y, W, H;
            public BspNode Left, Right, Parent;
            public int RoomX, RoomY, RoomW, RoomH;
            public bool IsLeaf => Left == null && Right == null;
            public bool HasRoom => RoomW > 0 && RoomH > 0;
            public BspNode(int x, int y, int w, int h) { X = x; Y = y; W = w; H = h; }
        }

        /// <summary>
        /// 在矩阵上绘制地图（入口方法），会计算有效的结束坐标并调用DrawNormal。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <returns>返回DrawNormal的结果（通常为true）。returns>
        public bool Draw(int[,] matrix)
        {
            return DrawNormal(
                matrix,
                (width == 0 || startX + width >= (matrix.Length == 0 ? 0 : (uint)(matrix.Length / matrix.GetLength(0)))) ? (uint)(matrix.Length / matrix.GetLength(0)) : startX + width,
                (height == 0 || startY + height >= matrix.GetLength(0)) ? (uint)(matrix.Length == 0 ? 0 : matrix.GetLength(0)) : startY + height);
        }

        /// <summary>
        /// 绘制并返回日志（简单实现）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="log">输出日志字符串。</param>
        /// <returns>返回Draw的结果。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            log = "RogueLikeBSP Draw executed.";
            return Draw(matrix);
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