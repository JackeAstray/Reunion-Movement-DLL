using System;
using System.Linq;
using ReunionMovementDLL.Dungeon.Shape;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;
using ReunionMovementDLL.Dungeon.Random;
using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Util;
using ITerrainDrawer = ReunionMovementDLL.Dungeon.Shape.ITerrainDrawer;

/// <summary>
/// 使用 Diamond-Square 算法生成岛屿地形的类实现（平均值变体）。
/// 继承自 RectBaseFractal，并实现了 IDrawer<int> 与 ITerrainDrawer 接口。
/// </summary>
public class DiamondSquareAverageIsland : RectBaseFractal<DiamondSquareAverageIsland>, IDrawer<int>, ITerrainDrawer
{
    /// <summary>
    /// 内部随机数生成器（XorShift128）。用于在角点与中心添加随机偏移。
    /// </summary>
    XorShift128 rand = new XorShift128();

    /// <summary>
    /// 计算适合用于 Diamond-Square 算法的地图尺寸（以 2 的幂为基准）。
    /// 返回值在找到第一个大于 matrixSize 的 mapSize 前右移一位（相当于除以 2）。
    /// </summary>
    /// <param name="matrixSize">输入的矩阵尺寸（通常为行或列数）。</param>
    /// <returns>用于分形算法的 mapSize（int）。</returns>
    public int GetMatrixSize(int matrixSize)
    {
        var mapSize = 2; // note, overflow.
        while (true)
        {
            if (mapSize + 1 > matrixSize) return mapSize >>= 1;
            else mapSize <<= 1;
        }
    }

    /// <summary>
    /// 将地形绘制到整数矩阵中，调用内部标准绘制逻辑。
    /// </summary>
    /// <param name="matrix">目标整数矩阵（高度值矩阵）。</param>
    /// <returns>绘制是否成功。</returns>
    public bool Draw(int[,] matrix)
    {
        return DrawNormal(matrix);
    }

    /// <summary>
    /// 带日志输出的绘制方法（当前未实现）。
    /// </summary>
    /// <param name="matrix">目标整数矩阵。</param>
    /// <param name="log">输出日志字符串（未实现）。</param>
    /// <returns>当前实现会抛出 NotImplementedException。</returns>
    public bool Draw(int[,] matrix, out string log)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 对传入矩阵进行绘制并返回该矩阵的引用（便捷方法）。
    /// </summary>
    /// <param name="matrix">待绘制的矩阵。</param>
    /// <returns>返回已被修改的同一矩阵引用。</returns>
    public int[,] Create(int[,] matrix)
    {
        this.Draw(matrix);
        return matrix;
    }

    /// <summary>
    /// 标准绘制流程：当 altitude 小于 2 时不执行；否则根据 width 决定调用完整绘制或按宽度绘制。
    /// </summary>
    /// <param name="matrix">目标整数矩阵。</param>
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
    /// 将浮点矩阵转换为整数矩阵进行绘制，然后对结果进行归一化（0-1）并写回浮点矩阵。
    /// 该方法用于支持浮点输入和输出。
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

        // 使用 LINQ 对二维数组进行扁平化并保留坐标，然后将结果写入整型矩阵
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
    /// 将整型矩阵按其最大值归一化到目标浮点矩阵（值域 0.0 到 1.0）。
    /// </summary>
    /// <param name="matrix">源整型矩阵。</param>
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
    /// 根据给定的 endX/endY 选择合适的 mapSize 并调用 AssignSTL（用于按指定宽度绘制）。
    /// </summary>
    /// <param name="matrix">目标整数矩阵。</param>
    /// <param name="endX">结束列索引。</param>
    /// <param name="endY">结束行索引。</param>
    /// <returns>绘制是否成功。</returns>
    private bool DrawWidthSTL(int[,] matrix, uint endX, uint endY)
    {
        var x_ = MatrixUtil.GetX(matrix);
        if (this.altitude < 2) return false;
        AssignSTL(matrix, GetMatrixSize(endY > Math.Min((int)x_, (int)endX) ? (int)Math.Min(x_, endX) : (int)endY));
        return true;
    }

