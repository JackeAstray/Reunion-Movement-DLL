using System;
using ReunionMovementDLL.Dungeon.Random;
using ReunionMovementDLL.Dungeon.Util;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// 随机矩形绘制器：在指定的矩形区域内按照一定概率将指定的值写入矩阵。
    /// 支持只指定起点/高度或同时指定宽度/高度的多种绘制方式，也支持使用判定函数进行条件绘制。
    /// </summary>
    public class RandomRect : IDrawer<int>
    {
        /// <summary>
        /// 用于概率判定的随机数生成器。
        /// </summary>
        private RandomBase randBase = new RandomBase();

        /// <summary>
        /// 绘制区域的起始 X 坐标（列）。
        /// </summary>
        public uint startX { get; set; }

        /// <summary>
        /// 绘制区域的起始 Y 坐标（行）。
        /// </summary>
        public uint startY { get; set; }

        /// <summary>
        /// 绘制区域的宽度（若为 0 则表示绘制到矩阵右边界）。
        /// </summary>
        public uint width { get; set; }

        /// <summary>
        /// 绘制区域的高度（若为 0 则表示绘制到矩阵下边界）。
        /// </summary>
        public uint height { get; set; }

        /// <summary>
        /// 要写入矩阵的值（瓦片 ID）。
        /// </summary>
        public int drawValue { get; set; }

        /// <summary>
        /// 写入的概率值（取值范围 0.0 - 1.0），默认为 0.5。
        /// </summary>
        private double probabilityValue = 0.5;

        /// <summary>
        /// 将值随机绘制到目标矩阵（不返回日志）。
        /// 根据 width/height 的设置决定绘制方式：若 width==0 使用整行绘制，否则限制到指定宽度。
        /// </summary>
        /// <param name="matrix">目标二维整型矩阵。</param>
        /// <returns>绘制是否成功（当前实现总是返回 true，除非抛出异常）。</returns>
        public bool Draw(int[,] matrix)
        {
            return (this.width == 0)
                ? this.DrawSTL(matrix,
                    (this.height == 0 || this.startY + this.height >= MatrixUtil.GetY(matrix))
                        ? MatrixUtil.GetY(matrix)
                        : this.startY + this.height)
                : this.DrawWidthSTL(matrix, this.startX + this.width,
                    (this.height == 0 || this.startY + this.height >= MatrixUtil.GetY(matrix))
                        ? MatrixUtil.GetY(matrix)
                        : this.startY + this.height);
        }

        /// <summary>
        /// 带日志输出的绘制方法（未实现）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="log">输出日志（out）。</param>
        /// <returns>抛出 NotImplementedException 表示该方法尚未实现。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 使用判定函数并按概率进行绘制的入口方法。
        /// 当 width==0 时绘制到矩阵右边界，否则在指定宽度内绘制。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="func">用于判断是否允许写入的函数，参数为当前单元格的值，返回 true 表示允许覆盖。</param>
        /// <returns>绘制是否成功。</returns>
        public bool DrawOperator(int[,] matrix, Func<int, bool> func)
        {
            return (this.width == 0)
                ? this.DrawSTL(matrix,
                    (this.height == 0 || this.startY + this.height >= MatrixUtil.GetY(matrix))
                        ? MatrixUtil.GetY(matrix)
                        : this.startY + this.height, func)
                : this.DrawWidthSTL(matrix, this.startX + this.width,
                    (this.height == 0 || this.startY + this.height >= MatrixUtil.GetY(matrix))
                        ? MatrixUtil.GetY(matrix)
                        : this.startY + this.height, func);
        }

        /// <summary>
        /// 在目标矩阵上绘制并返回该矩阵引用（便捷方法）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <returns>已绘制的矩阵引用。</returns>
        public int[,] Create(int[,] matrix)
        {
            this.Draw(matrix);
            return matrix;
        }

        /// <summary>
        /// 使用判定函数绘制并返回该矩阵引用（便捷方法）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="func">判定函数。</param>
        /// <returns>已绘制的矩阵引用。</returns>
        public int[,] Create(int[,] matrix, Func<int, bool> func)
        {
            this.DrawOperator(matrix, func);
            return matrix;
        }

        /// <summary>
        /// 绘制到指定的结束行（不限制列宽），基于 startX 起始列向右直到行宽结束。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="endY">结束行（不包含）。</param>
        /// <returns>绘制是否成功。</returns>
        private bool DrawSTL(int[,] matrix, uint endY)
        {
            for (var row = this.startY; row < endY; ++row)
                for (var col = this.startX; col < MatrixUtil.GetX(matrix, (int)row); ++col)
                    this.AssignSTL(matrix, col, row);
            return true;
        }

        /// <summary>
        /// 绘制到指定的结束行和结束列（限制宽度）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="endX">结束列（不包含）。</param>
        /// <param name="endY">结束行（不包含）。</param>
        /// <returns>绘制是否成功。</returns>
        private bool DrawWidthSTL(int[,] matrix, uint endX, uint endY)
        {
            for (var row = this.startY; row < endY; ++row)
                for (var col = this.startX; col < MatrixUtil.GetX(matrix, (int)row) && col < endX; ++col)
                    this.AssignSTL(matrix, col, row);
            return true;
        }

        /// <summary>
        /// 使用判定函数绘制到指定结束行（不限制列宽）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="endY">结束行（不包含）。</param>
        /// <param name="func">判定函数，返回 true 表示允许覆盖当前单元格。</param>
        /// <returns>绘制是否成功。</returns>
        private bool DrawSTL(int[,] matrix, uint endY, Func<int, bool> func)
        {
            for (var row = this.startY; row < endY; ++row)
                for (var col = this.startX; col < MatrixUtil.GetX(matrix, (int)row); ++col)
                    this.AssignSTL(matrix, col, row, func);
            return true;
        }

        /// <summary>
        /// 使用判定函数绘制到指定结束行和结束列（限制宽度）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="endX">结束列（不包含）。</param>
        /// <param name="endY">结束行（不包含）。</param>
        /// <param name="func">判定函数。</param>
        /// <returns>绘制是否成功。</returns>
        private bool DrawWidthSTL(int[,] matrix, uint endX, uint endY, Func<int, bool> func)
        {
            for (var row = this.startY; row < endY; ++row)
                for (var col = this.startX; col < MatrixUtil.GetX(matrix, (int)row) && col < endX; ++col)
                    this.AssignSTL(matrix, col, row, func);
            return true;
        }


        /// <summary>
        /// 根据内部概率在指定位置写入 drawValue（不带判定函数）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="endX">列索引。</param>
        /// <param name="endY">行索引。</param>
        private void AssignSTL(int[,] matrix, uint endX, uint endY)
        {
            if (randBase.Probability(probabilityValue)) matrix[endY, endX] = this.drawValue;
        }

        /// <summary>
        /// 在满足判定函数且通过概率判定时写入 drawValue。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="endX">列索引。</param>
        /// <param name="endY">行索引。</param>
        /// <param name="func">判定函数。</param>
        private void AssignSTL(int[,] matrix, uint endX, uint endY, Func<int, bool> func)
        {
            if (func(matrix[endY, endX]) && randBase.Probability(probabilityValue)) matrix[endY, endX] = this.drawValue;
        }

        /// <summary>
        /// 默认构造函数。
        /// </summary>
        public RandomRect() { }

        /// <summary>
        /// 使用绘制值初始化的构造函数。
        /// </summary>
        /// <param name="drawValue">要写入的值。</param>
        public RandomRect(int drawValue)
        {
            this.drawValue = drawValue;
        }

        /// <summary>
        /// 使用绘制值与概率值初始化的构造函数。
        /// </summary>
        public RandomRect(int drawValue, double probabilityValue)
        {
            this.drawValue = drawValue;
            this.probabilityValue = probabilityValue;
        }


        /// <summary>
        /// 使用矩阵范围初始化的构造函数（从 MatrixRange 中读取起点与尺寸）。
        /// </summary>
        /// <param name="matrixRange">矩阵范围对象。</param>
        public RandomRect(MatrixRange matrixRange)
        {
            this.startX = (uint)matrixRange.x;
            this.startY = (uint)matrixRange.y;
            this.width = (uint)matrixRange.w;
            this.height = (uint)matrixRange.h;
        }

        /// <summary>
        /// 使用矩阵范围与绘制值初始化的构造函数。
        /// </summary>
        public RandomRect(MatrixRange matrixRange, int drawValue)
        {
            this.startX = (uint)matrixRange.x;
            this.startY = (uint)matrixRange.y;
            this.width = (uint)matrixRange.w;
            this.height = (uint)matrixRange.h;
            this.drawValue = drawValue;
        }

        /// <summary>
        /// 使用矩阵范围、绘制值与概率值初始化的构造函数。
        /// </summary>
        public RandomRect(MatrixRange matrixRange, int drawValue, double probabilityValue)
        {
            this.startX = (uint)matrixRange.x;
            this.startY = (uint)matrixRange.y;
            this.width = (uint)matrixRange.w;
            this.height = (uint)matrixRange.h;
            this.drawValue = drawValue;
            this.probabilityValue = probabilityValue;
        }

        /// <summary>
        /// 使用起始坐标与尺寸初始化的构造函数。
        /// </summary>
        public RandomRect(uint endX, uint endY, uint width, uint height)
        {
            this.startX = endX;
            this.startY = endY;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// 使用起始坐标、尺寸与绘制值初始化的构造函数。
        /// </summary>
        public RandomRect(uint endX, uint endY, uint width, uint height, int drawValue)
        {
            this.startX = endX;
            this.startY = endY;
            this.width = width;
            this.height = height;
            this.drawValue = drawValue;
        }

        /// <summary>
        /// 使用起始坐标、尺寸、绘制值与概率值初始化的构造函数。
        /// </summary>
        public RandomRect(uint endX, uint endY, uint width, uint height, int drawValue, double probabilityValue)
        {
            this.startX = endX;
            this.startY = endY;
            this.width = width;
            this.height = height;
            this.drawValue = drawValue;
            this.probabilityValue = probabilityValue;
        }
    }
}