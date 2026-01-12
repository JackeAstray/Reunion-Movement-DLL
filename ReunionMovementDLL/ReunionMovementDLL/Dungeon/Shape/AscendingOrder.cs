using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Util;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// 升序
    /// </summary>
    public class AscendingOrder : RectBaseWithValue<AscendingOrder>, IDrawer<int>
    {
        /// <summary>
        /// 在给定的矩阵区域内按行优先顺序升序填充整数值（使用内部的起始值和矩阵范围）。
        /// 成功返回true。
        /// </summary>
        /// <param name="matrix">要填充的二维整数矩阵。</param>
        public bool Draw(int[,] matrix)
        {
            return DrawNormal(matrix);
        }

        /// <summary>
        /// 在给定的矩阵上绘制，并输出日志信息（未实现）。
        /// 目前抛出NotImplementedException，调用方不应依赖此重载。
        /// </summary>
        /// <param name="matrix">要绘制的矩阵。</param>
        /// <param name="log">输出日志字符串（当前未实现）。</param>
        /// <returns>抛出NotImplementedException。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            // TODO: 如果需要日志功能，可在此实现并返回对应的日志信息。
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 在给定矩阵上创建（填充）升序数据并返回该矩阵。
        /// 该方法会调用Draw来执行实际的填充。
        /// </summary>
        /// <param name="matrix">要填充并返回的矩阵。</param>
        /// <returns>填充后的矩阵引用。</returns>
        public int[,] Create(int[,] matrix)
        {
            this.Draw(matrix);
            return matrix;
        }

        /// <summary>
        /// 执行实际的升序填充逻辑：按行从左到右、从上到下填充数值。
        /// 使用对象的drawValue作为起始值，并根据矩阵和当前矩形范围计算终点位置。
        /// 返回true表示绘制成功。
        /// </summary>
        /// <param name="matrix">要填充的二维矩阵。</param>
        /// <returns>表示绘制是否成功的布尔值。</returns>
        private bool DrawNormal(int[,] matrix)
        {
            var value = this.drawValue;
            var endX = this.CalcEndX(MatrixUtil.GetX(matrix));
            var endY = this.CalcEndY(MatrixUtil.GetY(matrix));
            for (var row = startY; row < endY; ++row)
                for (var col = startX; col < endX; ++col, value++)
                    matrix[row, col] = value;

            return true;
        }

        /// <summary>
        /// 默认构造函数，创建一个未初始化值的升序绘制器。
        /// </summary>
        public AscendingOrder() { }

        /// <summary>
        /// 使用初始绘制值和矩阵范围构造升序绘制器。
        /// </summary>
        /// <param name="drawValue">起始填充值。</param>
        /// <param name="matrixRange">矩阵范围，用于指定要填充的矩形区域。</param>
        public AscendingOrder(int drawValue, MatrixRange matrixRange) : base(drawValue, matrixRange)
        {
            this.drawValue = drawValue;
        }

        /// <summary>
        /// 使用起始值和矩形参数构造升序绘制器。
        /// startX/startY为起始坐标，width/height为矩形宽高。
        /// </summary>
        /// <param name="drawValue">起始填充值。</param>
        /// <param name="startX">矩形区域起始X（列）索引。</param>
        /// <param name="startY">矩形区域起始Y（行）索引。</param>
        /// <param name="width">矩形区域宽度（列数）。</param>
        /// <param name="height">矩形区域高度（行数）。param>
        public AscendingOrder(int drawValue, uint startX, uint startY, uint width, uint height) : base(drawValue, startX, startY, width,
            height)
        {
            this.drawValue = drawValue;
        }

        /// <summary>
        /// 使用指定起始值构造升序绘制器，矩形范围需另行设置或使用默认范围。
        /// </summary>
        /// <param name="drawValue">起始填充值。</param>
        public AscendingOrder(int drawValue) : base(drawValue)
        {
            this.drawValue = drawValue;
        }
    }
}