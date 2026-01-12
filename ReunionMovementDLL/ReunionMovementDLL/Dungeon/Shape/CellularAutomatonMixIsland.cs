using ReunionMovementDLL.Dungeon.Retouch;
using System.Collections.Generic;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// 将半混合矩形、边框和元胞自动机结合起来生成岛屿的绘制器。
    /// 该类组合多个绘制器：先绘制混合矩形和边框，然后多次应用元胞自动机进行平滑/细化。
    /// </summary>
    public class CellularAutomatonMixIsland : IDrawer<int>
    {
        /// <summary>
        /// 用于基于细胞自动机规则处理地图的实例。
        /// </summary>
        private CellularAutomaton cellularAutomaton = new CellularAutomaton();

        /// <summary>
        /// 用于绘制边界的绘制器实例（通常作为混合矩形的边界）。
        /// </summary>
        Border border = new Border();

        /// <summary>
        /// 半混合矩形绘制器，用于初始地形的生成。
        /// </summary>
        HalfMixRect mixRect;

        /// <summary>
        /// 元胞自动机循环执行的次数，用于控制平滑/重绘的强度。
        /// </summary>
        protected uint loopNum = 1;

        /// <summary>
        /// 在给定矩阵上执行绘制：先绘制半混合矩形，再绘制边框，最后执行指定次数的元胞自动机处理。
        /// 返回true表示操作已完成。
        /// </summary>
        /// <param name="matrix">要绘制的二维整数矩阵。</param>
        /// <returns>表示绘制是否成功的布尔值（本实现始终返回true）。</returns>
        public bool Draw(int[,] matrix)
        {
            this.mixRect.Draw(matrix);
            this.border.Draw(matrix);
            for (var i = 0; i < this.loopNum; ++i)
            {
                this.cellularAutomaton.Draw(matrix);
            }
            return true;
        }

        /// <summary>
        /// 在给定矩阵上绘制并输出日志信息的重载版本。
        /// 目前尚未实现，调用时会抛出NotImplementedException。
        /// </summary>
        /// <param name="matrix">要绘制的矩阵。</param>
        /// <param name="log">输出日志字符串（未实现）。</param>
        /// <returns>抛出System.NotImplementedException。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 在给定矩阵上创建并返回绘制结果（调用Draw）。
        /// </summary>
        /// <param name="matrix">要填充并返回的矩阵。</param>
        /// <returns>填充后的矩阵引用。</returns>
        public int[,] Create(int[,] matrix)
        {
            this.Draw(matrix);
            return matrix;
        }

        /* Getter */

        /// <summary>
        /// 获取当前设置的起始点X（列索引），来自内部的border对象。
        /// </summary>
        public uint GetPointX()
        {
            return this.border.GetPointX();
        }

        /// <summary>
        /// 获取当前设置的起始点Y（行索引），来自内部的border对象。
        /// </summary>
        public uint GetPointY()
        {
            return this.border.GetPointY();
        }

        /// <summary>
        /// 获取当前设置的宽度，来自内部的border对象。
        /// </summary>
        public uint GetWidth()
        {
            return this.border.GetWidth();
        }

        /// <summary>
        /// 获取当前设置的高度，来自内部的border对象。
        /// </summary>
        public uint Getheight()
        {
            return this.border.GetHeight();
        }

        /* Setter */

        /// <summary>
        /// 设置起始点X（列索引），并将该值应用到mixRect、border和cellularAutomaton。
        /// 返回当前实例以便链式调用。
        /// </summary>
        public CellularAutomatonMixIsland SetPointX(uint endX)
        {
            this.mixRect.SetPointX(endX);
            this.border.SetPointX(endX);
            this.cellularAutomaton.SetPointX(endX);
            return this;
        }

        /// <summary>
        /// 设置起始点Y（行索引），并将该值应用到mixRect、border和cellularAutomaton。
        /// 返回当前实例以便链式调用。
        /// </summary>
        public CellularAutomatonMixIsland SetPointY(uint endY)
        {
            this.mixRect.SetPointY(endY);
            this.border.SetPointY(endY);
            this.cellularAutomaton.SetPointY(endY);
            return this;
        }

        /// <summary>
        /// 设置宽度并将该值应用到mixRect、border和cellularAutomaton。
        /// 返回当前实例以便链式调用。
        /// </summary>
        public CellularAutomatonMixIsland SetWidth(uint width)
        {
            this.mixRect.SetWidth(width);
            this.border.SetWidth(width);
            this.cellularAutomaton.SetWidth(width);
            return this;
        }

        /// <summary>
        /// 设置高度并将该值应用到mixRect、border和cellularAutomaton。
        /// 返回当前实例以便链式调用。
        /// </summary>
        public CellularAutomatonMixIsland SetHeight(uint height)
        {
            this.mixRect.SetHeight(height);
            this.border.SetHeight(height);
            this.cellularAutomaton.SetHeight(height);
            return this;
        }

        /// <summary>
        /// 使用MatrixRange设置范围，并将其应用到mixRect、border和cellularAutomaton。
        /// 返回当前实例以便链式调用。
        /// </summary>
        public CellularAutomatonMixIsland SetRange(MatrixRange matrixRange)
        {
            this.mixRect.SetRange(matrixRange);
            this.border.SetRange(matrixRange);
            this.cellularAutomaton.SetRange(matrixRange);
            return this;
        }

        /// <summary>
        /// 使用单一长度设置方形范围（endX, endY, length），并将其应用到内部组件。
        /// 返回当前实例以便链式调用。
        /// </summary>
        public CellularAutomatonMixIsland SetRange(uint endX, uint endY, uint length)
        {
            this.SetPointX(endX);
            this.SetPointY(endY);
            this.SetWidth(length);
            this.SetHeight(length);
            return this;
        }

        /// <summary>
        /// 使用明确的宽高设置矩形范围，并将其应用到内部组件。
        /// 返回当前实例以便链式调用。
        /// </summary>
        public CellularAutomatonMixIsland SetRange(uint endX, uint endY, uint width, uint height)
        {
            this.SetPointX(endX);
            this.SetPointY(endY);
            this.SetWidth(width);
            this.SetHeight(height);
            return this;
        }

        /// <summary>
        /// 同时设置起始点X和Y为同一个值（point）。
        /// 返回当前实例以便链式调用。
        /// </summary>
        public CellularAutomatonMixIsland SetPoint(uint point)
        {
            this.SetPointX(point);
            this.SetPointY(point);
            return this;
        }

        /// <summary>
        /// 同时设置起始点X和Y为给定的endX和endY。
        /// 返回当前实例以便链式调用。
        /// </summary>
        public CellularAutomatonMixIsland SetPoint(uint endX, uint endY)
        {
            this.SetPointX(endX);
            this.SetPointY(endY);
            return this;
        }

        /* Clear */

        /// <summary>
        /// 清除起始点X的设置（在mixRect、border和cellularAutomaton中）。
        /// 返回当前实例以便链式调用。
        /// </summary>
        public CellularAutomatonMixIsland ClearPointX()
        {
            this.mixRect.ClearPointX();
            this.border.ClearPointX();
            this.cellularAutomaton.ClearPointX();
            return this;
        }

        /// <summary>
        /// 清除起始点Y的设置（在mixRect、border和cellularAutomaton中）。
        /// 返回当前实例以便链式调用。
        /// </summary>
        public CellularAutomatonMixIsland ClearPointY()
        {
            this.mixRect.ClearPointY();
            this.border.ClearPointY();
            this.cellularAutomaton.ClearPointY();
            return this;
        }

        /// <summary>
        /// 清除宽度设置（在mixRect、border和cellularAutomaton中）。
        /// 返回当前实例以便链式调用。
        /// </summary>
        public CellularAutomatonMixIsland ClearWidth()
        {
            this.mixRect.ClearWidth();
            this.border.ClearWidth();
            this.cellularAutomaton.ClearWidth();
            return this;
        }

        /// <summary>
        /// 清除高度设置（在mixRect、border和cellularAutomaton中）。
        /// 返回当前实例以便链式调用。
        /// </summary>
        public CellularAutomatonMixIsland ClearHeight()
        {
            this.mixRect.ClearHeight();
            this.border.ClearHeight();
            this.cellularAutomaton.ClearHeight();
            return this;
        }

        /// <summary>
        /// 清除绘制值设置：对mixRect、border执行ClearValue，并对cellularAutomaton执行Clear（清空状态）。
        /// 返回当前实例以便链式调用。
        /// </summary>
        public CellularAutomatonMixIsland ClearValue()
        {
            this.mixRect.ClearValue();
            this.border.ClearValue();
            // this.cellularAutomaton.ClearValue();
            this.cellularAutomaton.Clear();
            return this;
        }

        /// <summary>
        /// 清除范围设置：对mixRect和border执行ClearRange，并对cellularAutomaton清除起始Y（根据原实现）。
        /// 返回当前实例以便链式调用。
        /// </summary>
        public CellularAutomatonMixIsland ClearRange()
        {
            this.mixRect.ClearRange();
            this.border.ClearRange();
            this.cellularAutomaton.ClearPointY();
            return this;
        }

        /// <summary>
        /// 清除起始点设置（X和Y）。
        /// 返回当前实例以便链式调用。
        /// </summary>
        public CellularAutomatonMixIsland ClearPoint()
        {
            this.ClearPointX();
            this.ClearPointY();
            return this;
        }

        /* Constructors */

        /// <summary>
        /// 默认构造函数：创建一个默认的HalfMixRect并使用默认的cellularAutomaton和border。
        /// </summary>
        public CellularAutomatonMixIsland()
        {
            this.mixRect = new HalfMixRect();
        }

        /// <summary>
        /// 使用循环次数和初始列表构造对象，list用于初始化HalfMixRect。
        /// </summary>
        /// <param name="loopNum">应用cellularAutomaton的循环次数。</param>
        /// <param name="list">初始化HalfMixRect时使用的整数列表。</param>
        public CellularAutomatonMixIsland(uint loopNum, IList<int> list)
        {
            this.loopNum = loopNum;
            this.mixRect = new HalfMixRect(list);
        }

        /// <summary>
        /// 使用给定的MatrixRange、循环次数和列表构造对象，并初始化内部的cellularAutomaton为指定范围。
        /// </summary>
        /// <param name="matrixRange">用于初始化内部组件范围的MatrixRange。</param>
        /// <param name="loopNum">应用cellularAutomaton的循环次数。</param>
        /// <param name="list">初始化HalfMixRect时使用的整数列表。</param>
        public CellularAutomatonMixIsland(MatrixRange matrixRange, uint loopNum, IList<int> list)
        {
            this.loopNum = loopNum;
            this.mixRect = new HalfMixRect(matrixRange, list);
            this.cellularAutomaton = new CellularAutomaton(matrixRange);
        }
    }
}