using ReunionMovementDLL.Dungeon.Random;
using ReunionMovementDLL.Dungeon.Util;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// 随机 Voronoi 生成器：基于 VoronoiDiagram 对矩阵区域进行分区，并按给定的概率对每个分区点赋予两种颜色之一。
    /// </summary>
    public class RandomVoronoi : IDrawer<int>
    {
        /// <summary>
        /// 随机数生成器，用于按概率选择颜色。
        /// </summary>
        private RandomBase rand = new RandomBase();

        /// <summary>
        /// 内部使用的 VoronoiDiagram 对象，负责实际的 Voronoi 分区与绘制流程。
        /// </summary>
        private VoronoiDiagram voronoiDiagram;

        /// <summary>
        /// 判定为 trueColor 的概率值（范围 0.0 - 1.0）。
        /// </summary>
        public double probabilityValue { get; set; }

        /// <summary>
        /// 在随机判定为 true 时使用的颜色值（或瓦片 ID）。
        /// </summary>
        public int trueColor { get; set; }

        /// <summary>
        /// 在随机判定为 false 时使用的颜色值（或瓦片 ID）。
        /// </summary>
        public int falseColor { get; set; }

        /// <summary>
        /// 将 Voronoi 分区绘制到目标矩阵。对于每个分区点，按 probabilityValue 决定使用 trueColor 或 falseColor。
        /// </summary>
        /// <param name="matrix">目标矩阵（二维整型数组）。</param>
        /// <returns>绘制是否成功，当前实现总是返回 true。</returns>
        public bool Draw(int[,] matrix)
        {
            DungeonDelegate.VoronoiDiagramDelegate voronoiDiagramDelegate =
                (ref Pair point, ref int color, uint startX, uint startY, uint w, uint h) =>
                {
                    if (rand.Probability((this.probabilityValue)))
                        color = this.trueColor;
                    else
                        color = this.falseColor;
                };

            voronoiDiagram.Draw(matrix, voronoiDiagramDelegate);
            return true;
        }

        /// <summary>
        /// 带日志输出的绘制方法（未实现）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="log">输出日志（out）。</param>
        /// <returns>抛出 <see cref="System.NotImplementedException"/> 表示该方法尚未实现。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 在指定矩阵上执行绘制并返回该矩阵引用（便捷方法）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <returns>已绘制的矩阵引用。</returns>
        public int[,] Create(int[,] matrix)
        {
            this.Draw(matrix);
            return matrix;
        }

        /* Getter */
        /// <summary>
        /// 获取 Voronoi 点的起始 X 坐标。
        /// </summary>
        public uint GetPointX()
        {
            return voronoiDiagram.startX;
        }

        /// <summary>
        /// 获取 Voronoi 点的起始 Y 坐标。
        /// </summary>
        public uint GetPointY()
        {
            return voronoiDiagram.startY;
        }

        /// <summary>
        /// 获取 Voronoi 区域的宽度。
        /// </summary>
        public uint GetWidth()
        {
            return voronoiDiagram.width;
        }

        /// <summary>
        /// 获取 Voronoi 区域的高度。
        /// </summary>
        public uint GetHeight()
        {
            return voronoiDiagram.height;
        }

        /// <summary>
        /// 获取 Voronoi 绘制使用的默认值（drawValue）。
        /// </summary>
        public int GetValue()
        {
            return voronoiDiagram.drawValue;
        }

        /* Setter */
        /// <summary>
        /// 设置 Voronoi 点的起始 X 坐标并返回自身以便链式调用。
        /// </summary>
        public RandomVoronoi SetPointX(uint value)
        {
            this.voronoiDiagram.startX = value;
            return this;
        }

        /// <summary>
        /// 设置 Voronoi 点的起始 Y 坐标并返回自身以便链式调用。
        /// </summary>
        public RandomVoronoi SetPointY(uint value)
        {
            voronoiDiagram.startY = value;
            return this;
        }

        /// <summary>
        /// 设置 Voronoi 区域的宽度并返回自身以便链式调用。
        /// </summary>
        public RandomVoronoi SetWidth(uint value)
        {
            voronoiDiagram.width = value;
            return this;
        }

        /// <summary>
        /// 设置 Voronoi 区域的高度并返回自身以便链式调用。
        /// </summary>
        public RandomVoronoi SetHeight(uint value)
        {
            voronoiDiagram.height = value;
            return this;
        }

        /// <summary>
        /// 同时设置起始 X/Y 为同一值并返回自身以便链式调用。
        /// </summary>
        public RandomVoronoi SetPoint(uint value)
        {
            voronoiDiagram.startX = value;
            voronoiDiagram.startY = value;
            return this;
        }

        /// <summary>
        /// 设置起始 X/Y 并返回自身以便链式调用。
        /// </summary>
        public RandomVoronoi SetPoint(uint startX, uint startY)
        {
            voronoiDiagram.startX = startX;
            voronoiDiagram.startY = startY;
            return this;
        }

        /// <summary>
        /// 设置区域范围（起始坐标与尺寸）并返回自身以便链式调用。
        /// </summary>
        public RandomVoronoi SetRange(uint startX, uint startY, uint width, uint height)
        {
            voronoiDiagram.startX = startX;
            voronoiDiagram.startY = startY;
            voronoiDiagram.width = width;
            voronoiDiagram.height = height;
            return this;
        }

        /// <summary>
        /// 使用 MatrixRange 设置 Voronoi 区域范围并返回自身以便链式调用。
        /// </summary>
        public RandomVoronoi SetRange(MatrixRange matrixRange)
        {
            voronoiDiagram.startX = (uint)matrixRange.x;
            voronoiDiagram.startY = (uint)matrixRange.y;
            voronoiDiagram.width = (uint)matrixRange.w;
            voronoiDiagram.height = (uint)matrixRange.h;
            return this;
        }


        /* Clear */
        /// <summary>
        /// 清除起始 X 设置并返回自身。
        /// </summary>
        public RandomVoronoi ClearPointX()
        {
            this.voronoiDiagram.ClearPointX();
            return this;
        }

        /// <summary>
        /// 清除起始 Y 设置并返回自身。
        /// </summary>
        public RandomVoronoi ClearPointY()
        {
            this.voronoiDiagram.ClearPointY();
            return this;
        }

        /// <summary>
        /// 清除宽度设置并返回自身。
        /// </summary>
        public RandomVoronoi ClearWidth()
        {
            this.voronoiDiagram.ClearWidth();
            return this;
        }

        /// <summary>
        /// 清除高度设置并返回自身。
        /// </summary>
        public RandomVoronoi ClearHeight()
        {
            this.voronoiDiagram.ClearHeight();
            return this;
        }

        /// <summary>
        /// 清除绘制值设置并返回自身。
        /// </summary>
        public RandomVoronoi ClearValue()
        {
            this.voronoiDiagram.ClearValue();
            return this;
        }

        /// <summary>
        /// 清除起始点 X/Y 并返回自身。
        /// </summary>
        public RandomVoronoi ClearPoint()
        {
            this.ClearPointX();
            this.ClearPointY();
            return this;
        }

        /// <summary>
        /// 清除区域范围的所有设置并返回自身。
        /// </summary>
        public RandomVoronoi ClearRange()
        {
            this.ClearPointX();
            this.ClearPointY();
            this.ClearWidth();
            this.ClearHeight();
            return this;
        }

        /// <summary>
        /// 清除所有设置（范围与绘制值）并返回自身。
        /// </summary>
        public RandomVoronoi Clear()
        {
            this.ClearRange();
            this.ClearValue();
            return this;
        }

        /* Constructors */
        /// <summary>
        /// 默认构造函数，创建内部 VoronoiDiagram 实例。
        /// </summary>
        public RandomVoronoi()
        {
            voronoiDiagram = new VoronoiDiagram();
        } // default

        /// <summary>
        /// 使用指定的绘制值初始化 VoronoiDiagram。
        /// </summary>
        /// <param name="drawValue">绘制值（drawValue）。</param>
        public RandomVoronoi(int drawValue)
        {
            voronoiDiagram = new VoronoiDiagram(drawValue);
        }

        /// <summary>
        /// 使用绘制值与概率值初始化。
        /// </summary>
        public RandomVoronoi(int drawValue, double probabilityValue)
        {
            voronoiDiagram = new VoronoiDiagram(drawValue);
            this.probabilityValue = probabilityValue;
        }

        /// <summary>
        /// 使用绘制值、概率与 trueColor 初始化。
        /// </summary>
        public RandomVoronoi(int drawValue, double probabilityValue, int trueColor)
        {
            voronoiDiagram = new VoronoiDiagram(drawValue);
            this.probabilityValue = probabilityValue;
            this.trueColor = trueColor;
        }

        /// <summary>
        /// 使用绘制值、概率、trueColor 与 falseColor 初始化。
        /// </summary>
        public RandomVoronoi(int drawValue, double probabilityValue, int trueColor, int falseColor)
        {
            voronoiDiagram = new VoronoiDiagram(drawValue);
            this.probabilityValue = probabilityValue;
            this.trueColor = trueColor;
            this.falseColor = falseColor;
        }

        /// <summary>
        /// 使用矩阵范围初始化 VoronoiDiagram 的构造函数。
        /// </summary>
        public RandomVoronoi(MatrixRange matrixRange)
        {
            voronoiDiagram = new VoronoiDiagram(matrixRange);
        }

        /// <summary>
        /// 使用矩阵范围与绘制值初始化。
        /// </summary>
        public RandomVoronoi(MatrixRange matrixRange, int drawValue)
        {
            voronoiDiagram = new VoronoiDiagram(matrixRange, drawValue);
        }

        /// <summary>
        /// 使用矩阵范围、绘制值及概率值初始化。
        /// </summary>
        public RandomVoronoi(MatrixRange matrixRange, int drawValue, double probabilityValue)
        {
            voronoiDiagram = new VoronoiDiagram(matrixRange, drawValue);
            this.probabilityValue = probabilityValue;
        }

        /// <summary>
        /// 使用矩阵范围、绘制值、概率与 trueColor 初始化。
        /// </summary>
        public RandomVoronoi(MatrixRange matrixRange, int drawValue, double probabilityValue, int trueColor)
        {
            voronoiDiagram = new VoronoiDiagram(matrixRange, drawValue);
            this.probabilityValue = probabilityValue;
            this.trueColor = trueColor;
        }

        /// <summary>
        /// 使用矩阵范围、绘制值、概率、trueColor 与 falseColor 初始化。
        /// </summary>
        public RandomVoronoi(MatrixRange matrixRange, int drawValue, double probabilityValue, int trueColor,
            int falseColor)
        {
            voronoiDiagram = new VoronoiDiagram(matrixRange, drawValue);
            this.probabilityValue = probabilityValue;
            this.trueColor = trueColor;
            this.falseColor = falseColor;
        }
    }
}