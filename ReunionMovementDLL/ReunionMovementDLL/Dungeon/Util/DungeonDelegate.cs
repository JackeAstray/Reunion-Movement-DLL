using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Util
{
    /// <summary>
    /// 地牢相关的委托集合。
    /// </summary>
    public static class DungeonDelegate
    {
        /// <summary>
        /// Voronoi 图委托：允许调用方根据生成的点设置颜色/值。
        /// 参数：ref Pair (点坐标), ref int (颜色/值), startX, startY, w, h
        /// </summary>
        public delegate void VoronoiDiagramDelegate(ref Pair pair, ref int color, uint startX, uint startY, uint w, uint h);
    }
}