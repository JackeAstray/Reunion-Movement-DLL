using ReunionMovementDLL.Dungeon.Random;
using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Shape;
using ReunionMovementDLL.Dungeon.Util;
using System;
using System.Collections.Generic;
using System.Text;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Retouch
{
    /// <summary>
    /// 细胞自动机重绘器。
    /// 使用简单的四邻域规则根据邻居决定当前单元的值。
    /// </summary>
    public class CellularAutomaton : RectBase<CellularAutomaton>, IDrawer<int>
    {
        // 随机数生成器，用于在邻居不一致时随机选择一个邻居的值
        RandomBase rand = new RandomBase();

        /// <summary>
        /// 将细胞自动机应用到矩阵上。
        /// 成功时返回 true，并通过 out 参数返回日志（当前未使用）。
        /// </summary>
        /// <param name="matrix">目标整数矩阵</param>
        public bool Draw(int[,] matrix)
        {
            return DrawNormal(matrix);
        }

        public bool Draw(int[,] matrix, out string log)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 将位于指定列、行的单元根据其四个邻居的值进行赋值。
        /// 规则：若四个邻居都相同，则采用该相同值；否则随机选择四个邻居之一的值。
        /// </summary>
        /// <param name="matrix">目标矩阵（必须足够大以包含邻居）</param>
        /// <param name="col">列索引（uint）</param>
        /// <param name="row">行索引（uint）</param>
        private void Assign(int[,] matrix, uint col, uint row)
        {
            // 将索引转换成 int，避免在下标计算时产生多次强制转换
            int r = (int)row;
            int c = (int)col;

            // 缓存四个邻居的值，减少重复索引访问
            var left = matrix[r, c - 1];
            var right = matrix[r, c + 1];
            var up = matrix[r - 1, c];
            var down = matrix[r + 1, c];

            if (left == right && right == up && up == down)
            {
                matrix[r, c] = right;
                return;
            }

            // 如果邻居不一致，随机选择一个邻居的值
            switch (rand.Next(4))
            {
                case 0:
                    matrix[r, c] = left;
                    break;
                case 1:
                    matrix[r, c] = right;
                    break;
                case 2:
                    matrix[r, c] = up;
                    break;
                default:
                    matrix[r, c] = down;
                    break;
            }
        }

        /// <summary>
        /// 在矩形区域内依次对每个非边界单元执行细胞自动机规则。
        /// 优化点：先做空/null/尺寸检查并缓存矩阵尺寸；在 Assign 中缓存邻居以减少内存访问。
        /// </summary>
        /// <param name="matrix">目标整数矩阵</param>
        /// <returns>如果绘制成功返回 true；当矩阵为空或尺寸不足时返回 false</returns>
        private bool DrawNormal(int[,] matrix)
        {
            if (matrix == null)
                return false;

            var width = MatrixUtil.GetX(matrix);
            var height = MatrixUtil.GetY(matrix);

            // 矩阵必须至少为 3x3 才存在四邻域
            if (width < 3 || height < 3)
                return false;

            var endX = this.CalcEndX(width) - 1;
            var endY = this.CalcEndY(height) - 1;

            for (var row = this.startY + 1; row < endY; ++row)
            {
                for (var col = this.startX + 1; col < endX; ++col)
                {
                    Assign(matrix, col, row);
                }
            }

            return true;
        }

        /// <summary>
        /// 默认构造函数，创建一个未指定区域的细胞自动机实例。
        /// </summary>
        public CellularAutomaton()
        {
        }

        /// <summary>
        /// 使用指定的矩阵范围构造实例。
        /// </summary>
        /// <param name="matrixRange">矩阵坐标范围</param>
        public CellularAutomaton(MatrixRange matrixRange) : base(matrixRange)
        {
        }

        /// <summary>
        /// 使用起始坐标和宽高构造实例。
        /// </summary>
        /// <param name="startX">起始 X 坐标</param>
        /// <param name="startY">起始 Y 坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public CellularAutomaton(uint startX, uint startY, uint width, uint height) : base(startX, startY, width, height)
        {
        }
    }
}