    /// <summary>
    /// 根据 endY 选择合适的 mapSize 并调用 AssignSTL（用于默认宽度绘制）。
    /// </summary>
    /// <param name="matrix">目标整数矩阵。</param>
    /// <param name="endY">结束行索引。</param>
    /// <returns>绘制是否成功。</returns>
    private bool DrawSTL(int[,] matrix, uint endY)
    {
        var x_ = MatrixUtil.GetX(matrix);
        AssignSTL(matrix, GetMatrixSize(endY > x_ ? (int)x_ : (int)endY));
        return true;
    }

    /// <summary>
    /// 使用默认衰减函数（arg => arg / 2）调用 AssignSTL 的简便方法。
    /// </summary>
    /// <param name="matrix">目标整数矩阵。</param>
    /// <param name="mapSize">用于分形计算的 mapSize。</param>
    private void AssignSTL(int[,] matrix, int mapSize)
    {
        Func<int, int> func = arg => arg / 2;
        AssignSTL(matrix, mapSize, func);
    }

    /// <summary>
    /// 初始化矩阵角点和中心点的高度值并对四个子区块分别调用 CreateDiamondSquareAverage 开始递归生成。
    /// 角点与边中点使用随机值，中心点设置为最大高度（minValue + altitude）。
    /// </summary>
    /// <param name="matrix">目标整数矩阵。</param>
    /// <param name="mapSize">用于分形计算的 mapSize（通常为 2 的幂）。</param>
    /// <param name="func">用于衰减 addAltitude 的函数。</param>
    private void AssignSTL(int[,] matrix, int mapSize, Func<int, int> func)
    {
        // static_assert(this.startY == 0 && this.startX == 0)
        matrix[this.startY, this.startX] = matrix[this.startY + mapSize / 2, this.startX] =
            (int)rand.Next((uint)this.minValue, (uint)(this.minValue + this.altitude / 2));
        matrix[this.startY, this.startX + mapSize] = matrix[this.startY, this.startX + mapSize / 2] =
            (int)rand.Next((uint)this.minValue, (uint)(this.minValue + this.altitude / 2));
        matrix[this.startY + mapSize, this.startX] = matrix[this.startY + mapSize, this.startX + mapSize / 2] =
            (int)rand.Next((uint)this.minValue, (uint)(this.minValue + this.altitude / 2));
        matrix[this.startY + mapSize, this.startX + mapSize] =
            matrix[this.startY + mapSize, this.startX + mapSize / 2] =
                (int)rand.Next((uint)this.minValue, (uint)(this.minValue + this.altitude / 2));
        matrix[this.startY + mapSize / 2, this.startX + mapSize / 2] = this.minValue + this.altitude;

        DiamondSquareAverage.CreateDiamondSquareAverage(matrix, this.startX, this.startY, (uint)mapSize / 4,
            (uint)mapSize / 4, (uint)mapSize / 4, matrix[this.startY, this.startX],
            matrix[this.startY + mapSize / 2, this.startX], matrix[this.startY, this.startX + mapSize / 2],
            matrix[this.startY + mapSize / 2, this.startX + mapSize / 2], this.minValue + this.altitude,
            this.addAltitude, rand, func);
        DiamondSquareAverage.CreateDiamondSquareAverage(matrix, this.startX, this.startY, (uint)mapSize / 4,
            (uint)mapSize * 3 / 4, (uint)mapSize / 4, matrix[this.startY + mapSize / 2, this.startX],
            matrix[this.startY + mapSize, this.startX], matrix[this.startY + mapSize / 2, this.startX + mapSize / 2],
            matrix[this.startY + mapSize, this.startX + mapSize / 2], this.minValue + this.altitude, this.addAltitude,
            rand, func);
        DiamondSquareAverage.CreateDiamondSquareAverage(matrix, this.startX, this.startY, (uint)mapSize * 3 / 4,
            (uint)mapSize / 4, (uint)mapSize / 4, matrix[this.startY, this.startX + mapSize / 2],
            matrix[this.startY + mapSize / 2, this.startX + mapSize / 2], matrix[this.startY, this.startX + mapSize],
            matrix[this.startY + mapSize / 2, this.startX + mapSize], this.minValue + this.altitude, this.addAltitude,
            rand, func);
        DiamondSquareAverage.CreateDiamondSquareAverage(matrix, this.startX, this.startY, (uint)mapSize * 3 / 4,
            (uint)mapSize * 3 / 4, (uint)mapSize / 4, matrix[this.startY + mapSize / 2, this.startX + mapSize / 2],
            matrix[this.startY + mapSize, this.startX + mapSize / 2],
            matrix[this.startY + mapSize / 2, this.startX + mapSize],
            matrix[this.startY + mapSize, this.startX + mapSize], this.minValue + this.altitude, this.addAltitude, rand,
            func);
    }

