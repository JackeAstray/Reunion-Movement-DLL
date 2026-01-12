using System;
using System.Linq;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;
using ReunionMovementDLL.Dungeon.Random;
using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Util;

namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// 使用 Diamond-Square 算法在矩形区域生成角落为高度源的岛屿地形。
    /// 继承自 RectBaseFractal，并实现了 IDrawer<int> 和 ITerrainDrawer。
    /// </summary>
    public class DiamondSquareAverageCornerIsland : RectBaseFractal<DiamondSquareAverageCornerIsland>, IDrawer<int>, ITerrainDrawer
    {
        /// <summary>
        /// 内部使用的随机数生成器（XorShift128）。
        /// </summary>
        XorShift128 rand = new XorShift128();

        /// <summary>
        /// 根据提供的矩阵尺寸计算最接近且符合 Diamond-Square 要求的地图尺寸（2 的幂），
        /// 返回时会将结果右移一位（除以 2），此实现用于确定用于分形算法的基准大小。
        /// </summary>
        /// <param name="matrixSize">矩阵的尺寸（通常为行或列数）。</param>
        /// <returns>适合用于算法的 mapSize（已右移一位）。</returns>
        public int GetMatrixSize(int matrixSize)
        {
            var mapSize = 2;
            while (true)
            {
                if (mapSize + 1 > matrixSize) return mapSize >>= 1;
                else mapSize <<= 1;
            }
        }

        /// <summary>
        /// 将地形绘制到整型矩阵中，调用内部的标准绘制逻辑。
        /// </summary>
        /// <param name="matrix">目标高度矩阵。</param>
        /// <returns>如果绘制成功则返回 true，否则返回 false。</returns>
        public bool Draw(int[,] matrix)
        {
            return DrawNormal(matrix);
        }

        /// <summary>
        /// 带日志的绘制方法（未实现）。
        /// </summary>
        /// <param name="matrix">目标高度矩阵。</param>
        /// <param name="log">输出日志字符串（未实现）。</param>
        /// <returns>始终抛出 <see cref="NotImplementedException"/>。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 创建并返回已绘制的矩阵（便捷方法）。
        /// </summary>
        /// <param name="matrix">传入并将被修改的高度矩阵。</param>
        /// <returns>返回同一个已修改的矩阵引用。</returns>
        public int[,] Create(int[,] matrix)
        {
            Draw(matrix);
            return matrix;
        }

        /// <summary>
        /// 标准绘制流程：根据当前实例的 altitude 判断是否可绘制，
        /// 若 width 为 0 使用完整高度，否则使用指定宽度进行绘制。
        /// </summary>
        /// <param name="matrix">目标高度矩阵。</param>
        /// <returns>绘制是否成功。</returns>
        private bool DrawNormal(int[,] matrix)
        {
            if (this.altitude < 2) return false;
            return (this.width == 0)
                ? DrawSTL(matrix, CalcEndY(MatrixUtil.GetY(matrix)))
                : DrawWidthSTL(matrix, this.startX + this.width, this.CalcEndY(MatrixUtil.GetY(matrix)));
            // return DrawSTL(matrix, CalcEndY(MatrixUtil.GetY(matrix)));
        }

        /// <summary>
        /// 将浮点矩阵标准化为整数矩阵并绘制，然后将结果归一化回浮点矩阵。
        /// 主要用于将浮点格式的数据作为输入并得到归一化输出。
        /// 使用 LINQ 进行二维数组扁平化并保留坐标以填充整型矩阵。
        /// </summary>
        /// <param name="matrix">要绘制并归一化的浮点矩阵。</param>
        /// <returns>总是返回 true（当绘制完成）。</returns>
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
        /// 将整型矩阵按最大值归一化到目标浮点矩阵中（0.0 - 1.0）。
        /// </summary>
        /// <param name="matrix">源整型高度矩阵。</param>
        /// <param name="retMatrix">目标浮点矩阵，将被写入归一化结果。</param>
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
        /// 根据给定的 endX 和 endY 选择合适的 mapSize 并执行 AssignSTL（用于指定宽度的绘制）。
        /// </summary>
        /// <param name="matrix">目标高度矩阵。</param>
        /// <param name="endX">结束的 X 索引（列）。</param>
        /// <param name="endY">结束的 Y 索引（行）。</param>
        /// <returns>始终返回 true（表示已成功设置并开始绘制）。</returns>
        private bool DrawWidthSTL(int[,] matrix, uint endX, uint endY)
        {
            var x_ = MatrixUtil.GetX(matrix);
            if (this.altitude < 2) return false;
            AssignSTL(matrix, GetMatrixSize(endY > Math.Min((int)x_, (int)endX) ? (int)Math.Min(x_, endX) : (int)endY));
            return true;
        }

        /// <summary>
        /// 根据 endY 选择合适的 mapSize 并执行 AssignSTL（用于默认宽度的绘制）。
        /// </summary>
        /// <param name="matrix">目标高度矩阵。</param>
        /// <param name="endY">结束的 Y 索引（行）。</param>
        /// <returns>始终返回 true（表示已成功设置并开始绘制）。returns>
        private bool DrawSTL(int[,] matrix, uint endY)
        {
            var x_ = MatrixUtil.GetX(matrix);
            AssignSTL(matrix, GetMatrixSize(endY > x_ ? (int)x_ : (int)endY));
            return true;
        }

        /// <summary>
        /// 使用给定的 mapSize 和默认的衰减函数（arg => arg/2）初始化四个角点并调用 Diamond-Square 算法。
        /// </summary>
        /// <param name="matrix">目标高度矩阵。</param>
        /// <param name="mapSize">用于分形计算的地图尺寸（通常为 2 的幂）。</param>
        private void AssignSTL(int[,] matrix, int mapSize)
        {
            Func<int, int> func = arg => arg / 2;
            AssignSTL(matrix, mapSize, func);
        }

        /// <summary>
        /// 使用指定的 mapSize 和衰减函数初始化角点高度并调用 Diamond-Square 算法开始递归生成地形。
        /// 角点高度值基于实例的 minValue 和 altitude，并在其余三个角上加入随机偏移。
        /// </summary>
        /// <param name="matrix">目标高度矩阵。</param>
        /// <param name="mapSize">用于分形计算的地图尺寸（通常为 2 的幂）。</param>
        /// <param name="func">用于衰减 addAltitude 的函数（例如每次递归除以 2）。</param>
        private void AssignSTL(int[,] matrix, int mapSize, Func<int, int> func)
        {
            matrix[this.startY, this.startX] = this.minValue + this.altitude;
            matrix[this.startY, this.startX + mapSize] = this.minValue + (int)rand.Next((uint)this.altitude);
            matrix[this.startY + mapSize, this.startX] = this.minValue + (int)rand.Next((uint)this.altitude);
            matrix[this.startY + mapSize, this.startX + mapSize] =
                this.minValue + (int)rand.Next((uint)this.altitude);
            DiamondSquareAverage.CreateDiamondSquareAverage(matrix, this.startX, this.startY, (uint)mapSize / 2,
                (uint)mapSize / 2,
                (uint)mapSize / 2, matrix[this.startY, this.startX], matrix[this.startY + mapSize, this.startX],
                matrix[this.startY, this.startX + mapSize], matrix[this.startY + mapSize, this.startX + mapSize],
                this.minValue + this.altitude, this.addAltitude, rand, func);
        }


        /// <summary>
        /// 默认构造函数，使用基类默认设置初始化实例。
        /// </summary>
        public DiamondSquareAverageCornerIsland()
        {
        }

        /// <summary>
        /// 使用起始坐标和大小初始化实例。
        /// </summary>
        /// <param name="startX">起始 X 坐标（列偏移）。</param>
        /// <param name="startY">起始 Y 坐标（行偏移）。</param>
        /// <param name="width">区域宽度。</param>
        /// <param name="height">区域高度。</param>
        public DiamondSquareAverageCornerIsland(uint startX, uint startY, uint width, uint height) :
            base(startX, startY, width, height)
        {
        }

        /// <summary>
        /// 使用最小值初始化实例（minValue）。
        /// </summary>
        /// <param name="minValue">最低高度值或基准值。</param>
        public DiamondSquareAverageCornerIsland(int minValue) : base(minValue)
        {
        }

        /// <summary>
        /// 使用最小值和高度（altitude）初始化实例。
        /// </summary>
        /// <param name="minValue">最低高度值或基准值。</param>
        /// <param name="altitude">高度范围或基准高度。</param>
        public DiamondSquareAverageCornerIsland(int minValue, int altitude) : base(minValue, altitude)
        {
        }

        /// <summary>
        /// 使用最小值、高度和附加高度参数初始化实例。
        /// </summary>
        /// <param name="minValue">最低高度值或基准值。</param>
        /// <param name="altitude">高度范围或基准高度。</param>
        /// <param name="addAltitude">用于控制随机偏移的附加高度参数。</param>
        public DiamondSquareAverageCornerIsland(int minValue, int altitude, int addAltitude) : base(minValue, altitude,
            addAltitude)
        {
        }

        /// <summary>
        /// 使用矩阵范围和最小值初始化实例。
        /// </summary>
        /// <param name="matrixRange">矩阵范围（Coordinate2DMatrix）。param>
        /// <param name="minValue">最低高度值或基准值。</param>
        public DiamondSquareAverageCornerIsland(MatrixRange matrixRange, int minValue) : base(matrixRange, minValue)
        {
        }

        /// <summary>
        /// 使用矩阵范围、最小值和高度初始化实例。
        /// </summary>
        /// <param name="matrixRange">矩阵范围（Coordinate2DMatrix）。</param>
        /// <param name="minValue">最低高度值或基准值。</param>
        /// <param name="altitude">高度范围或基准高度。</param>
        public DiamondSquareAverageCornerIsland(MatrixRange matrixRange, int minValue, int altitude) : base(matrixRange,
            minValue,
            altitude)
        {
        }

        /// <summary>
        /// 使用矩阵范围、最小值、高度和附加高度初始化实例。
        /// </summary>
        /// <param name="matrixRange">矩阵范围（Coordinate2DMatrix）。</param>
        /// <param name="minValue">最低高度值或基准值。</param>
        /// <param name="altitude">高度范围或基准高度。</param>
        /// <param name="addAltitude">用于控制随机偏移的附加高度参数。</param>
        public DiamondSquareAverageCornerIsland(MatrixRange matrixRange, int minValue, int altitude, int addAltitude) :
            base(
                matrixRange,
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
        public DiamondSquareAverageCornerIsland(uint startX, uint startY, uint width, uint height, int minValue) :
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
        public DiamondSquareAverageCornerIsland(uint startX, uint startY, uint width, uint height, int minValue,
            int altitude) :
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
        public DiamondSquareAverageCornerIsland(uint startX, uint startY, uint width, uint height, int minValue,
            int altitude,
            int addAltitude) :
            base(startX, startY, width, height, minValue, altitude, addAltitude)
        {
        }
    }
}