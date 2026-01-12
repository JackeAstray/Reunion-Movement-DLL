using ReunionMovementDLL.Dungeon.Shape;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;
using System;
using System.Linq;
using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Random;
using ReunionMovementDLL.Dungeon.Util;

namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// 使用分块的 Diamond-Square 算法生成岛屿地形的实现。
    /// 将地图分为多个小块（chunk），分别生成后拼接在一起以构建整个地形。
    /// </summary>
    public sealed class FractalIsland : RectBaseFractal<FractalIsland>, IDrawer<int>, ITerrainDrawer
    {
        /// <summary>
        /// 内部随机数生成器（XorShift128）。用于生成高度噪声。
        /// </summary>
        private XorShift128 rand = new XorShift128();

        /// <summary>
        /// 每个分块的大小（chunk 的边长），实际使用时会使用 fiChunkSize + 1 的维度矩阵。
        /// </summary>
        private readonly int fiChunkSize = 16;

        /// <summary>
        /// 将地形绘制到整数矩阵中，调用内部常规绘制流程。
        /// </summary>
        /// <param name="matrix">目标高度矩阵。</param>
        /// <returns>绘制是否成功。</returns>
        public bool Draw(int[,] matrix)
        {
            return DrawNormal(matrix);
        }

        /// <summary>
        /// 带日志输出的绘制方法（当前未实现）。
        /// </summary>
        /// <param name="matrix">目标高度矩阵。</param>
        /// <param name="log">输出日志字符串（未实现）。</param>
        /// <returns>当前实现会抛出 <see cref="NotImplementedException"/>。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 将浮点矩阵转换为整型并绘制，随后将结果归一化回浮点矩阵（0.0 - 1.0）。
        /// 该方法可用于处理浮点输入/输出的数据管线。
        /// 使用 LINQ 对二维数组扁平化并保留坐标进行转换。
        /// </summary>
        /// <param name="matrix">输入/输出的浮点矩阵。</param>
        /// <returns>总是返回 true（表示处理完成）。</returns>
        public bool DrawNormalize(float[,] matrix)
        {
            if (matrix == null)
                return false;

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int[,] convertedMatrix = new int[rows, cols];

            var items = Enumerable.Range(0, rows).SelectMany(y => Enumerable.Range(0, cols).Select(x => new { x, y, Value = (int)matrix[y, x] }));

            foreach (var it in items)
            {
                convertedMatrix[it.y, it.x] = it.Value;
            }

            DrawNormal(convertedMatrix);
            Normalize(convertedMatrix, matrix);
            return true;
        }

        /// <summary>
        /// 创建并返回已绘制的整数矩阵（便捷方法）。
        /// </summary>
        /// <param name="matrix">待绘制的整数矩阵。</param>
        /// <returns>返回已被修改的矩阵引用。</returns>
        public int[,] Create(int[,] matrix)
        {
            this.Draw(matrix);
            return matrix;
        }

        /// <summary>
        /// 将整型矩阵按其最大值归一化写入目标浮点矩阵（值域 0.0 到 1.0）。
        /// </summary>
        /// <param name="matrix">源整型高度矩阵。</param>
        /// <param name="retMatrix">目标浮点矩阵，用于写入归一化结果。</param>
        private void Normalize(int[,] matrix, float[,] retMatrix)
        {
            var maxHeight = MatrixUtil.GetMax(matrix);

            for (int y = 0; y < MatrixUtil.GetY(matrix); ++y)
            {
                for (int x = 0; x < MatrixUtil.GetX(matrix); ++x)
                {
                    retMatrix[y, x] = (float)matrix[y, x] / maxHeight;
                }
            }
        }

        /// <summary>
        /// 主绘制逻辑：按照分块（chunk）生成地形并将每个 chunk 的结果拼接到目标矩阵中。
        /// 根据实例的 start/width/height 参数计算分块数量并逐块生成。
        /// </summary>
        /// <param name="matrix">目标整数矩阵。</param>
        /// <returns>绘制是否成功。</returns>
        private bool DrawNormal(int[,] matrix)
        {
            var chunkMatrix = new int[fiChunkSize + 1, fiChunkSize + 1];
            var endX = CalcEndX(MatrixUtil.GetX(matrix));
            var endY = CalcEndY(MatrixUtil.GetY(matrix));

            if (this.altitude < 2) return false;

            int chunkX = (int)(endX - this.startX) / fiChunkSize;
            int chunkY = (int)(endY - this.startY) / fiChunkSize;

            var randUp = new int[chunkX + 1];
            var randDown = new int[chunkX + 1];

            for (var col = 0; col <= chunkX; ++col)
            {
                randUp[col] = 0;
            }

            for (var row = 0; row < chunkY; ++row)
            {
                if (row + 1 == chunkY)
                {
                    for (var col = 0; col <= chunkX; ++col)
                    {
                        randDown[col] = 0;
                    }
                }
                else
                {
                    for (var col = 1; col < chunkX; ++col)
                    {
                        randDown[col] = (int)rand.Next((uint)this.altitude);
                    }

                    randDown[0] = 0;
                    randDown[chunkX] = randDown[0];
                }

                for (var col = 0; col < chunkX; ++col)
                {
                    chunkMatrix[0, 0] = randUp[col];
                    chunkMatrix[fiChunkSize, 0] = randDown[col];
                    chunkMatrix[0, fiChunkSize] = randUp[col + 1];
                    chunkMatrix[fiChunkSize, fiChunkSize] = randDown[col + 1];

                    // 地形の生成
                    this.CreateWorldMapSimple(chunkMatrix);

                    for (var row2 = 0; row2 < fiChunkSize; ++row2)
                    {
                        for (var col2 = 0; col2 < fiChunkSize; ++col2)
                        {
                            matrix[this.startY + row * fiChunkSize + row2, startX + col * fiChunkSize + col2] =
                                chunkMatrix[row2, col2];
                        }
                    }
                }

                for (var col = 0; col <= chunkX; ++col)
                {
                    randUp[col] = randDown[col];
                }
            }

            return true;
        }

        /// <summary>
        /// 使用默认衰减函数（x => x/2）创建一个 chunk 的世界地图（辅助方法）。
        /// </summary>
        /// <param name="chunkMatrix">目标 chunk 矩阵，大小为 fiChunkSize + 1。</param>
        private void CreateWorldMapSimple(int[,] chunkMatrix)
        {
            CreateWorldMap(chunkMatrix, (int x) => x / 2);
        }

        /// <summary>
        /// 对给定 chunk 矩阵调用 Diamond-Square 算法，func 用于衰减 addAltitude。
        /// </summary>
        /// <param name="chunkMatrix">目标 chunk 矩阵。</param>
        /// <param name="func">用于衰减 addAltitude 的函数（每级递归调用）。</param>
        private void CreateWorldMap(int[,] chunkMatrix, Func<int, int> func)
        {
            DiamondSquareAverage.CreateDiamondSquareAverage(chunkMatrix, 0, 0, (uint)fiChunkSize / 2, (uint)fiChunkSize / 2,
                (uint)fiChunkSize / 2, chunkMatrix[0, 0], chunkMatrix[fiChunkSize, 0], chunkMatrix[0, fiChunkSize],
                chunkMatrix[fiChunkSize, fiChunkSize], this.minValue + this.altitude, this.addAltitude, rand, func
            );
        }

        /// <summary>
        /// 默认构造函数，使用基类默认设置初始化实例。
        /// </summary>
        public FractalIsland()
        {
        }

        /// <summary>
        /// 使用起始坐标和大小初始化实例。
        /// </summary>
        /// <param name="startX">起始 X 坐标（列偏移）。param>
        /// <param name="startY">起始 Y 坐标（行偏移）。param>
        /// <param name="width">区域宽度。</param>
        /// <param name="height">区域高度。</param>
        public FractalIsland(uint startX, uint startY, uint width, uint height) :
            base(startX, startY, width, height)
        {
        }

        /// <summary>
        /// 使用最小值初始化实例（minValue）。
        /// </summary>
        /// <param name="minValue">最低高度值或基准值。</param>
        public FractalIsland(int minValue) : base(minValue)
        {
        }

        /// <summary>
        /// 使用最小值和高度（altitude）初始化实例。
        /// </summary>
        /// <param name="minValue">最低高度值或基准值。</param>
        /// <param name="altitude">高度范围或基准高度。</param>
        public FractalIsland(int minValue, int altitude) : base(minValue, altitude)
        {
        }

        /// <summary>
        /// 使用最小值、高度和附加高度参数初始化实例。
        /// </summary>
        /// <param name="minValue">最低高度值或基准值。</param>
        /// <param name="altitude">高度范围或基准高度。</param>
        /// <param name="addAltitude">用于控制随机偏移的附加高度参数。</param>
        public FractalIsland(int minValue, int altitude, int addAltitude) : base(minValue, altitude, addAltitude)
        {
        }

        /// <summary>
        /// 使用矩阵范围和最小值初始化实例。
        /// </summary>
        /// <param name="matrixRange">矩阵范围（Coordinate2DMatrix）。</param>
        /// <param name="minValue">最低高度值或基准值。</param>
        public FractalIsland(MatrixRange matrixRange, int minValue) : base(matrixRange, minValue)
        {
        }

        /// <summary>
        /// 使用矩阵范围、最小值和高度初始化实例。
        /// </summary>
        /// <param name="matrixRange">矩阵范围（Coordinate2DMatrix）。</param>
        /// <param name="minValue">最低高度值或基准值。</param>
        /// <param name="altitude">高度范围或基准高度。</param>
        public FractalIsland(MatrixRange matrixRange, int minValue, int altitude) : base(matrixRange, minValue,
            altitude)
        {
        }

        /// <summary>
        /// 使用矩阵范围、最小值、高度和附加高度初始化实例。
        /// </summary>
        /// <param name="matrixRange">矩阵范围（Coordinate2DMatrix）。param>
        /// <param name="minValue">最低高度值或基准值。</param>
        /// <param name="altitude">高度范围或基准高度。</param>
        /// <param name="addAltitude">用于控制随机偏移的附加高度参数。</param>
        public FractalIsland(MatrixRange matrixRange, int minValue, int altitude, int addAltitude) : base(matrixRange,
            minValue, altitude, addAltitude)
        {
        }

        /// <summary>
        /// 使用起始坐标、大小和最小值初始化实例。
        /// </summary>
        /// <param name="startX">起始 X 坐标（列偏移）。</param>
        /// <param name="startY">起始 Y 坐标（行偏移）。</param>
        /// <param name="width">区域宽度。</param>
        /// <param name="height">区域高度。</param>
        /// <param name="minValue">最低高度值或基准值。</param>
        public FractalIsland(uint startX, uint startY, uint width, uint height, int minValue) :
            base(startX, startY, width, height, minValue)
        {
        }

        /// <summary>
        /// 使用起始坐标、大小、最小值和高度初始化实例。
        /// </summary>
        /// <param name="startX">起始 X 坐标（列偏移）。</param>
        /// <param name="startY">起始 Y 坐标（行偏移）。</param>
        /// <param name="width">区域宽度。</param>
        /// <param name="height">区域高度。</param>
        /// <param name="minValue">最低高度值或基准值。</param>
        /// <param name="altitude">高度范围或基准高度。</param>
        public FractalIsland(uint startX, uint startY, uint width, uint height, int minValue, int altitude) :
            base(startX, startY, width, height, minValue, altitude)
        {
        }

        /// <summary>
        /// 使用起始坐标、大小、最小值、高度和附加高度初始化实例。
        /// </summary>
        /// <param name="startX">起始 X 坐标（列偏移）。</param>
        /// <param name="startY">起始 Y 坐标（行偏移）。</param>
        /// <param name="width">区域宽度。</param>
        /// <param name="height">区域高度。</param>
        /// <param name="minValue">最低高度值或基准值。</param>
        /// <param name="altitude">高度范围或基准高度。</param>
        /// <param name="addAltitude">用于控制随机偏移的附加高度参数。</param>
        public FractalIsland(uint startX, uint startY, uint width, uint height, int minValue, int altitude,
            int addAltitude) :
            base(startX, startY, width, height, minValue, altitude, addAltitude)
        {
        }
    }
}