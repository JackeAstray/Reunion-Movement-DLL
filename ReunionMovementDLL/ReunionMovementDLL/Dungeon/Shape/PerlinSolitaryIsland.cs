using ReunionMovementDLL.Dungeon.Random;
using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Util;
using System;
using System.Linq;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// 基于 Perlin 噪声的独立岛屿生成器。
    /// 生成一个居中的孤立岛屿，岛屿高度由金字塔形状和 Perlin 噪声共同决定。
    /// </summary>
    public sealed class PerlinSolitaryIsland : RectBasePerlinSolitary<PerlinSolitaryIsland>, IDrawer<int>, ITerrainDrawer
    {
        /// <summary>
        /// 用于噪声种子的伪随机数生成器（XorShift128）。
        /// </summary>
        private XorShift128 rand = new XorShift128();

        /// <summary>
        /// 将结果绘制到整型矩阵（不返回日志）。
        /// </summary>
        /// <param name="matrix">目标整型矩阵。</param>
        /// <returns>绘制是否成功。</returns>
        public bool Draw(int[,] matrix)
        {
            return DrawNormal(matrix);
        }

        /// <summary>
        /// 带日志输出的绘制方法（未实现）。
        /// </summary>
        /// <param name="matrix">目标整型矩阵。</param>
        /// <param name="log">输出日志（out）。</param>
        /// <returns>抛出 <see cref="NotImplementedException"/> 表示未实现。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 在给定矩阵上创建岛屿并返回该矩阵（便捷方法）。
        /// </summary>
        /// <param name="matrix">目标整型矩阵。</param>
        /// <returns>已填充的矩阵引用。</returns>
        public int[,] Create(int[,] matrix)
        {
            this.Draw(matrix);
            return matrix;
        }

        /// <summary>
        /// 标准绘制实现：根据金字塔截断高度、山体比例与 Perlin 噪声为每个单元生成高度值并写回矩阵。
        /// 方法会校验比例参数并使用类中配置的起始坐标和尺寸限制绘制范围。
        /// </summary>
        /// <param name="matrix">目标整型矩阵。</param>
        /// <returns>绘制是否成功；如果参数非法返回 false，否则返回 true。</returns>
        private bool DrawNormal(int[,] matrix)
        {
            if (this.mountainProportion < 0.0 || this.mountainProportion > 1.0) return false;
            if (this.truncatedProportion <= 0.0 || this.truncatedProportion > 1.0) return false;
            uint endX = CalcEndX(MatrixUtil.GetX(matrix));
            uint endY = CalcEndY(MatrixUtil.GetY(matrix));
            uint midX = endX / 2;
            uint midY = endY / 2;

            int perlinHeight = (int)((double)(maxHeight - minHeight) * (1.0 - mountainProportion));
            int truncatedHeight = (int)((double)(maxHeight - minHeight) * mountainProportion);
            int pyramidHeight = (int)(truncatedHeight / truncatedProportion);

            PerlinNoise perlinNoise = new PerlinNoise();
            double frequencyX = (endX - this.startX) / this.frequency;
            double frequencyY = (endX - this.startY) / this.frequency;

            for (var row = startY; row < endY; ++row)
            {
                var row2 = row > midY ? endY - row - 1 : row;
                for (var col = startX; col < endX; ++col)
                {
                    var col2 = col > midX ? endX - col - 1 : col;
                    int setValue = Math.Min((int)((pyramidHeight * row2) / midY),
                        (int)((pyramidHeight * col2) / midX));
                    matrix[row, col] = this.minHeight + ((setValue > truncatedHeight) ? truncatedHeight : setValue) +
                                       (int)(perlinHeight * perlinNoise.OctaveNoise(this.octaves, col / frequencyX,
                                                  row / frequencyY));
                }
            }

            return true;
        }

        /// <summary>
        /// 将整数矩阵归一化写入浮点矩阵：先将传入的浮点矩阵转换为整型矩阵（使用 LINQ 生成元素序列），
        /// 在整型矩阵上执行绘制，然后将结果按 maxHeight 归一化回浮点矩阵。
        /// </summary>
        /// <param name="matrix">目标浮点矩阵（作为归一化输出的容器）。</param>
        /// <returns>绘制并归一化是否成功；当 matrix 为 null 时返回 false。</returns>
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
        /// 将整型高度矩阵按 maxHeight 归一化写回浮点矩阵。
        /// </summary>
        /// <param name="matrix">源整型矩阵。</param>
        /// <param name="retMatrix">目标浮点矩阵。</param>
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

        /* Constructors */

        /// <summary>
        /// 默认构造函数。
        /// </summary>
        public PerlinSolitaryIsland()
        {
        } // = default()

        /// <summary>
        /// 使用起始坐标与尺寸初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(uint startX, uint startY, uint width, uint height) : base(startX, startY, width,
            height)
        {
        }

        /// <summary>
        /// 使用截断比例初始化的构造函数（决定金字塔截断与山坡比例的基准）。
        /// </summary>
        public PerlinSolitaryIsland(double truncatedProportion) : base(truncatedProportion)
        {
        }

        /// <summary>
        /// 使用截断比例与山体比例初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(double truncatedProportion, double mountainProportion) : base(truncatedProportion,
            mountainProportion)
        {
        }

        /// <summary>
        /// 使用截断比例、山体比例与频率初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(double truncatedProportion, double mountainProportion, double frequency) : base(
            truncatedProportion, mountainProportion, frequency)
        {
        }

        /// <summary>
        /// 使用截断比例、山体比例、频率与倍频数初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(double truncatedProportion, double mountainProportion, double frequency,
            uint octaves) : base(truncatedProportion, mountainProportion, frequency, octaves)
        {
        }

        /// <summary>
        /// 使用截断比例、山体比例、频率、倍频与最大高度初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(double truncatedProportion, double mountainProportion, double frequency,
            uint octaves, int maxHeight) : base(truncatedProportion, mountainProportion, frequency, octaves,
            maxHeight)
        {
        }

        /// <summary>
        /// 使用截断比例、山体比例、频率、倍频、最大高度与最小高度初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(double truncatedProportion, double mountainProportion, double frequency,
            uint octaves, int maxHeight, int minHeight) : base(truncatedProportion, mountainProportion, frequency,
            octaves, maxHeight, minHeight)
        {
        }

        /// <summary>
        /// 使用矩阵范围与截断比例初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(MatrixRange matrixRange, double truncatedProportion) : base(matrixRange,
            truncatedProportion)
        {
        }

        /// <summary>
        /// 使用矩阵范围、截断比例与山体比例初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(MatrixRange matrixRange, double truncatedProportion, double mountainProportion) :
            base(matrixRange, truncatedProportion, mountainProportion)
        {
        }

        /// <summary>
        /// 使用矩阵范围、截断比例、山体比例与频率初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(MatrixRange matrixRange, double truncatedProportion, double mountainProportion,
            double frequency) : base(matrixRange, truncatedProportion, mountainProportion, frequency)
        {
        }

        /// <summary>
        /// 使用矩阵范围、截断比例、山体比例、频率与倍频初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(MatrixRange matrixRange, double truncatedProportion, double mountainProportion,
            double frequency, uint octaves) : base(matrixRange, truncatedProportion, mountainProportion, frequency,
            octaves)
        {
        }

        /// <summary>
        /// 使用矩阵范围、截断比例、山体比例、频率、倍频与最大高度初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(MatrixRange matrixRange, double truncatedProportion, double mountainProportion,
            double frequency, uint octaves, int maxHeight) : base(matrixRange, truncatedProportion, mountainProportion,
            frequency, octaves, maxHeight)
        {
        }

        /// <summary>
        /// 使用矩阵范围、截断比例、山体比例、频率、倍频、最大高度与最小高度初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(MatrixRange matrixRange, double truncatedProportion, double mountainProportion,
            double frequency, uint octaves, int maxHeight, int minHeight) : base(matrixRange, truncatedProportion,
            mountainProportion, frequency, octaves, maxHeight, minHeight)
        {
        }

        /// <summary>
        /// 使用坐标、尺寸与截断比例初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(uint startX, uint startY, uint width, uint height, double truncatedProportion) :
            base(startX, startY, width, height, truncatedProportion)
        {
        }

        /// <summary>
        /// 使用坐标、尺寸、截断比例与山体比例初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(uint startX, uint startY, uint width, uint height, double truncatedProportion,
            double mountainProportion) : base(startX, startY, width, height, truncatedProportion, mountainProportion)
        {
        }

        /// <summary>
        /// 使用坐标、尺寸、截断比例、山体比例与频率初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(uint startX, uint startY, uint width, uint height, double truncatedProportion,
            double mountainProportion, double frequency) : base(startX, startY, width, height, truncatedProportion,
            mountainProportion, frequency)
        {
        }

        /// <summary>
        /// 使用坐标、尺寸、截断比例、山体比例、频率与倍频初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(uint startX, uint startY, uint width, uint height, double truncatedProportion,
            double mountainProportion, double frequency, uint octaves) : base(startX, startY, width, height,
            truncatedProportion, mountainProportion, frequency, octaves)
        {
        }

        /// <summary>
        /// 使用坐标、尺寸、截断比例、山体比例、频率、倍频与最大高度初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(uint startX, uint startY, uint width, uint height, double truncatedProportion,
            double mountainProportion, double frequency, uint octaves, int maxHeight) : base(startX, startY, width,
            height, truncatedProportion, mountainProportion, frequency, octaves, maxHeight)
        {
        }

        /// <summary>
        /// 使用坐标、尺寸、截断比例、山体比例、频率、倍频、最大高度与最小高度初始化的构造函数。
        /// </summary>
        public PerlinSolitaryIsland(uint startX, uint startY, uint width, uint height, double truncatedProportion,
            double mountainProportion, double frequency, uint octaves, int maxHeight, int minHeight) : base(startX,
            startY, width, height, truncatedProportion, mountainProportion, frequency, octaves, maxHeight, minHeight)
        {
        }
    }
}