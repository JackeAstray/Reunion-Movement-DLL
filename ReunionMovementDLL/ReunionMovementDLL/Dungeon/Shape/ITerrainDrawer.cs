namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// 地形生成器接口。
    /// 提供将高度矩阵生成并归一化的方法，供 TerrainUtil 调用。
    /// 在 Unity 环境中可由具体实现（例如基于噪声的生成器）来实现该接口。
    /// </summary>
    public interface ITerrainDrawer
    {
        /// <summary>
        /// 将传入的高度矩阵填充为归一化高度数据（值通常在 [0,1] 范围内）。
        /// 参数为行=height、列=width 的矩阵。
        /// </summary>
        /// <param name="matrix">要填充的高度矩阵，被调用者修改。</param>
        void DrawNormalize(float[,] matrix);
    }
}