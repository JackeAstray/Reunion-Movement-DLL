using ReunionMovementDLL.Dungeon.Shape;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Util
{
    /// <summary>
    /// 地形工具类（不依赖 Unity）。
    /// 该类使用抽象接口 `ITerrain`/`ITerrainData`/`ITexture2D`，可在非 Unity 环境编译
    /// Unity 中可提供具体适配器实现这些接口以完成真正的地形操作。
    /// </summary>
    public class TerrainUtil
    {
        /// <summary>
        /// 地形对象抽象（只读）。
        /// </summary>
        public ITerrain terrain { get; private set; }

        /// <summary>
        /// 地形数据抽象（只读）。
        /// </summary>
        public ITerrainData terrainData { get; private set; }

        /// <summary>
        /// 用于将高度值映射到纹理索引的阈值列表。
        /// </summary>
        public List<float> textureToHeight { get; set; }

        /// <summary>
        /// 地形深度（Y 方向尺寸）。
        /// </summary>
        public int depth { get; set; }

        /// <summary>
        /// 地形宽度（X 方向尺寸，对应矩阵列数）。
        /// </summary>
        public int width { get; set; }

        /// <summary>
        /// 地形高度（Z 方向尺寸，对应矩阵行数）。
        /// </summary>
        public int height { get; set; }

        /// <summary>
        /// 高度矩阵（行 = height，列 = width）。
        /// </summary>
        public float[,] matrix { get; private set; }

        /// <summary>
        /// 平滑迭代次数。
        /// </summary>
        public uint smooth { get; set; }

        private List<ITexture2D> texture2D;
        private ITerrainDrawer terrainGenerator;

        /// <summary>
        /// 构造并使用抽象地形对象、纹理列表和地形绘制器。
        /// </summary>
        /// <param name="terrain">实现了 ITerrain 的地形对象（非 null）</param>
        /// <param name="texture2D">纹理列表（元素为 ITexture2D 实现）</param>
        /// <param name="terrainGenerator">地形生成器（实现 ITerrainDrawer）</param>
        /// <param name="height">高度（矩阵行数）</param>
        /// <param name="width">宽度（矩阵列数）</param>
        /// <param name="depth">地形深度</param>
        /// <param name="smooth">平滑迭代次数</param>
        public TerrainUtil(ITerrain terrain, List<ITexture2D> texture2D, ITerrainDrawer terrainGenerator,
            int height, int width, int depth, uint smooth = 0)
        {
            this.terrain = terrain ?? throw new ArgumentNullException(nameof(terrain));
            this.texture2D = texture2D ?? new List<ITexture2D>();
            this.terrainGenerator = terrainGenerator ?? throw new ArgumentNullException(nameof(terrainGenerator));
            this.terrainData = terrain.terrainData ?? throw new ArgumentNullException(nameof(terrain));
            this.height = height;
            this.width = width;
            this.depth = depth;
            this.smooth = smooth;
            SetTextureToHeight();
        }

        /// <summary>
        /// 构造并使用自定义的高度到纹理映射。
        /// </summary>
        public TerrainUtil(ITerrain terrain, List<ITexture2D> texture2D, ITerrainDrawer terrainGenerator,
            int height, int width, int depth, List<float> textureToHeight, uint smooth = 0)
        {
            this.terrain = terrain ?? throw new ArgumentNullException(nameof(terrain));
            this.texture2D = texture2D ?? new List<ITexture2D>();
            this.terrainGenerator = terrainGenerator ?? throw new ArgumentNullException(nameof(terrainGenerator));
            this.terrainData = terrain.terrainData ?? throw new ArgumentNullException(nameof(terrain));
            this.textureToHeight = textureToHeight ?? new List<float>();
            this.height = height;
            this.width = width;
            this.depth = depth;
            this.smooth = smooth;
        }

        /// <summary>
        /// 生成并应用地形数据到 terrainData。
        /// 在非 Unity 环境中，调用方应实现 ITerrainData 的 SetHeights/SetAlphamaps 做具体操作。
        /// </summary>
        public void Draw()
        {
            Generate();
            SetTerrainData();
            float[,,] textureMap;
            terrainData.SetHeights(0, 0, matrix);
            if (this.texture2D != null && this.texture2D.Count > 0)
            {
                textureMap = GetTexture(matrix, terrainData.alphamapWidth, terrainData.alphamapHeight);
                terrainData.SetAlphamaps(0, 0, textureMap);
            }
        }

        /// <summary>
        /// 调用地形生成器生成归一化高度矩阵并进行平滑处理。
        /// </summary>
        private void Generate()
        {
            if (height <= 0 || width <= 0)
                throw new ArgumentException("高度和宽度必须为正数");

            matrix = new float[height, width];
            terrainGenerator.DrawNormalize(matrix);
            Smooth(matrix, smooth);
        }

        /// <summary>
        /// 设置 terrainData 的大小、分辨率及 Splat 原型数组。
        /// 注意：在非 Unity 环境中，ITerrainData 的实现需要解释这些值。
        /// </summary>
        private void SetTerrainData()
        {
            // 使用 System.Numerics.Vector3 作为尺寸类型
            terrainData.size = new Vector3(width, depth, height);
            var alphaMapResolution = Math.Max(height, width);
            var heightMapResolution = Math.Max(height, width);
            var splatPrototypeArray = this.SetSplatPrototypes();
            SetResolutions(alphaMapResolution, heightMapResolution);
            terrainData.splatPrototypes = splatPrototypeArray;
        }

        /// <summary>
        /// 设置 alphamap 与 heightmap 的分辨率。
        /// </summary>
        private void SetResolutions(int alphaR, int heightR)
        {
            this.terrainData.alphamapResolution = alphaR;
            this.terrainData.heightmapResolution = heightR;
        }

        /// <summary>
        /// 创建 SplatPrototype 数组，用于 terrainData 指定贴图信息。
        /// </summary>
        /// <returns>SplatPrototype 数组</returns>
        private SplatPrototype[] SetSplatPrototypes()
        {
            var len = this.texture2D?.Count ?? 0;
            var splatPrototype = new SplatPrototype[len];
            for (int i = 0; i < len; ++i)
            {
                splatPrototype[i] = new SplatPrototype();
                splatPrototype[i].tileSize = new Vector2(1f, 1f);
                splatPrototype[i].texture = texture2D[i];
            }

            return splatPrototype;
        }

        /// <summary>
        /// 根据高度矩阵与阈值列表生成 alphamap（每个点的纹理权重）。
        /// 返回维度为 [w,h,textureCount] 的三维数组。
        /// </summary>
        private float[,,] GetTexture(float[,] matrix, int w, int h)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));
            if (textureToHeight == null || textureToHeight.Count == 0) throw new ArgumentException("必须提供纹理转高度");

            var txCount = this.texture2D.Count;
            var map = new float[w, h, txCount];
            for (var y = 0; y < h; ++y)
            {
                for (var x = 0; x < w; ++x)
                {
                    var value = matrix[y, x];
                    var idx = LowerBound(this.textureToHeight, value);
                    if (idx < 0) idx = 0;
                    if (idx >= txCount) idx = txCount - 1;
                    map[x, y, idx] = 1f;
                }
            }

            return map;
        }

        /// <summary>
        /// 在有序列表中寻找第一个 >= value 的下界索引（返回 index），若全部小于则返回最后一个索引。
        /// </summary>
        private int LowerBound(List<float> list, float value)
        {
            if (list == null || list.Count == 0) return 0;
            var left = 0;
            var right = list.Count - 1;

            while (left < right)
            {
                int mid = (left + right) / 2;
                if (list[mid] < value)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid;
                }
            }

            return left;
        }

        /// <summary>
        /// 根据当前纹理数量初始化默认的高度阈值列表（均匀分布在 [0,1]）。
        /// </summary>
        private void SetTextureToHeight()
        {
            var len = this.texture2D?.Count ?? 0;
            this.textureToHeight = new List<float>();
            if (len == 0)
                return;

            var dh = 1.0f / len;
            for (int i = 0; i < len; ++i)
            {
                textureToHeight.Add(i * dh);
            }
        }

        /// <summary>
        /// 对高度地图执行简单的平滑（基于四邻域平均）。
        /// </summary>
        /// <param name="heightMap">高度矩阵（行=height, 列=width）</param>
        /// <param name="iterationNum">迭代次数</param>
        public void Smooth(float[,] heightMap, uint iterationNum)
        {
            if (heightMap == null) throw new ArgumentNullException(nameof(heightMap));
            if (height <= 0 || width <= 0) return;

            var dh = new[] { 1, -1, 0, 0 };
            var dw = new[] { 0, 0, 1, -1 };
            for (int iter = 0; iter < iterationNum; ++iter)
            {
                for (var h = 0; h < height; ++h)
                {
                    for (var w = 0; w < width; ++w)
                    {
                        var cumulative = 0;
                        float cumulativeValue = 0f;
                        for (int i = 0; i < 4; ++i)
                        {
                            var nh = h + dh[i];
                            var nw = w + dw[i];

                            if (nh >= 0 && nw >= 0 && nh < height && nw < width)
                            {
                                ++cumulative;
                                cumulativeValue += heightMap[nh, nw];
                            }
                        }

                        cumulativeValue += heightMap[h, w];
                        ++cumulative;

                        heightMap[h, w] = cumulativeValue / cumulative;
                    }
                }
            }
        }
    }
}