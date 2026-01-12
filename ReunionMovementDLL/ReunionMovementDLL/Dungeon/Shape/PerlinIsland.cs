using ReunionMovementDLL.Dungeon.Random;
using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Util;
using System;
using System.Linq;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// 基于 Perlin 噪声的岛屿高度图生成器。
    /// 继承自 RectBasePerlin，提供将噪声结果绘制到整型或浮点型矩阵的能力。
    /// </summary>
    public sealed class PerlinIsland : RectBasePerlin<PerlinIsland>, IDrawer<int>, ITerrainDrawer
    {
        /// <summary>
        /// 伪随机数生成器（XorShift128）。用于 Perlin 噪声的种子或其它随机选择。
        /// </summary>
        private XorShift128 rand = new XorShift128();

        /// <summary>
        /// 将当前形状绘制到整型矩阵（不返回日志）。
        /// </summary>
        /// <param name="matrix">目标整型矩阵。</param>
        /// <returns>绘制是否成功，当前实现总是返回 true。</returns>
        public bool Draw(int[,] matrix)
        {
            return DrawNormal(matrix);
        }

        /// <summary>
        /// 带日志输出的绘制方法（尚未实现）。
        /// </summary>
        /// <param name="matrix">目标整型矩阵。</param>
        /// <param name="log">输出日志（out）。</param>
        /// <returns>抛出 <see cref="NotImplementedException"/> 表示方法未实现。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 将 Perlin 噪声绘制并标准化为浮点矩阵。先将传入的浮点矩阵转换为整型矩阵（使用 LINQ 进行行/列映射），
        /// 在整型矩阵上调用绘制，然后将结果归一化回浮点矩阵。
        /// </summary>
        /// <param name="matrix">目标浮点矩阵（作为归一化输出的容器）。</param>
        /// <returns>绘制并归一化是否成功，当前实现总是返回 true。</returns>
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
        /// 标准绘制实现：在矩形区域内基于 Perlin 噪声为每个单元生成高度值并写入整型矩阵。
        /// 使用类中配置的 frequency、octaves、maxHeight、minHeight 等参数。
        /// </summary>
        /// <param name="matrix">目标整型矩阵。</param>
        /// <returns>绘制是否成功，当前实现总是返回 true。</returns>
        private bool DrawNormal(int[,] matrix)
        {
            uint endX = CalcEndX(MatrixUtil.GetX(matrix));
            uint endY = CalcEndY(MatrixUtil.GetY(matrix));

            PerlinNoise perlin = new PerlinNoise((int)rand.Next());

            double frequencyX = (endX - startX) / frequency;
            double frequencyY = (endY - startY) / frequency;

            // 为矩形区域内的每个坐标生成噪声高度并写入矩阵
            for (uint row = startY; row < endY; ++row)
            {
                for (uint col = startX; col < endX; ++col)
                {
                    matrix[row, col] = minHeight + minHeight + (int)((double)(maxHeight - minHeight) *
                                       perlin.OctaveNoise(octaves, (col / frequencyX),
                                           (row / frequencyY)));
                }
            }

            return true;
        }

        /// <summary>
        /// 将整型矩阵中的高度值归一化写入浮点矩阵，使用派生类中的 maxHeight 作为归一化基准。
        /// </summary>
        /// <param name="matrix">源整型矩阵。</param>
        /// <param name="retMatrix">目标浮点矩阵（归一化结果写入）。</param>
        private void Normalize(int[,] matrix, float[,] retMatrix)
        {
            // use maxHeight from derived class.
            for (int y = 0; y < MatrixUtil.GetY(matrix); ++y)
            {
                for (int x = 0; x < MatrixUtil.GetX(matrix); ++x)
                {
                    retMatrix[y, x] = (float)matrix[y, x] / maxHeight;
                }
            }
        }

        /// <summary>
        /// 默认构造函数。
        /// </summary>
        public PerlinIsland()
        {
        } // = default()

        /// <summary>
        /// 使用坐标和尺寸初始化的构造函数。
        /// </summary>
        public PerlinIsland(uint startX, uint startY, uint width, uint height) : base(startX, startY, width, height)
        {
        }

        /// <summary>
        /// 使用频率初始化的构造函数。
        /// </summary>
        /// <param name="frequency">噪声频率。</param>
        public PerlinIsland(double frequency)
        {
            this.frequency = frequency;
        }

        /// <summary>
        /// 使用频率和倍频数初始化的构造函数。
        /// </summary>
        public PerlinIsland(double frequency, uint octaves) : base(frequency, octaves)
        {
        }

        /// <summary>
        /// 使用频率、倍频和最大高度初始化的构造函数。
        /// </summary>
        public PerlinIsland(double frequency, uint octaves, int maxHeight)
        {
            this.frequency = frequency;
            this.octaves = octaves;
            this.maxHeight = maxHeight;
        }

        /// <summary>
        /// 使用频率、倍频、最大高度和最小高度初始化的构造函数。
        /// </summary>
        public PerlinIsland(double frequency, uint octaves, int maxHeight, int minHeight) : base(frequency, octaves,
            maxHeight, minHeight)
        {
        }

        /// <summary>
        /// 使用矩阵范围和频率初始化的构造函数。
        /// </summary>
        public PerlinIsland(MatrixRange matrixRange, double frequency) : base(matrixRange, frequency)
        {
        }

        /// <summary>
        /// 使用矩阵范围、频率和倍频初始化的构造函数。
        /// </summary>
        public PerlinIsland(MatrixRange matrixRange, double frequency, uint octaves) : base(matrixRange, frequency,
            octaves)
        {
        }

        /// <summary>
        /// 使用矩阵范围、频率、倍频和最大高度初始化的构造函数。
        /// </summary>
        public PerlinIsland(MatrixRange matrixRange, double frequency, uint octaves, int maxHeight) : base(
            matrixRange, frequency, octaves, maxHeight)
        {
        }

        /// <summary>
        /// 使用矩阵范围、频率、倍频、最大高度和最小高度初始化的构造函数。
        /// </summary>
        public PerlinIsland(MatrixRange matrixRange, double frequency, uint octaves, int maxHeight, int minHeight) :
            base(matrixRange, frequency, octaves, maxHeight, minHeight)
        {
        }

        /// <summary>
        /// 使用坐标、尺寸和频率初始化的构造函数。
        /// </summary>
        public PerlinIsland(uint startX, uint startY, uint width, uint height, double frequency) : base(startX, startY,
            width, height, frequency)
        {
        }

        /// <summary>
        /// 使用坐标、尺寸、频率和倍频初始化的构造函数。
        /// </summary>
        public PerlinIsland(uint startX, uint startY, uint width, uint height, double frequency, uint octaves) : base(
            startX, startY, width, height, frequency, octaves)
        {
        }

        /// <summary>
        /// 使用坐标、尺寸、频率、倍频和最大高度初始化的构造函数。
        /// </summary>
        public PerlinIsland(uint startX, uint startY, uint width, uint height, double frequency, uint octaves,
            int maxHeight) : base(startX, startY, width, height, frequency, octaves, maxHeight)
        {
        }

        /// <summary>
        /// 使用坐标、尺寸、频率、倍频、最大高度和最小高度初始化的构造函数。
        /// </summary>
        public PerlinIsland(uint startX, uint startY, uint width, uint height, double frequency, uint octaves,
            int maxHeight, int minHeight) : base(startX, startY, width, height, frequency, octaves, maxHeight,
            minHeight)
        {
        }
    }
}