using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Util;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// 绘制矩形边框的绘制器，在指定矩形范围内将边缘单元设置为给定的值。
    /// </summary>
    public class Border : RectBaseWithValue<Border>, IDrawer<int>
    {
        /// <summary>
        /// 在给定矩阵上绘制边框（不返回日志）。
        /// 该方法使用内部范围信息计算要绘制的矩形区域并调用实际绘制逻辑。
        /// 返回true表示绘制成功。
        /// </summary>
        /// <param name="matrix">要绘制的二维整数矩阵。</param>
        public bool Draw(int[,] matrix)
        {
            return DrawNormal(matrix);
        }

        /// <summary>
        /// 在给定矩阵上绘制边框并输出日志（当前未实现）。
        /// 该重载目前抛出NotImplementedException，调用方不应依赖此方法获取日志。
        /// </summary>
        /// <param name="matrix">要绘制的矩阵。</param>
        /// <param name="log">输出的日志字符串（未实现）。</param>
        /// <returns>抛出NotImplementedException。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 在给定矩阵上创建（填充）边框并返回矩阵，同时输出日志（通过Draw重载）。
        /// 该方法会调用带日志的Draw方法来执行实际的绘制并返回原始矩阵引用。
        /// </summary>
        /// <param name="matrix">要填充并返回的矩阵。</param>
        /// <param name="log">输出的日志字符串（来自Draw）。</param>
        /// <returns>填充后的矩阵引用。</returns>
        public int[,] Create(int[,] matrix, out string log)
        {
            Draw(matrix, out log);
            return matrix;
        }

        /// <summary>
        /// 执行实际的边框绘制逻辑：计算矩阵的终点坐标并分别绘制上/下/左/右边缘。
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
                matrix[endY - 1, col] = this.drawValue;
            }

            for (var row = this.startY; row < endY; ++row)
            {
                matrix[row, startX] = this.drawValue;
                matrix[row, endX - 1] = this.drawValue;
            }
            return true;
        }

        /// <summary>
        /// 默认构造函数，创建一个未初始化值的边框绘制器。
        /// </summary>
        public Border() { } // = default();

        /// <summary>
        /// 使用初始绘制值和矩阵范围构造边框绘制器。
        /// </summary>
        /// <param name="drawValue">边框绘制使用的值。</param>
        /// <param name="matrixRange">指定矩形范围的矩阵范围对象。</param>
        public Border(int drawValue, MatrixRange matrixRange) : base(drawValue, matrixRange)
        {
            this.drawValue = drawValue;
        }

        /// <summary>
        /// 使用起始值和矩形参数构造边框绘制器。
        /// startX/startY为起始坐标，width/height为矩形宽高。
        /// </summary>
        /// <param name="drawValue">边框绘制使用的值。</param>
        /// <param name="startX">矩形区域起始X（列）索引。</param>
        /// <param name="startY">矩形区域起始Y（行）索引。</param>
        /// <param name="width">矩形区域宽度（列数）。</param>
        /// <param name="height">矩形区域高度（行数）。</param>
        public Border(int drawValue, uint startX, uint startY, uint width, uint height) : base(drawValue, startX, startY, width,
            height)
        {
            this.drawValue = drawValue;
        }

        /// <summary>
        /// 使用指定起始值构造边框绘制器，矩形范围需另行设置或使用默认范围。
        /// </summary>
        /// <param name="drawValue">边框绘制使用的值。</param>
        public Border(int drawValue) : base(drawValue)
        {
            this.drawValue = drawValue;
        }
    }
}