using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Util;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// 绘制矩形边框（含奇数处理）的绘制器：在指定矩形范围内将边缘单元设置为给定的值，
    /// 并针对偶数高度或宽度在内侧再绘制一条线以保证视觉上的“厚度”或对称性。
    /// </summary>
    public class BorderOdd : RectBaseWithValue<BorderOdd>, IDrawer<int>
    {
        /// <summary>
        /// 在给定矩阵上绘制边框（不返回日志）。
        /// 此方法会调用实际的绘制实现DrawNormal并返回其结果。
        /// </summary>
        /// <param name="matrix">要绘制的二维整数矩阵。</param>
        public bool Draw(int[,] matrix)
        {
            return DrawNormal(matrix);
        }

        /// <summary>
        /// 在给定矩阵上绘制边框并输出日志（当前未实现）。
        /// 该重载目前抛出NotImplementedException，调用方不应依赖此方法获取日志信息。
        /// </summary>
        /// <param name="matrix">要绘制的矩阵。</param>
        /// <param name="log">输出的日志字符串（未实现）。</param>
        /// <returns>抛出NotImplementedException。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 执行实际的边框绘制逻辑：计算矩阵的终点坐标并分别绘制上/下/左/右边缘。
        /// 如果矩形的高度或宽度为偶数，会在对应边缘的内侧再绘制一条线以保证边框对称（即'奇数'处理逻辑）。
        /// 如果计算得到的终点不大于起点，方法将认为无需绘制并直接返回true。
        /// </summary>
        /// <param name="matrix">要绘制的二维矩阵。</param>
        /// <returns>表示绘制是否成功的布尔值。</returns>
        public bool DrawNormal(int[,] matrix)
        {
            var endX = this.CalcEndX(MatrixUtil.GetX(matrix));
            var endY = this.CalcEndY(MatrixUtil.GetY(matrix));
            if (endX <= startX || endY <= this.startY) return true;
            for (var col = startX; col < endX; ++col)
            {
                matrix[this.startY, col] = this.drawValue;
                if ((endY - this.startY) % 2 == 0) matrix[endY - 2, col] = this.drawValue;
                matrix[endY - 1, col] = this.drawValue;
            }

            for (var row = this.startY; row < endY; ++row)
            {
                matrix[row, startX] = this.drawValue;
                if ((endX - this.startX) % 2 == 0) matrix[row, endX - 2] = this.drawValue;
                matrix[row, endX - 1] = this.drawValue;
            }

            return true;
        }

        /// <summary>
        /// 默认构造函数，创建一个未初始化值的BorderOdd实例。
        /// </summary>
        public BorderOdd()
        {
        }

        /// <summary>
        /// 使用初始绘制值和矩阵范围构造BorderOdd实例。
        /// </summary>
        /// <param name="drawValue">边框绘制使用的值。</param>
        /// <param name="matrixRange">指定矩形范围的矩阵范围对象。</param>
        public BorderOdd(int drawValue, MatrixRange matrixRange) : base(drawValue, matrixRange)
        {
            this.drawValue = drawValue;
        }

        /// <summary>
        /// 使用起始值和矩形参数构造BorderOdd实例。
        /// startX/startY为起始坐标，width/height为矩形宽高。
        /// </summary>
        /// <param name="drawValue">边框绘制使用的值。</param>
        /// <param name="startX">矩形区域起始X（列）索引。</param>
        /// <param name="startY">矩形区域起始Y（行）索引。</param>
        /// <param name="width">矩形区域宽度（列数）。</param>
        /// <param name="height">矩形区域高度（行数）。</param>
        public BorderOdd(int drawValue, uint startX, uint startY, uint width, uint height) : base(drawValue, startX,
            startY, width, height)
        {
            this.drawValue = drawValue;
        }

        /// <summary>
        /// 使用指定起始值构造BorderOdd实例，矩形范围需另行设置或使用默认范围。
        /// </summary>
        /// <param name="drawValue">边框绘制使用的值。</param>
        public BorderOdd(int drawValue) : base(drawValue)
        {
            this.drawValue = drawValue;
        }
    }
}