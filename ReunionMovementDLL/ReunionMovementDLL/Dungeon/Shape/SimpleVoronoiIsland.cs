using ReunionMovementDLL.Dungeon.Random;
using ReunionMovementDLL.Dungeon.Util;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// 简单的Voronoi岛屿绘制器：使用Voronoi图生成岛屿形状并在矩阵上绘制陆地与海洋值。
    /// </summary>
    public class SimpleVoronoiIsland : IDrawer<int>
    {
        /// <summary>
        /// 随机数生成器，用于概率判断。
        /// </summary>
        private RandomBase rand = new RandomBase();

        /// <summary>
        /// 内部使用的Voronoi图对象。
        /// </summary>
        private VoronoiDiagram voronoiDiagram;

        /// <summary>
        /// 陆地的值（在矩阵中表示陆地的整数）。
        /// </summary>
        public int landValue { get; protected set; }

        /// <summary>
        /// 海洋的值（在矩阵中表示海洋的整数）。
        /// </summary>
        public int seaValue { get; protected set; }

        /// <summary>
        /// 将某个Voronoi单元标记为陆地时使用的概率值（0.0 - 1.0）。
        /// </summary>
        public double probability { get; protected set; }

        /// <summary>
        /// 判断给定点是否位于指定范围的岛屿内部。
        /// </summary>
        /// <param name="point_">要判断的点（Pair）。</param>
        /// <param name="sx_">区域起始X坐标。</param>
        /// <param name="sy_">区域起始Y坐标。</param>
        /// <param name="w_">区域宽度（右边界坐标）。</param>
        /// <param name="h_">区域高度（下边界坐标）。</param>
        /// <param name="numerator_">分子，用于计算内边距比率。</param>
        /// <param name="denominator_">分母，用于计算内边距比率。</param>
        /// <returns>如果点位于岛屿内部返回true，否则返回false。</returns>
        bool isIsland(Pair point_, uint sx_, uint sy_, uint w_, uint h_, uint numerator_, uint denominator_)
        {
            return (int)point_.First > ((w_ - sx_) * numerator_ / denominator_ + sx_) &&
                   (int)point_.First < ((w_ - sx_) * (denominator_ - numerator_) / denominator_ + sx_) &&
                   (int)point_.Second > ((h_ - sy_) * numerator_ / denominator_ + sy_) &&
                   (int)point_.Second < ((h_ - sy_) * (denominator_ - numerator_) / denominator_ + sy_);
        }

        /// <summary>
        /// 在给定矩阵上绘制Voronoi岛屿：由内部Voronoi图决定每个单元的颜色（陆地或海洋）。
        /// </summary>
        /// <param name="matrix">目标绘制矩阵（二维整型数组）。</param>
        /// <returns>如果绘制成功返回true，否则返回false（由内部VoronoiDiagram.Draw决定）。</returns>
        public bool Draw(int[,] matrix)
        {
            DungeonDelegate.VoronoiDiagramDelegate voronoiDiagramDelegate =
                (ref Pair point, ref int color, uint startX, uint startY, uint w, uint h) =>
                {
                    if ((this.isIsland(point, startX, startY, w, h, 2, 5) ||
                         this.isIsland(point, startX, startY, w, h, 1, 5)) &&
                        rand.Probability((this.probability)))
                        color = this.landValue;
                    else
                        color = this.seaValue;
                };

            return this.voronoiDiagram.Draw(matrix, voronoiDiagramDelegate);
        }

        /// <summary>
        /// 绘制并返回日志（未实现）。
        /// </summary>
        /// <param name="matrix">目标绘制矩阵。</param>
        /// <param name="log">输出的日志字符串（未实现）。</param>
        /// <returns>目前抛出NotImplementedException。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 在给定矩阵上生成（Create）并返回该矩阵的引用，实际调用Draw进行绘制。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <returns>返回被绘制过的矩阵。</returns>
        public int[,] Create(int[,] matrix)
        {
            this.Draw(matrix);
            return matrix;
        }

        /* Getter */
        /// <summary>
        /// 获取内部Voronoi图的起始X坐标。
        /// </summary>
        public uint GetPointX()
        {
            return voronoiDiagram.startX;
        }

        /// <summary>
        /// 获取内部Voronoi图的起始Y坐标。
        /// </summary>
        public uint GetPointY()
        {
            return voronoiDiagram.startY;
        }

        /// <summary>
        /// 获取内部Voronoi图的宽度。
        /// </summary>
        public uint GetWidth()
        {
            return voronoiDiagram.width;
        }

        /// <summary>
        /// 获取内部Voronoi图的高度。
        /// </summary>
        public uint GetHeight()
        {
            return voronoiDiagram.height;
        }

        /// <summary>
        /// 获取用于绘制的值（drawValue）。
        /// </summary>
        public int GetValue()
        {
            return voronoiDiagram.drawValue;
        }

        /* Setter */
        /// <summary>
        /// 设置内部Voronoi图的起始X坐标。
        /// </summary>
        /// <param name="value">新的起始X值。</param>
        /// <returns>返回当前实例以便链式调用。</returns>
        public SimpleVoronoiIsland SetPointX(uint value)
        {
            this.voronoiDiagram.startX = value;
            return this;
        }

        /// <summary>
        /// 设置内部Voronoi图的起始Y坐标。
        /// </summary>
        /// <param name="value">新的起始Y值。</param>
        /// <returns>返回当前实例以便链式调用。</returns>
        public SimpleVoronoiIsland SetPointY(uint value)
        {
            voronoiDiagram.startY = value;
            return this;
        }

        /// <summary>
        /// 设置内部Voronoi图的宽度。
        /// </summary>
        /// <param name="value">新的宽度值。</param>
        /// <returns>当前实例（链式）。</returns>
        public SimpleVoronoiIsland SetWidth(uint value)
        {
            voronoiDiagram.width = value;
            return this;
        }

        /// <summary>
        /// 设置内部Voronoi图的高度。
        /// </summary>
        /// <param name="value">新的高度值。</param>
        /// <returns>当前实例（链式）。</returns>
        public SimpleVoronoiIsland SetHeight(uint value)
        {
            voronoiDiagram.height = value;
            return this;
        }

        /// <summary>
        /// 同时设置起始X和起始Y为相同值（便捷方法）。
        /// </summary>
        /// <param name="value">用于X和Y的同一值。</param>
        /// <returns>当前实例（链式）。</returns>
        public SimpleVoronoiIsland SetPoint(uint value)
        {
            voronoiDiagram.startX = value;
            voronoiDiagram.startY = value;
            return this;
        }

        /// <summary>
        /// 设置起始X和起始Y为指定值。
        /// </summary>
        /// <param name="startX">起始X。</param>
        /// <param name="startY">起始Y。</param>
        /// <returns>当前实例（链式）。</returns>
        public SimpleVoronoiIsland SetPoint(uint startX, uint startY)
        {
            voronoiDiagram.startX = startX;
            voronoiDiagram.startY = startY;
            return this;
        }

        /// <summary>
        /// 设置Voronoi图的范围（起始坐标与宽高）。
        /// </summary>
        /// <param name="startX">起始X。</param>
        /// <param name="startY">起始Y。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <returns>当前实例（链式）。</returns>
        public SimpleVoronoiIsland SetRange(uint startX, uint startY, uint width, uint height)
        {
            voronoiDiagram.startX = startX;
            voronoiDiagram.startY = startY;
            voronoiDiagram.width = width;
            voronoiDiagram.height = height;
            return this;
        }

        /// <summary>
        /// 使用MatrixRange设置Voronoi图范围。
        /// </summary>
        /// <param name="matrixRange">包含x,y,w,h的矩形范围。</param>
        /// <returns>当前实例（链式）。</returns>
        public SimpleVoronoiIsland SetRange(MatrixRange matrixRange)
        {
            voronoiDiagram.startX = (uint)matrixRange.x;
            voronoiDiagram.startY = (uint)matrixRange.y;
            voronoiDiagram.width = (uint)matrixRange.w;
            voronoiDiagram.height = (uint)matrixRange.h;
            return this;
        }


        /* Clear */
        /// <summary>
        /// 清除起始X设置，恢复为默认（由VoronoiDiagram处理）。
        /// </summary>
        /// <returns>当前实例（链式）。</returns>
        SimpleVoronoiIsland ClearPointX()
        {
            this.voronoiDiagram.ClearPointX();
            return this;
        }

        /// <summary>
        /// 清除起始Y设置，恢复为默认（由VoronoiDiagram处理）。
        /// </summary>
        /// <returns>当前实例（链式）。</returns>
        SimpleVoronoiIsland ClearPointY()
        {
            this.voronoiDiagram.ClearPointY();
            return this;
        }

        /// <summary>
        /// 清除宽度设置，恢复为默认（由VoronoiDiagram处理）。
        /// </summary>
        /// <returns>当前实例（链式）。</returns>
        SimpleVoronoiIsland ClearWidth()
        {
            this.voronoiDiagram.ClearWidth();
            return this;
        }

        /// <summary>
        /// 清除高度设置，恢复为默认（由VoronoiDiagram处理）。
        /// </summary>
        /// <returns>当前实例（链式）。</returns>
        SimpleVoronoiIsland ClearHeight()
        {
            this.voronoiDiagram.ClearHeight();
            return this;
        }

        /// <summary>
        /// 清除绘制值（注意：当前实现错误地调用了ClearHeight，这里保留行为以兼容原实现）。
        /// </summary>
        /// <returns>当前实例（链式）。</returns>
        SimpleVoronoiIsland ClearValue()
        {
            this.voronoiDiagram.ClearHeight();
            return this;
        }

        /// <summary>
        /// 清除起始点（X和Y）。
        /// </summary>
        /// <returns>当前实例（链式）。</returns>
        SimpleVoronoiIsland ClearPoint()
        {
            this.ClearPointX();
            this.ClearPointY();
            return this;
        }

        /// <summary>
        /// 清除范围设置（起始点、宽度与高度）。
        /// </summary>
        /// <returns>当前实例（链式）。</returns>
        SimpleVoronoiIsland ClearRange()
        {
            this.ClearPointX();
            this.ClearPointY();
            this.ClearWidth();
            this.ClearHeight();
            return this;
        }

        /// <summary>
        /// 清除所有设置（范围与绘制值）。
        /// </summary>
        /// <returns>当前实例（链式）。</returns>
        SimpleVoronoiIsland Clear()
        {
            this.ClearRange();
            this.ClearValue();
            return this;
        }

        /* Constructors */
        /// <summary>
        /// 默认构造函数：创建一个默认配置的Voronoi图实例。
        /// </summary>
        public SimpleVoronoiIsland()
        {
            voronoiDiagram = new VoronoiDiagram();
        } // default

        /// <summary>
        /// 使用指定的Voronoi点数量构造。
        /// </summary>
        /// <param name="voronoiNum">Voronoi点的数量。</param>
        public SimpleVoronoiIsland(int voronoiNum)
        {
            voronoiDiagram = new VoronoiDiagram(voronoiNum);
        }

        /// <summary>
        /// 使用Voronoi点数量和陆地概率构造。
        /// </summary>
        /// <param name="voronoiNum">Voronoi点的数量。</param>
        /// <param name="probabilityValue">陆地生成的概率（0.0 - 1.0）。</param>
        public SimpleVoronoiIsland(int voronoiNum, double probabilityValue)
        {
            voronoiDiagram = new VoronoiDiagram(voronoiNum);
            this.probability = probabilityValue;
        }

        /// <summary>
        /// 使用Voronoi点数量、陆地概率与陆地值构造。
        /// </summary>
        /// <param name="voronoiNum">Voronoi点的数量。</param>
        /// <param name="probabilityValue">陆地生成的概率。</param>
        /// <param name="landValue">陆地对应的矩阵值。</param>
        public SimpleVoronoiIsland(int voronoiNum, double probabilityValue, int landValue)
        {
            voronoiDiagram = new VoronoiDiagram(voronoiNum);
            this.probability = probabilityValue;
            this.landValue = landValue;
        }

        /// <summary>
        /// 使用Voronoi点数量、陆地概率、陆地值与海洋值构造。
        /// </summary>
        /// <param name="voronoiNum">Voronoi点的数量。</param>
        /// <param name="probabilityValue">陆地生成的概率。</param>
        /// <param name="landValue">陆地对应的矩阵值。</param>
        /// <param name="seaValue">海洋对应的矩阵值。</param>
        public SimpleVoronoiIsland(int voronoiNum, double probabilityValue, int landValue, int seaValue)
        {
            voronoiDiagram = new VoronoiDiagram(voronoiNum);
            this.probability = probabilityValue;
            this.landValue = landValue;
            this.seaValue = seaValue;
        }

        /// <summary>
        /// 使用矩阵范围构造，范围由MatrixRange提供。
        /// </summary>
        /// <param name="matrixRange">矩阵范围（x,y,w,h）。</param>
        public SimpleVoronoiIsland(MatrixRange matrixRange)
        {
            voronoiDiagram = new VoronoiDiagram(matrixRange);
        }

        /// <summary>
        /// 使用矩阵范围与Voronoi点数量构造。
        /// </summary>
        /// <param name="matrixRange">矩阵范围。</param>
        /// <param name="voronoiNum">Voronoi点数量。</param>
        public SimpleVoronoiIsland(MatrixRange matrixRange, int voronoiNum)
        {
            voronoiDiagram = new VoronoiDiagram(matrixRange, voronoiNum);
        }

        /// <summary>
        /// 使用矩阵范围、Voronoi点数量与陆地概率构造。
        /// </summary>
        /// <param name="matrixRange">矩阵范围。</param>
        /// <param name="voronoiNum">Voronoi点数量。</param>
        /// <param name="probabilityValue">陆地生成概率。</param>
        public SimpleVoronoiIsland(MatrixRange matrixRange, int voronoiNum, double probabilityValue)
        {
            voronoiDiagram = new VoronoiDiagram(matrixRange, voronoiNum);
            this.probability = probabilityValue;
        }

        /// <summary>
        /// 使用矩阵范围、Voronoi点数量、陆地概率与陆地值构造。
        /// </summary>
        /// <param name="matrixRange">矩阵范围。</param>
        /// <param name="voronoiNum">Voronoi点数量。</param>
        /// <param name="probabilityValue">陆地生成概率。</param>
        /// <param name="landValue">陆地对应矩阵值。</param>
        public SimpleVoronoiIsland(MatrixRange matrixRange, int voronoiNum, double probabilityValue, int landValue)
        {
            voronoiDiagram = new VoronoiDiagram(matrixRange, voronoiNum);
            this.probability = probabilityValue;
            this.landValue = landValue;
        }

        /// <summary>
        /// 使用矩阵范围、Voronoi点数量、陆地概率、陆地值与海洋值构造。
        /// </summary>
        /// <param name="matrixRange">矩阵范围。</param>
        /// <param name="voronoiNum">Voronoi点数量。</param>
        /// <param name="probabilityValue">陆地生成概率。</param>
        /// <param name="landValue">陆地对应矩阵值。</param>
        /// <param name="seaValue">海洋对应矩阵值。</param>
        public SimpleVoronoiIsland(MatrixRange matrixRange, int voronoiNum, double probabilityValue, int landValue, int seaValue)
        {
            voronoiDiagram = new VoronoiDiagram(matrixRange, voronoiNum);
            this.probability = probabilityValue;
            this.landValue = landValue;
            this.seaValue = seaValue;
        }
    }
}