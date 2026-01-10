using ReunionMovementDLL.Dungeon.Random;
using System;
using System.Collections.Generic;
using System.Text;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Util
{
    /// <summary>
    /// Voronoi 图生成器，在给定区域内根据随机点产生分区并填充矩阵值。
    /// </summary>
    public class VoronoiDiagram
    {
        RandomBase rand = new RandomBase();

        public uint startX { get; set; }
        public uint startY { get; set; }
        public uint width { get; set; }
        public uint height { get; set; }
        public int drawValue { get; set; }

        /* Draw */

        /// <summary>
        /// 在矩阵上绘制 Voronoi 图。function_ 用于为每个随机点指定颜色/值。
        /// </summary>
        /// <param name="matrix">目标矩阵</param>
        /// <param name="function_">回调，用于设置点的颜色/值</param>
        /// <returns>成功返回 true</returns>
        public bool Draw(int[,] matrix, DungeonDelegate.VoronoiDiagramDelegate function_)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));
            if (function_ == null) throw new ArgumentNullException(nameof(function_));

            var lenX = MatrixUtil.GetX(matrix);
            var lenY = MatrixUtil.GetY(matrix);

            uint endX = this.width == 0 || this.startX + this.width >= ((lenX == 0) ? 0 : lenX)
                ? ((lenX == 0) ? 0u : lenX)
                : this.startX + this.width;
            uint endY = (this.height == 0 || this.startY + this.height >= lenY) ? lenY : this.startY + this.height;

            return this.DrawNormal(matrix, endX, endY, function_);
        }

        /// <summary>
        /// 将点生成与区域填充的主流程，返回是否成功。
        /// </summary>
        private bool DrawNormal(int[,] matrix, uint endX, uint endY, DungeonDelegate.VoronoiDiagramDelegate function)
        {
            this.AssignSTL(matrix, endX, endY, function);
            return true;
        }

        /// <summary>
        /// 内部名称调整为 PascalCase。生成随机点并创建站点。
        /// </summary>
        private void AssignSTL(int[,] matrix, uint endX, uint endY, DungeonDelegate.VoronoiDiagramDelegate function)
        {
            if (drawValue <= 0) return;

            Pair[] point = new Pair[drawValue];
            int[] color = new int[drawValue];
            CreatePoint(point, color, endX, endY, function);
            CreateSites(point, color, matrix, (int)endX, (int)endY);
        }

        /// <summary>
        /// 为所有像素选择最近的站点并把对应颜色写入矩阵。
        /// </summary>
        private void CreateSites(Pair[] point, int[] color, int[,] matrix, int w, int h)
        {
            if (point == null || color == null || matrix == null) return;

            for (int hh = 0; hh < h; ++hh)
            {
                for (int ww = 0; ww < w; ++ww)
                {
                    int ind = -1;
                    int bestDist = int.MaxValue;
                    for (var it = 0; it < point.Length; ++it)
                    {
                        int ds = distanceSqrd(point[it], ww, hh);
                        if (ds < bestDist)
                        {
                            bestDist = ds;
                            ind = it;
                        }
                    }

                    if (ind >= 0)
                        matrix[hh, ww] = color[ind];
                }
            }
        }

        /// <summary>
        /// 计算点到像素位置的平方距离。
        /// </summary>
        private int distanceSqrd(Pair pair, int x, int y)
        {
            x -= (int)pair.First;
            y -= (int)pair.Second;
            return x * x + y * y;
        }

        /// <summary>
        /// 在给定区域随机生成 drawValue 个点，并通过回调设置对应颜色/值。
        /// </summary>
        private void CreatePoint(Pair[] point, int[] color, uint w, uint h, DungeonDelegate.VoronoiDiagramDelegate function_)
        {
            for (int arrayNum = 0; arrayNum < this.drawValue; ++arrayNum)
            {
                point[arrayNum] = new Pair((int)rand.Next(w), (int)rand.Next(h));
                function_(ref point[arrayNum], ref color[arrayNum], startX, startY, w, h);
            }
        }

        /* Clear/Setter methods with fluent API */

        public VoronoiDiagram ClearPointX()
        {
            this.startX = 0;
            return this;
        }

        public VoronoiDiagram ClearPointY()
        {
            this.startY = 0;
            return this;
        }

        public VoronoiDiagram ClearWidth()
        {
            this.width = 0;
            return this;
        }

        public VoronoiDiagram ClearHeight()
        {
            this.height = 0;
            return this;
        }

        public VoronoiDiagram ClearValue()
        {
            this.drawValue = 0;
            return this;
        }

        public VoronoiDiagram ClearPoint()
        {
            this.startX = 0;
            this.startY = 0;
            return this;
        }

        public VoronoiDiagram ClearRange()
        {
            this.ClearPointX();
            this.ClearPointY();
            this.ClearWidth();
            this.ClearHeight();
            return this;
        }

        public VoronoiDiagram Clear()
        {
            this.ClearRange();
            this.ClearValue();
            return this;
        }

        public VoronoiDiagram SetRange(uint startX, uint startY, uint length)
        {
            this.startX = startX;
            this.startY = startY;
            this.width = length;
            this.height = length;
            return this;
        }

        public VoronoiDiagram SetRange(uint startX, uint startY, uint width, uint height)
        {
            this.startX = startX;
            this.startY = startY;
            this.width = width;
            this.height = height;
            return this;
        }

        /* Constructors */

        /// <summary>
        /// 默认构造函数，创建一个空的 VoronoiDiagram 实例。
        /// </summary>
        public VoronoiDiagram()
        {
        } // default

        /// <summary>
        /// 构造函数，指定生成点数量 drawValue。
        /// </summary>
        public VoronoiDiagram(int drawValue)
        {
            this.drawValue = drawValue;
        }

        /// <summary>
        /// 使用 Coordinate2DMatrix 初始化范围。
        /// </summary>
        public VoronoiDiagram(MatrixRange matrixRange)
        {
            this.startX = (uint)matrixRange.x;
            this.startY = (uint)matrixRange.y;
            this.width = (uint)matrixRange.w;
            this.height = (uint)matrixRange.h;
        }

        /// <summary>
        /// 使用范围和点数初始化。
        /// </summary>
        public VoronoiDiagram(MatrixRange matrixRange, int drawValue)
        {
            this.startX = (uint)matrixRange.x;
            this.startY = (uint)matrixRange.y;
            this.width = (uint)matrixRange.w;
            this.height = (uint)matrixRange.h;
            this.drawValue = drawValue;
        }

        /// <summary>
        /// 使用起点与尺寸初始化。
        /// </summary>
        public VoronoiDiagram(uint startX, uint startY, uint width, uint height)
        {
            this.startX = startX;
            this.startY = startY;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// 使用起点、尺寸与点数初始化。
        /// </summary>
        public VoronoiDiagram(uint startX, uint startY, uint width, uint height, int drawValue)
        {
            this.startX = startX;
            this.startY = startY;
            this.width = width;
            this.height = height;
            this.drawValue = drawValue;
        }
    }
}