    /// <summary>
    /// 默认构造函数，使用基类默认设置初始化实例。
    /// </summary>
    public DiamondSquareAverageIsland()
    {
    }

    /// <summary>
    /// 使用起始坐标和大小初始化实例。
    /// </summary>
    /// <param name="startX">起始 X 坐标（列偏移）。</param>
    /// <param name="startY">起始 Y 坐标（行偏移）。</param>
    /// <param name="width">区域宽度。</param>
    /// <param name="height">区域高度。</param>
    public DiamondSquareAverageIsland(uint startX, uint startY, uint width, uint height) :
        base(startX, startY, width, height)
    {
    }

    /// <summary>
    /// 使用最小值初始化实例（minValue）。
    /// </summary>
    /// <param name="minValue">最低高度值或基准值。</param>
    public DiamondSquareAverageIsland(int minValue) : base(minValue)
    {
    }

    /// <summary>
    /// 使用最小值和高度（altitude）初始化实例。
    /// </summary>
    /// <param name="minValue">最低高度值或基准值。</param>
    /// <param name="altitude">高度范围或基准高度。</param>
    public DiamondSquareAverageIsland(int minValue, int altitude) : base(minValue, altitude)
    {
    }

    /// <summary>
    /// 使用最小值、高度和附加高度参数初始化实例。
    /// </summary>
    /// <param name="minValue">最低高度值或基准值。</param>
    /// <param name="altitude">高度范围或基准高度。</param>
    /// <param name="addAltitude">用于控制随机偏移的附加高度参数。</param>
    public DiamondSquareAverageIsland(int minValue, int altitude, int addAltitude) : base(minValue, altitude,
        addAltitude)
    {
    }

    /// <summary>
    /// 使用矩阵范围和最小值初始化实例。
    /// </summary>
    /// <param name="matrixRange">矩阵范围（Coordinate2DMatrix）。</param>
    /// <param name="minValue">最低高度值或基准值。</param>
    public DiamondSquareAverageIsland(MatrixRange matrixRange, int minValue) : base(matrixRange, minValue)
    {
    }

    /// <summary>
    /// 使用矩阵范围、最小值和高度初始化实例。
    /// </summary>
    /// <param name="matrixRange">矩阵范围（Coordinate2DMatrix）。</param>
    /// <param name="minValue">最低高度值或基准值。</param>
    /// <param name="altitude">高度范围或基准高度。</param>
    public DiamondSquareAverageIsland(MatrixRange matrixRange, int minValue, int altitude) : base(matrixRange, minValue,
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
    public DiamondSquareAverageIsland(MatrixRange matrixRange, int minValue, int altitude, int addAltitude) : base(
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
    public DiamondSquareAverageIsland(uint startX, uint startY, uint width, uint height, int minValue) :
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
    public DiamondSquareAverageIsland(uint startX, uint startY, uint width, uint height, int minValue, int altitude) :
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
    public DiamondSquareAverageIsland(uint startX, uint startY, uint width, uint height, int minValue, int altitude,
        int addAltitude) :
        base(startX, startY, width, height, minValue, altitude, addAltitude)
    {
    }
}