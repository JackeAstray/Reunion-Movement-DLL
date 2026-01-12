using System.Collections.Generic;
using ReunionMovementDLL.Dungeon.Random;
using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Util;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;


namespace ReunionMovementDLL.Dungeon.Shape
{

    /// <summary>
    /// 半混合矩形：在矩阵的指定矩形区域内，以一定概率在两类取值中混合绘制。
    /// </summary>
    public class HalfMixRect : RectBaseWithIList<HalfMixRect>, IDrawer<int>
    {
        /// <summary>
        /// 随机数生成器，用于决定每个单元格采用哪种绘制值。
        /// </summary>
        RandomBase rand = new RandomBase();

        /// <summary>
        /// 将当前形状绘制到目标矩阵。该方法会调用内部的绘制实现（DrawNormal）。
        /// </summary>
        /// <param name="matrix">目标二维矩阵，行列索引为 [row, col]。</param>
        /// <returns>绘制是否成功，始终返回 true（当前实现）。</returns>
        public bool Draw(int[,] matrix)
        {
            return this.DrawNormal(matrix);
        }

        /// <summary>
        /// 带日志输出的绘制方法（未实现）。
        /// </summary>
        /// <param name="matrix">目标二维矩阵。</param>
        /// <param name="log">输出日志字符串（未实现）。</param>
        /// <returns>抛出 <see cref="System.NotImplementedException"/> 表示该方法尚未实现。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 标准绘制实现：在矩形区域内遍历每个单元格，以 50% 概率使用 drawValue[0]，否则从 drawValue 的其余元素中随机选择一个值进行填充。
        /// 计算绘制区域时会根据传入矩阵的尺寸校正矩形的结束坐标。
        /// </summary>
        /// <param name="matrix">目标二维矩阵。</param>
        /// <returns>绘制是否成功，当前实现总是返回 true。</returns>
        private bool DrawNormal(int[,] matrix)
        {
            uint drawValueCount = (uint)this.drawValue.Count;
            var endX = this.CalcEndX(MatrixUtil.GetX(matrix));
            var endY = this.CalcEndY(MatrixUtil.GetY(matrix));
            for (var row = this.startY; row < endY; ++row)
                for (var col = this.startX; col < endX; ++col)
                    matrix[row, col]
                        = this.rand.Probability(0.5) ?
                        this.drawValue[0] : this.drawValue[(int)rand.Next(drawValueCount)];
            return true;
        }

        /// <summary>
        /// 默认构造函数，创建一个空的半混合矩形实例。
        /// </summary>
        public HalfMixRect() { } // default

        /// <summary>
        /// 使用指定的绘制值列表构造实例。
        /// </summary>
        /// <param name="drawValue">用于绘制的值列表，索引 0 被视为优先值（50% 概率）。</param>
        public HalfMixRect(IList<int> drawValue) : base(drawValue)
        {
        }

        /// <summary>
        /// 使用指定的矩阵范围和绘制值列表构造实例。
        /// </summary>
        /// <param name="matrixRange">定义矩形在矩阵中的坐标范围。</param>
        /// <param name="drawValue">用于绘制的值列表。</param>
        public HalfMixRect(MatrixRange matrixRange, IList<int> drawValue) : base(matrixRange, drawValue)
        {
        }
    }
}