using System.Numerics;
using System;
using System.Collections.Generic;

namespace ReunionMovementDLL.Dungeon.Util
{
    /// <summary>
    /// 纹理抽象（占位），用于替代 UnityEngine.Texture2D。
    /// 实际项目中可由 Unity 的 Texture2D 或自定义实现替代。
    /// </summary>
    public interface ITexture2D { }

    /// <summary>
    /// 地形数据抽象，提供 TerrainData 所需的最小成员。
    /// 具体在 Unity 中应由 UnityEngine.TerrainData 实现或适配器实现。
    /// </summary>
    public interface ITerrainData
    {
        Vector3 size { get; set; }
        int alphamapWidth { get; }
        int alphamapHeight { get; }
        int alphamapResolution { get; set; }
        int heightmapResolution { get; set; }
        SplatPrototype[] splatPrototypes { get; set; }
        void SetHeights(int xBase, int yBase, float[,] heights);
        void SetAlphamaps(int xBase, int yBase, float[,,] alphamaps);
    }

    /// <summary>
    /// 地形抽象（占位），用于替代 UnityEngine.Terrain。
    /// 应至少提供对 ITerrainData 的访问。
    /// </summary>
    public interface ITerrain
    {
        ITerrainData terrainData { get; }
    }

    /// <summary>
    /// Splat 原型占位类型，包含瓦片大小与贴图引用。
    /// 用于替代 UnityEngine.SplatPrototype。
    /// </summary>
    public class SplatPrototype
    {
        public Vector2 tileSize { get; set; }
        public ITexture2D texture { get; set; }

        public SplatPrototype()
        {
            tileSize = new Vector2(1f, 1f);
        }
    }
}