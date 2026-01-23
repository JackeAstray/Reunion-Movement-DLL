using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReunionMovementDLL.Dungeon.Base
{
    /// <summary>
    /// 简单的无限地形管理器（基于分块、确定性生成、按需加载与回收）
    /// 目的：提供最小可用实现，演示如何实现无限大地形的基础设施。
    /// 注意：此实现仅作为库内示例，具体噪声/地图规则可按需替换。
    /// 
    /// 使用示例：
    /// var terrain = new InfiniteTerrain(seed:123);
    /// terrain.Logger = m => Console.WriteLine(m);
    /// // 根据玩家世界坐标请求周围区块（世界坐标以瓦片单位计）
    /// terrain.RequestChunksAroundWorld(playerWorldX, playerWorldY);
    /// // 立即获得某个瓦片（若区块不存在会同步生成）
    /// int tile = terrain.GetTileAtWorldOrGenerate(playerWorldX, playerWorldY);
    /// </summary>
    public class InfiniteTerrain
    {
        public class Chunk
        {
            public int X { get; }
            public int Y { get; }
            public int Size { get; }
            // 瓦片 ID 数组：Size x Size
            public int[,] Tiles { get; }
            public bool Dirty { get; set; }
            public DateTime LastAccessUtc { get; set; }

            public Chunk(int x, int y, int size)
            {
                X = x; Y = y; Size = size;
                Tiles = new int[size, size];
                Dirty = false;
                LastAccessUtc = DateTime.UtcNow;
            }
        }

        private readonly Dictionary<(int, int), Chunk> chunks = new Dictionary<(int, int), Chunk>();
        private readonly object @lock = new object();
        public int ChunkSize { get; }
        public int ViewRadiusChunks { get; }
        private readonly uint seed;

        // 噪声与分形参数
        public double NoiseScale { get; set; } = 0.08; // 控制噪声频率（每瓦片单位）
        public int NoiseOctaves { get; set; } = 3;
        public double NoisePersistence { get; set; } = 0.5;

        // 缓存限制：按区块数或估算内存字节数（0 表示不限制）
        public int MaxCachedChunks { get; set; } = 0;
        public long MaxMemoryBytes { get; set; } = 0;

        // 可选日志回调（信息以中文输出）
        public Action<string> Logger { get; set; }

        private readonly PerlinNoise noise;

        /// <summary>
        /// 创建一个 InfiniteTerrain 管理器实例。
        /// </summary>
        /// <param name="seed">用于确定性生成的全局种子</param>
        /// <param name="chunkSize">每个区块的瓦片边长（必须 >=2）</param>
        /// <param name="viewRadiusChunks">当调用 RequestChunksAround 时，保留的半径区块数量</param>
        public InfiniteTerrain(long seed = 0, int chunkSize = 32, int viewRadiusChunks = 2)
        {
            if (chunkSize < 2) throw new ArgumentOutOfRangeException(nameof(chunkSize));
            ChunkSize = chunkSize;
            ViewRadiusChunks = Math.Max(0, viewRadiusChunks);
            this.seed = (uint)seed;
            noise = new PerlinNoise((int)this.seed);
        }

        /// <summary>
        /// 同步生成一个区块（基于全局种子 + 区块坐标确定性生成）。
        /// 使用分形 Perlin 噪声为瓦片赋值，保证块与块之间连续。
        /// </summary>
        public Chunk GenerateChunk(int cx, int cy)
        {
            var chunk = new Chunk(cx, cy, ChunkSize);
            // 使用连续世界坐标作为噪声输入，保证块间一致性
            for (int y = 0; y < ChunkSize; y++)
            {
                for (int x = 0; x < ChunkSize; x++)
                {
                    double gx = (cx * ChunkSize + x) * NoiseScale;
                    double gy = (cy * ChunkSize + y) * NoiseScale;
                    double n = FractalNoise(gx, gy, NoiseOctaves, NoisePersistence);
                    // n 的范围接近 [-1,1]，先映射到 [0,1]
                    double nv = (n + 1.0) * 0.5;
                    // 基于阈值决定瓦片类型（可根据需要调整）
                    int tile;
                    if (nv < 0.15) tile = 1; // 墙
                    else if (nv < 0.30) tile = 2; // 房间
                    else if (nv < 0.33) tile = 3; // 入口
                    else if (nv < 0.45) tile = 4; // 通路
                    else tile = 0; // 空地
                    chunk.Tiles[x, y] = tile;
                }
            }
            chunk.LastAccessUtc = DateTime.UtcNow;
            Logger?.Invoke($"生成区块 ({cx},{cy})，ChunkSize={ChunkSize}");
            return chunk;
        }

        /// <summary>
        /// 异步生成一个区块。
        /// </summary>
        public Task<Chunk> GenerateChunkAsync(int cx, int cy)
        {
            return Task.Run(() => GenerateChunk(cx, cy));
        }

        /// <summary>
        /// 获取已存在的区块，或同步创建它。
        /// </summary>
        public Chunk GetOrCreateChunk(int cx, int cy)
        {
            lock (@lock)
            {
                if (chunks.TryGetValue((cx, cy), out var c))
                {
                    c.LastAccessUtc = DateTime.UtcNow;
                    return c;
                }
            }
            var created = GenerateChunk(cx, cy);
            lock (@lock)
            {
                chunks[(cx, cy)] = created;
                EnsureCacheLimits();
            }
            return created;
        }

        /// <summary>
        /// 请求在中心区块坐标周围生成/加载区块。
        /// 该方法将异步生成缺失的区块，并回收超出半径的区块（LRU/内存限制策略）。
        /// </summary>
        public void RequestChunksAround(int centerCx, int centerCy)
        {
            var tasks = new List<Task>();
            for (int dy = -ViewRadiusChunks; dy <= ViewRadiusChunks; dy++)
            {
                for (int dx = -ViewRadiusChunks; dx <= ViewRadiusChunks; dx++)
                {
                    int cx = centerCx + dx;
                    int cy = centerCy + dy;
                    lock (@lock)
                    {
                        if (chunks.ContainsKey((cx, cy)))
                        {
                            chunks[(cx, cy)].LastAccessUtc = DateTime.UtcNow;
                            continue;
                        }
                    }
                    // 异步生成区块并在完成后保存到字典
                    var t = GenerateChunkAsync(cx, cy).ContinueWith(tt =>
                    {
                        if (tt.IsCompletedSuccessfully)
                        {
                            lock (@lock)
                            {
                                chunks[(cx, cy)] = tt.Result;
                                EnsureCacheLimits();
                            }
                            Logger?.Invoke($"异步生成并缓存区块 ({cx},{cy})");
                        }
                    });
                    tasks.Add(t);
                }
            }

            // 回收超出视野半径但仍在缓存中的区块（先按半径剔除）
            List<(int, int)> toRemove = new List<(int, int)>();
            lock (@lock)
            {
                foreach (var kv in chunks)
                {
                    var key = kv.Key;
                    int dx = Math.Abs(key.Item1 - centerCx);
                    int dy = Math.Abs(key.Item2 - centerCy);
                    if (dx > ViewRadiusChunks || dy > ViewRadiusChunks)
                    {
                        toRemove.Add(key);
                    }
                }
                foreach (var k in toRemove)
                {
                    // 注意：在真实场景中应在移除前序列化脏区块以持久化修改
                    chunks.Remove(k);
                    Logger?.Invoke($"回收超出视野的区块 ({k.Item1},{k.Item2})");
                }
            }
        }

        /// <summary>
        /// 根据世界坐标（瓦片单位）获取对应的瓦片 ID（仅当区块已加载）。
        /// 如果区块不存在，返回 null。
        /// </summary>
        public int? GetTileAtWorld(int worldX, int worldY)
        {
            WorldToChunkAndLocal(worldX, worldY, out int cx, out int cy, out int lx, out int ly);
            lock (@lock)
            {
                if (chunks.TryGetValue((cx, cy), out var c))
                {
                    c.LastAccessUtc = DateTime.UtcNow;
                    return c.Tiles[lx, ly];
                }
            }
            return null;
        }

        /// <summary>
        /// 根据世界坐标（瓦片单位）获取对应的瓦片 ID；若区块不存在则同步生成该区块。
        /// </summary>
        public int GetTileAtWorldOrGenerate(int worldX, int worldY)
        {
            WorldToChunkAndLocal(worldX, worldY, out int cx, out int cy, out int lx, out int ly);
            var c = GetOrCreateChunk(cx, cy);
            return c.Tiles[lx, ly];
        }

        /// <summary>
        /// 根据世界坐标（瓦片单位）请求在玩家所在区块周围生成/加载区块。
        /// </summary>
        public void RequestChunksAroundWorld(int worldX, int worldY)
        {
            WorldToChunkCoords(worldX, worldY, out int cx, out int cy);
            RequestChunksAround(cx, cy);
        }

        /// <summary>
        /// 将世界坐标（瓦片单位）转换为区块坐标。
        /// 处理负坐标，返回向下取整的区块索引。
        /// </summary>
        public void WorldToChunkCoords(int worldX, int worldY, out int chunkX, out int chunkY)
        {
            chunkX = FloorDiv(worldX, ChunkSize);
            chunkY = FloorDiv(worldY, ChunkSize);
        }

        /// <summary>
        /// 将世界坐标（瓦片单位）转换为区块内局部坐标（[0,ChunkSize-1]）。
        /// </summary>
        public void WorldToLocalCoords(int worldX, int worldY, out int localX, out int localY)
        {
            int cx = FloorDiv(worldX, ChunkSize);
            int cy = FloorDiv(worldY, ChunkSize);
            localX = worldX - cx * ChunkSize;
            localY = worldY - cy * ChunkSize;
        }

        /// <summary>
        /// 同时计算区块坐标与区块内局部坐标。
        /// </summary>
        public void WorldToChunkAndLocal(int worldX, int worldY, out int chunkX, out int chunkY, out int localX, out int localY)
        {
            chunkX = FloorDiv(worldX, ChunkSize);
            chunkY = FloorDiv(worldY, ChunkSize);
            localX = worldX - chunkX * ChunkSize;
            localY = worldY - chunkY * ChunkSize;
        }

        private static int FloorDiv(int a, int b)
        {
            if (b <= 0) throw new ArgumentOutOfRangeException(nameof(b));
            int d = a / b;
            int r = a % b;
            if (r != 0 && ((a < 0) != (b < 0))) d -= 1; // 当 a 为负且有余数时向下取整
            return d;
        }

        /// <summary>
        /// 通过调用提供的保存回调来保存所有脏区块。此方法不会移除区块。
        /// </summary>
        public void SaveDirtyChunks(Action<Chunk> saver)
        {
            if (saver == null) return;
            List<Chunk> copy;
            lock (@lock)
            {
                copy = new List<Chunk>(chunks.Values);
            }
            foreach (var c in copy)
            {
                if (c.Dirty) saver(c);
            }
        }

        /// <summary>
        /// 确保缓存限制（按 MaxCachedChunks 或 MaxMemoryBytes）。
        /// 采用 LRU（基于 LastAccessUtc）策略回收最久未访问的区块。
        /// </summary>
        private void EnsureCacheLimits()
        {
            if (MaxCachedChunks <= 0 && MaxMemoryBytes <= 0) return;
            // 估算每块内存：瓦片数量 * 4 字节（int）
            long bytesPerChunk = (long)ChunkSize * ChunkSize * sizeof(int);

            lock (@lock)
            {
                if (MaxCachedChunks > 0 && chunks.Count <= MaxCachedChunks && (MaxMemoryBytes <= 0 || chunks.Count * bytesPerChunk <= MaxMemoryBytes))
                {
                    return;
                }

                // 按 LastAccessUtc 升序排序（最久未访问的先移除）
                var ordered = chunks.OrderBy(kv => kv.Value.LastAccessUtc).ToList();
                int idx = 0;
                while (idx < ordered.Count && ((MaxCachedChunks > 0 && chunks.Count > MaxCachedChunks) || (MaxMemoryBytes > 0 && chunks.Count * bytesPerChunk > MaxMemoryBytes)))
                {
                    var key = ordered[idx].Key;
                    // 在真实场景应先保存脏块
                    if (chunks.Remove(key))
                    {
                        Logger?.Invoke($"因缓存限制回收区块 ({key.Item1},{key.Item2})");
                    }
                    idx++;
                }
            }
        }

        /// <summary>
        /// 将种子和区块坐标组合为用于每块 PRNG 的 uint 值。
        /// </summary>
        private uint MixSeed(uint cx, uint cy)
        {
            // 简单混合：使用一些常数与全局种子进行扰动
            uint h = seed;
            h ^= cx + 0x9E3779B9u + (h << 6) + (h >> 2);
            h ^= cy + 0x85EBCA6Bu + (h << 6) + (h >> 2);
            return h == 0 ? 0x9E3779B9u : h;
        }

        /// <summary>
        /// 32 位 xorshift 伪随机数生成步骤
        /// </summary>
        private static uint XorShift32(uint x)
        {
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            return x;
        }

        #region PerlinNoise Implementation
        /// <summary>
        /// 小型 Perlin 噪声实现（基于 Ken Perlin 算法，返回范围约为 [-1,1]）
        /// </summary>
        private class PerlinNoise
        {
            private readonly int[] perm;

            public PerlinNoise(int seed)
            {
                perm = new int[512];
                var rnd = new System.Random(seed);
                int[] p = new int[256];
                for (int i = 0; i < 256; i++) p[i] = i;
                // Fisher-Yates
                for (int i = 255; i > 0; i--)
                {
                    int j = rnd.Next(i + 1);
                    int tmp = p[i]; p[i] = p[j]; p[j] = tmp;
                }
                for (int i = 0; i < 512; i++) perm[i] = p[i & 255];
            }

            /// <summary>
            /// 计算二维 Perlin 噪声值。
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public double Noise(double x, double y)
            {
                // 找到单元格坐标
                int xi = FastFloor(x) & 255;
                int yi = FastFloor(y) & 255;
                double xf = x - Math.Floor(x);
                double yf = y - Math.Floor(y);

                double u = Fade(xf);
                double v = Fade(yf);

                int aa = perm[perm[xi] + yi];
                int ab = perm[perm[xi] + yi + 1];
                int ba = perm[perm[xi + 1] + yi];
                int bb = perm[perm[xi + 1] + yi + 1];

                double x1 = Lerp(Grad(aa, xf, yf), Grad(ba, xf - 1, yf), u);
                double x2 = Lerp(Grad(ab, xf, yf - 1), Grad(bb, xf - 1, yf - 1), u);
                double result = Lerp(x1, x2, v);
                // result 约在 [-1,1]
                return result;
            }
            /// <summary>
            /// 快速向下取整
            /// </summary>
            /// <param name="x"></param>
            /// <returns></returns>
            private static int FastFloor(double x) => (int)(x >= 0 ? x : x - 1);
            /// <summary>
            /// 平滑插值函数
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            private static double Fade(double t) => t * t * t * (t * (t * 6 - 15) + 10);
            /// <summary>
            /// 线性插值
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="t"></param>
            /// <returns></returns>
            private static double Lerp(double a, double b, double t) => a + t * (b - a);
            /// <summary>
            /// 梯度函数
            /// </summary>
            /// <param name="hash"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            private static double Grad(int hash, double x, double y)
            {
                int h = hash & 15;
                double u = h < 8 ? x : y;
                double v = h < 4 ? y : h == 12 || h == 14 ? x : 0;
                double res = ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
                return res;
            }
        }

        /// <summary>
        /// 分形噪声（分层叠加多个频率）
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="octaves"></param>
        /// <param name="persistence"></param>
        /// <returns></returns>
        private double FractalNoise(double x, double y, int octaves, double persistence)
        {
            double total = 0;
            double frequency = 1.0;
            double amplitude = 1.0;
            double maxValue = 0;  // 用于归一化
            for (int i = 0; i < octaves; i++)
            {
                total += noise.Noise(x * frequency, y * frequency) * amplitude;
                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= 2.0;
            }
            if (maxValue == 0) return 0;
            return total / maxValue;
        }
        #endregion
    }
}