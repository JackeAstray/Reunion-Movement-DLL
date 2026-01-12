using ReunionMovementDLL.Dungeon.Retouch;

namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// 基于元胞自动机的岛屿绘制器。
    /// 使用内部的CellularAutomaton实例进行地形处理或重绘（具体算法在Retouch层实现）。
    /// </summary>
    public class CellularAutomatonIsland : IDrawer<int>
    {
        /// <summary>
        /// 元胞自动机实例，用于执行细化/重绘等操作。
        /// </summary>
        private CellularAutomaton cellularAutomaton = new CellularAutomaton();

        /// <summary>
        /// 在给定的二维整数矩阵上绘制基于元胞自动机的岛屿。
        /// 目前方法体仅返回true；实际处理应由内部的CellularAutomaton完成（若需要，需在此处调用相应方法）。
        /// </summary>
        /// <param name="matrix">要绘制或处理的矩阵，行列索引对应地图坐标。</param>
        /// <returns>如果绘制/处理成功则返回true（当前总是返回true）。</returns>
        public bool Draw(int[,] matrix)
        {
            return true;
        }

        /// <summary>
        /// 在给定矩阵上绘制并输出日志信息的重载版本。
        /// 该重载当前未实现，会抛出NotImplementedException；调用方不应依赖此重载获取日志。
        /// </summary>
        /// <param name="matrix">要绘制或处理的矩阵。</param>
        /// <param name="log">输出参数：用于返回运行时日志信息（当前未实现）。</param>
        /// <returns>抛出System.NotImplementedException。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            throw new System.NotImplementedException();
        }
    }
}