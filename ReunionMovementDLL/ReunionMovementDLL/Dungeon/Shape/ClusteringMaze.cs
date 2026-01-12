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

        /// <summary>
        /// 为指定单元(x,y)查找一个不同集合的邻居并与之合并（如果存在）。
        /// 如果找到不同集合的邻居，会将对应的outX/outY/outDir设置为邻居位置和方向。
        /// </summary>
        /// <param name="data">并查集的父指针数组。</param>
        /// <param name="rank">用于按秩合并的秩数组。</param>
        /// <param name="mWidth">缩小格子宽度（单元格数）。</param>
        /// <param name="mHeight">缩小格子高度（单元格数）。</param>
        /// <param name="mSize">总单元格数（mWidth * mHeight）。</param>
        /// <param name="x">单元格的x索引（在mWidth范围内）。</param>
        /// <param name="y">单元格的y索引（在mHeight范围内）。</param>
        /// <param name="outX">输出参数：找到的邻居x。</param>
        /// <param name="outY">输出参数：找到的邻居y。</param>
        /// <param name="outDir">输出参数：从原单元到邻居的方向。</param>
        private void uniteDifNeighbor(uint[] data, uint[] rank, uint mWidth, uint mHeight, uint mSize, uint x, uint y,
            ref uint outX, ref uint outY, ref Direction outDir)
        {
            uint oX = 0, oY = 0;
            Direction oDir = Direction.UP_DIR;
            if (this.findDifNeighbor(data, mWidth, mHeight, mSize, x, y, ref oX, ref oY, ref oDir) != -1)
                this.unite(data, rank, y * mWidth + x, oY * mWidth + oX);
            outX = oX;
            outY = oY;
            outDir = oDir;
        }

        /// <summary>
        /// 查找与单元(x,y)不属于同一集合的邻居（随机顺序检查同一集合内的单元和方向）。
        /// 如果找到，返回0并通过out参数设置邻居位置和方向；如果未找到，返回-1。
        /// </summary>
        /// <param name="data">并查集的父指针数组。</param>
        /// <param name="mWidth">缩小格子宽度（单元格数）。</param>
        /// <param name="mHeight">缩小格子高度（单元格数）。</param>
        /// <param name="mSize">总单元格数（mWidth * mHeight）。</param>
        /// <param name="x">单元格的x索引（在mWidth范围内）。</param>
        /// <param name="y">单元格的y索引（在mHeight范围内）。</param>
        /// <param name="outX">输出参数：找到的邻居x。</param>
        /// <param name="outY">输出参数：找到的邻居y。</param>
        /// <param name="outDir">输出参数：从原单元到邻居的方向。</param>
        /// <returns>找到返回0，未找到返回-1。</returns>
        int findDifNeighbor(uint[] data, uint mWidth, uint mHeight, uint mSize, uint x, uint y,
            ref uint outX, ref uint outY, ref Direction outDir)
        {
            var sameTags = new List<uint>();
            var cellind = y * mWidth + x;
            for (uint i = 0; i < mSize; ++i)
            {
                if (this.same(data, cellind, i)) sameTags.Add(i);
            }

            ArrayUtil.Shuffle(sameTags, rand);

            uint cell1X = 0, cell1Y = 0, cell2X = 0, cell2Y = 0, cell2ind = 0;
            var dirs = new Direction[4];
            dirs[0] = Direction.UP_DIR;
            dirs[1] = Direction.RIGHT_DIR;
            dirs[2] = Direction.DOWN_DIR;
            dirs[3] = Direction.LEFT_DIR;

            foreach (var cell1ind in sameTags)
            {
                //                Debug.Log(cell1ind);
                cell1X = cell1ind % mWidth;
                cell1Y = cell1ind / mWidth;
                dirs = ArrayUtil.Shuffle(dirs, rand);
                foreach (var dir in dirs)
                {
                    //                    Debug.Log(dir.ToString());
                    if ((dirDx(dir) < 0 && cell1X == 0) || (dirDx(dir) > 0 && cell1X == mWidth - 1)) continue;
                    cell2X = (uint)((int)cell1X + dirDx(dir));
                    if ((dirDy(dir) < 0 && cell1Y == 0) || (dirDy(dir) > 0 && cell1Y == mHeight - 1)) continue;
                    cell2Y = (uint)((int)cell1Y + dirDy(dir));
                    cell2ind = cell2Y * mWidth + cell2X;
                    if (!this.same(data, cell1ind, cell2ind))
                    {
                        outX = cell2X;
                        outY = cell2Y;
                        outDir = dir;
                        return 0;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// 检查并查集中是否所有元素都属于同一集合（即迷宫是否已完全连通）。
        /// </summary>
        /// <param name="data">并查集的父指针数组。</param>
        /// <param name="dSize">并查集大小（元素数）。</param>
        /// <returns>如果所有元素都与索引0相同（连通）则返回true。</returns>
        private bool isAllSame(uint[] data, uint dSize)
        {
            for (uint i = 1; i < dSize; ++i)
                if (!this.same(data, i, 0))
                    return false;
            return true;
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

            uint randCellX = 0;
            uint randCellY = 0;
            uint outX = 0;
            uint outY = 0;
            Direction outDir = Direction.UP_DIR;
            while (!this.isAllSame(data, mSize))
            {
                randCellX = rand.Next() % (width3 / 2);
                randCellY = rand.Next() % (height3 / 2);

                this.uniteDifNeighbor(data, rank, mWidth, mHeight, mSize, randCellX, randCellY, ref outX, ref outY, ref outDir);

                // break wall
                matrix[2 * outY + 1 - this.dirDy(outDir), 2 * outX + 1 - this.dirDx(outDir)] = this.drawValue;
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