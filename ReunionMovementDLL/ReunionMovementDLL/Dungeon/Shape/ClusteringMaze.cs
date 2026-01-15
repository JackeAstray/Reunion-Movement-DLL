using ReunionMovementDLL.Dungeon.Random;
using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Util;
using System.Collections.Generic;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// 基于聚类（合并集）算法生成迷宫的绘制器。
    /// 使用并查集（Union-Find）在网格上创建通道，最终生成连通的迷宫结构。
    /// </summary>
    public class ClusteringMaze : RectBaseWithValue<ClusteringMaze>, IDrawer<int>
    {
        private RandomBase rand = new RandomBase();

        // 可配置的出口数量：
        // 0 表示使用默认随机策略（2-10 个）；大于等于 1 表示固定出口数量（会被限制在候选数量范围内）
        public int ExitCount { get; set; } = 0;

        /// <summary>
        /// 四个方向的枚举值，表示上、右、下、左。
        /// </summary>
        private enum Direction
        {
            UP_DIR = 0,
            RIGHT_DIR,
            DOWN_DIR,
            LEFT_DIR
        }

        /// <summary>
        /// 根据方向返回x方向的增量（列变化）。
        /// 例如：向右返回1，向左返回-1，上/下返回0。
        /// </summary>
        /// <param name="dir">方向枚举。</param>
        /// <returns>x方向的位移（-1、0或1）。</returns>
        int dirDx(Direction dir)
        {
            switch (dir)
            {
                case Direction.UP_DIR:
                case Direction.DOWN_DIR: return 0;
                case Direction.RIGHT_DIR: return 1;
                case Direction.LEFT_DIR: return -1;
            }
            return 0;
        }

        /// <summary>
        /// 根据方向返回y方向的增量（行变化）。
        /// 例如：向下返回1，向上返回-1，左/右返回0。
        /// </summary>
        /// <param name="dir">方向枚举。</param>
        /// <returns>y方向的位移（-1、0或1）。</returns>
        int dirDy(Direction dir)
        {
            switch (dir)
            {
                case Direction.RIGHT_DIR:
                case Direction.LEFT_DIR: return 0;
                case Direction.UP_DIR: return -1;
                case Direction.DOWN_DIR: return 1;
            }
            return 0;
        }

        /// <summary>
        /// 并查集的查找（带路径压缩）：返回元素x的根节点索引。
        /// </summary>
        /// <param name="data">并查集的父指针数组。</param>
        /// <param name="x">要查找的元素索引。</param>
        /// <returns>元素x的根索引。</returns>
        private uint root(uint[] data, uint x)
        {
            return data[x] == x ? x : data[x] = this.root(data, data[x]);
        }

        /// <summary>
        /// 判断两个元素是否属于同一个集合（根是否相同）。
        /// </summary>
        /// <param name="data">并查集的父指针数组。</param>
        /// <param name="x">第一个元素索引。</param>
        /// <param name="y">第二个元素索引。</param>
        /// <returns>如果两者属于同一集合则返回true。</returns>
        private bool same(uint[] data, uint x, uint y)
        {
            return this.root(data, x) == this.root(data, y);
        }

        /// <summary>
        /// 将两个集合合并（按秩合并策略）。
        /// </summary>
        /// <param name="data">并查集的父指针数组。</param>
        /// <param name="rank">用于按秩合并的秩数组。</param>
        /// <param name="x">第一个元素索引。</param>
        /// <param name="y">第二个元素索引。</param>
        private void unite(uint[] data, uint[] rank, uint x, uint y)
        {
            x = this.root(data, x);
            y = this.root(data, y);

            if (rank[x] < rank[y]) data[x] = y;
            else
            {
                data[y] = x;
                if (rank[x] == rank[y]) ++rank[x];
            }
        }

        private struct Edge
        {
            public uint x;
            public uint y;
            public Direction dir;
            public Edge(uint x, uint y, Direction dir)
            {
                this.x = x;
                this.y = y;
                this.dir = dir;
            }
        }

        /// <summary>
        /// 在给定矩阵区域内生成聚类迷宫：先在网格上设置初始的墙体点，然后使用并查集随机合并并破墙形成通路。
        /// 返回true表示绘制成功。
        /// </summary>
        /// <param name="matrix">要绘制的二维整数矩阵。</param>
        /// <returns>表示绘制是否成功的布尔值。</returns>
        public bool Draw(int[,] matrix)
        {
            return DrawNormal(matrix);
        }

        /// <summary>
        /// 带日志输出的Draw重载（当前未实现）。
        /// </summary>
        /// <param name="matrix">要绘制的矩阵。</param>
        /// <param name="log">输出日志字符串（未实现）。</param>
        /// <returns>抛出System.NotImplementedException。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 实际绘制逻辑实现：根据给定的矩形范围计算可用区域，创建缩小网格并应用并查集来破墙生成迷宫。
        /// 该实现会在缩小网格的每个单元中心设置墙体，然后随机选择不同集合的邻居并破墙直至所有单元连通。
        /// </summary>
        /// <param name="matrix">要绘制的二维整数矩阵。</param>
        /// <returns>表示绘制是否成功的布尔值。</returns>
        private bool DrawNormal(int[,] matrix)
        {
            var width = this.CalcEndX(MatrixUtil.GetX(matrix) - this.startX);
            var height = this.CalcEndY(MatrixUtil.GetY(matrix) - this.startY);

            // Determine wall value (default 0, unless drawValue is 0 then 1)
            // 确定墙壁值（默认为0，如果绘制值为0则为1），用于初始化背景
            int wallValue = this.drawValue == 0 ? 1 : 0;

            // Fill the area with walls first to ensure a proper maze background
            // 首先用墙壁填充区域，确保迷宫背景正确，避免出现“多入口”或不连通的视觉假象
            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    matrix[this.startY + y, this.startX + x] = wallValue;
                }
            }

            var width3 = width % 2 == 0 ? width - 1 : width;
            var height3 = height % 2 == 0 ? height - 1 : height;

            for (var i = 0; i < width3 / 2; ++i)
            {
                for (var j = 0; j < height3 / 2; ++j)
                {
                    matrix[this.startY + (2 * j + 1), this.startX + (2 * i + 1)] = this.drawValue;
                }
            }

            var mWidth = width3 / 2;
            var mHeight = height3 / 2;
            var mSize = mWidth * mHeight;

            // Union Find, Collectionを使わずに素の配列で実装する。
            var data = new uint[mSize];
            var rank = new uint[mSize];
            for (uint i = 0; i < mSize; ++i) data[i] = i; // 自分の親は自分

            var edges = new List<Edge>();
            for (uint y = 0; y < mHeight; ++y)
            {
                for (uint x = 0; x < mWidth; ++x)
                {
                    if (x + 1 < mWidth) edges.Add(new Edge(x, y, Direction.RIGHT_DIR));
                    if (y + 1 < mHeight) edges.Add(new Edge(x, y, Direction.DOWN_DIR));
                }
            }

            ArrayUtil.Shuffle(edges, rand);

            foreach (var e in edges)
            {
                var cell1 = e.y * mWidth + e.x;
                var cell2X = (uint)((int)e.x + dirDx(e.dir));
                var cell2Y = (uint)((int)e.y + dirDy(e.dir));
                var cell2 = cell2Y * mWidth + cell2X;

                if (!this.same(data, cell1, cell2))
                {
                    this.unite(data, rank, cell1, cell2);
                    matrix[this.startY + 2 * e.y + 1 + dirDy(e.dir), this.startX + 2 * e.x + 1 + dirDx(e.dir)] = this.drawValue;
                }
            }

            // 生成2-10个随机出入口，确保贯通
            var candidates = new List<(int x, int y, Direction dir)>();

            // Top (y=0) and Bottom (y=height-1) - Valid x are odd columns
            for (var i = 0; i < mWidth; ++i)
            {
                int x = 2 * i + 1;
                candidates.Add((x, 0, Direction.UP_DIR));
                candidates.Add((x, (int)height - 1, Direction.DOWN_DIR));
            }

            // Left (x=0) and Right (x=width-1) - Valid y are odd rows
            for (var j = 0; j < mHeight; ++j)
            {
                int y = 2 * j + 1;
                candidates.Add((0, y, Direction.LEFT_DIR));
                candidates.Add(((int)width - 1, y, Direction.RIGHT_DIR));
            }

            // Shuffle candidates
            int n = candidates.Count;
            while (n > 1)
            {
                n--;
                int k = (int)rand.Next((uint)(n + 1));
                var val = candidates[k];
                candidates[k] = candidates[n];
                candidates[n] = val;
            }

            // Select 2 to 10 entrances
            int entranceCount;
            if (this.ExitCount >= 2 && this.ExitCount <= 10)
            {
                entranceCount = this.ExitCount;
            }
            else
            {
                entranceCount = (int)rand.Next(2, 11);
            }
            if (entranceCount > candidates.Count) entranceCount = candidates.Count;

            for (int k = 0; k < entranceCount; ++k)
            {
                var (cx, cy, dir) = candidates[k];
                if (dir == Direction.UP_DIR || dir == Direction.LEFT_DIR)
                {
                    matrix[this.startY + cy, this.startX + cx] = this.drawValue;
                }
                else if (dir == Direction.DOWN_DIR)
                {
                    // Drill from bottom up to the effective maze boundary (height3 - 1)
                    for (int y = cy; y >= (int)height3 - 1; --y)
                        matrix[this.startY + y, this.startX + cx] = this.drawValue;
                }
                else if (dir == Direction.RIGHT_DIR)
                {
                    // Drill from right left to the effective maze boundary (width3 - 1)
                    for (int x = cx; x >= (int)width3 - 1; --x)
                        matrix[this.startY + cy, this.startX + x] = this.drawValue;
                }
            }

            return true;
        }

        /// <summary>
        /// 默认构造函数，创建一个ClusteringMaze实例。
        /// </summary>
        public ClusteringMaze()
        {
        } // = default();

        /// <summary>
        /// 使用绘制值和矩阵范围构造ClusteringMaze实例。
        /// </summary>
        /// <param name="drawValue">用于绘制墙/通路的值。</param>
        /// <param name="matrixRange">指定矩阵范围的MatrixRange对象。</param>
        public ClusteringMaze(int drawValue, MatrixRange matrixRange) : base(drawValue, matrixRange)
        {
        }

        /// <summary>
        /// 使用绘制值和矩形参数构造ClusteringMaze实例。
        /// </summary>
        /// <param name="drawValue">用于绘制墙/通路的值。</param>
        /// <param name="startX">起始X（列）索引。</param>
        /// <param name="startY">起始Y（行）索引。</param>
        /// <param name="width">矩形宽度（列数）。</param>
        /// <param name="height">矩形高度（行数）。</param>
        public ClusteringMaze(int drawValue, uint startX, uint startY, uint width, uint height) : base(drawValue,
            startX, startY, width, height)
        {
        }

        /// <summary>
        /// 使用绘制值构造ClusteringMaze实例，范围使用默认或另行设置。
        /// </summary>
        /// <param name="drawValue">用于绘制墙/通路的值。</param>
        public ClusteringMaze(int drawValue) : base(drawValue)
        {
        }
    }
}