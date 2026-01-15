using ReunionMovementDLL.Dungeon.Base;
using ReunionMovementDLL.Dungeon.Random;
using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// TinyKeep 风格的 RogueLike 生成器（Delaunay/MST 风格近似实现）。
    /// 名称为 RogueLikeTinyKeep。
    /// 实现流程：采样房间中心 -> 放置房间 -> 计算最小生成树（Prim） -> 生成走廊 -> 可选添加额外边形成环路。
    /// 注：为简化实现，三角化采用全连边集合再构造 MST（适用于房间数较少的场景）。
    /// </summary>
    public sealed class RogueLikeTinyKeep : RectBaseRogueLike<RogueLikeTinyKeep>, IDrawer<int>
    {
        private DungeonRandom rand = new DungeonRandom();

        // 支持固定房间模板（相对坐标）
        private List<RogueLikeOutputNumber> fixedRooms = new List<RogueLikeOutputNumber>();

        /// <summary>
        /// 设置固定房间模板（相对绘制区域坐标）。调用后这些房间将在随机房间生成前尝试放置。
        /// </summary>
        /// <param name="rooms">房间模板列表（相对坐标 x,y,w,h）。</param>
        /// <returns>当前实例，便于链式调用。</returns>
        public RogueLikeTinyKeep SetFixedRooms(List<RogueLikeOutputNumber> rooms)
        {
            this.fixedRooms = rooms ?? new List<RogueLikeOutputNumber>();
            return this;
        }

        /// <summary>
        /// 清除所有固定房间模板。
        /// </summary>
        /// <returns>当前实例。</returns>
        public RogueLikeTinyKeep ClearFixedRooms()
        {
            this.fixedRooms.Clear();
            return this;
        }

        /// <summary>
        /// 默认构造函数，创建 RogueLikeTinyKeep 的实例。
        /// </summary>
        public RogueLikeTinyKeep()
        {
        }

        /// <summary>
        /// 使用矩阵范围构造器，设置绘制区域。
        /// </summary>
        /// <param name="matrixRange">矩阵范围（起点与大小）。param>
        public RogueLikeTinyKeep(MatrixRange matrixRange) : base(matrixRange) { }

        /// <summary>
        /// 指定绘制值与最大通路数的构造函数。
        /// </summary>
        /// <param name="drawValue">绘制值列表（RogueLikeList）。param>
        /// <param name="maxWay">最大通路数。</param>
        public RogueLikeTinyKeep(RogueLikeList drawValue, uint maxWay) : base(drawValue, maxWay) { }

        /// <summary>
        /// 在给定矩阵上绘制地牢（入口方法）。
        /// 如果起始坐标越界返回 false，否则调用核心绘制函数 DrawNormal.
        /// </summary>
        /// <param name="matrix">目标矩阵（整型二维数组）。param>
        /// <returns>是否成功绘制。</returns>
        public bool Draw(int[,] matrix)
        {
            if (startX >= MatrixUtil.GetX(matrix) || startY >= MatrixUtil.GetY(matrix)) return false;
            return DrawNormal(matrix);
        }

        /// <summary>
        /// 带日志的绘制入口（当前仅返回日志占位），实际行为与 Draw 相同。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="log">输出日志（out）。</param>
        /// <returns>是否成功绘制。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            log = string.Empty;
            return Draw(matrix);
        }

        /// <summary>
        /// 在矩阵上创建地牢并返回该矩阵引用（便捷包装器）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <returns>被修改的矩阵引用。</returns>
        public int[,] Create(int[,] matrix)
        {
            this.Draw(matrix);
            return matrix;
        }

        /// <summary>
        /// 核心绘制逻辑：采样房间中心、放置房间、构建全连边并计算最小生成树（Prim），
        /// 最后在 MST 的边上开凿走廊并可选添加额外边以形成循环。
        /// 支持在生成前放置若干固定房间模板，其他房间按随机采样分布。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <returns>是否成功生成地牢。</returns>
        private bool DrawNormal(int[,] matrix)
        {
            var width = MatrixUtil.GetX(matrix);
            var height = MatrixUtil.GetY(matrix);
            var areaW = (int)(width - startX);
            var areaH = (int)(height - startY);

            if (areaW <= 4 || areaH <= 4) return false;

            // 使用简单拒绝抽样方法对房间中心进行采样，以确保最小间距
            var minSep = Math.Max(2, (int)((roomRange.x + roomRange.w) / 2));
            // 当房间数非常多时，允许采样更多点；但不超过 maxWay 且不超过区域可容纳数量
            int estimatedCapacity = Math.Max(1, (areaW * areaH) / Math.Max(1, minSep * minSep));
            int target = (int)Math.Max(3, Math.Min((int)maxWay, Math.Max(20, estimatedCapacity)));

            // 先尝试放置固定房间模板（如果有）
            var rooms = new List<RogueLikeOutputNumber>();
            var fixedCenters = new List<PairInt>();
            if (fixedRooms != null && fixedRooms.Count > 0)
            {
                foreach (var f in fixedRooms)
                {
                    // 尝试放置；PlaceRoom 参数为相对坐标，矩阵使用 startX/startY 偏移
                    if (PlaceRoom(matrix, areaW, areaH, f, rogueLikeList.roomId))
                    {
                        rooms.Add(new RogueLikeOutputNumber(f.x, f.y, f.w, f.h));
                        fixedCenters.Add(new PairInt(f.x + f.w / 2, f.y + f.h / 2));
                    }
                }
            }

            // 只采样剩余的房间中心数量，避免与固定中心冲突
            int remaining = target - fixedCenters.Count;
            if (remaining < 0) remaining = 0;

            var centers = SamplePoints(areaW, areaH, remaining, minSep, fixedCenters);
            if (fixedCenters.Count + centers.Count < 2) return false;

            // 放置采样到的房间
            foreach (var c in centers)
            {
                var rw = rand.Next(roomRange.x, roomRange.x + roomRange.w);
                var rh = rand.Next(roomRange.y, roomRange.y + roomRange.h);
                int rx = c.X - rw / 2;
                int ry = c.Y - rh / 2;
                var room = new RogueLikeOutputNumber(rx, ry, rw, rh);
                if (PlaceRoom(matrix, areaW, areaH, room, rogueLikeList.roomId))
                {
                    rooms.Add(room);
                }
            }

            if (rooms.Count < 2) return false;

            // 点集合（房间中心）
            var pts = rooms.Select(r => new PairInt(r.x + r.w / 2, r.y + r.h / 2)).ToArray();

            // 使用带堆的 Prim 算法构建最小生成树（减少选择成本）
            int n = pts.Length;
            var inMst = new bool[n];
            var minDist = new int[n];
            var parent = new int[n];
            for (int i = 0; i < n; ++i) { minDist[i] = int.MaxValue; parent[i] = -1; }

            var mst = new List<Edge>();

            var heap = new MinHeap();
            heap.Push(new HeapNode(0, 0, -1));

            while (heap.Count > 0)
            {
                var node = heap.Pop();
                int u = node.Vertex;
                if (inMst[u]) continue;
                inMst[u] = true;
                if (node.Parent != -1)
                {
                    mst.Add(new Edge(node.Parent, u, node.Key));
                }

                // 更新邻居距离（complete graph distances)
                for (int v = 0; v < n; ++v)
                {
                    if (inMst[v] || v == u) continue;
                    var dx = pts[u].X - pts[v].X;
                    var dy = pts[u].Y - pts[v].Y;
                    var d = dx * dx + dy * dy;
                    if (d < minDist[v])
                    {
                        minDist[v] = d;
                        parent[v] = u;
                        heap.Push(new HeapNode(d, v, u));
                    }
                }
            }

            // 使用基于空间分桶的 k-NN 策略生成候选额外边，避免全连边遍历
            var added = new HashSet<long>();
            foreach (var e in mst)
            {
                int a = Math.Min(e.U, e.V);
                int b = Math.Max(e.U, e.V);
                added.Add(((long)a << 32) | (uint)b);
            }

            var candidates = new List<Edge>();

            // build grid
            int cellSize = Math.Max(1, minSep);
            int gridW = Math.Max(1, (areaW + cellSize - 1) / cellSize);
            int gridH = Math.Max(1, (areaH + cellSize - 1) / cellSize);
            var grid = new List<int>[gridW, gridH];
            for (int gx = 0; gx < gridW; ++gx)
                for (int gy = 0; gy < gridH; ++gy)
                    grid[gx, gy] = new List<int>();

            for (int i = 0; i < n; ++i)
            {
                int gx = pts[i].X / cellSize;
                int gy = pts[i].Y / cellSize;
                if (gx < 0) gx = 0; if (gx >= gridW) gx = gridW - 1;
                if (gy < 0) gy = 0; if (gy >= gridH) gy = gridH - 1;
                grid[gx, gy].Add(i);
            }

            int k = Math.Min(8, Math.Max(1, n - 1));
            int maxRadius = Math.Max(gridW, gridH);

            for (int i = 0; i < n; ++i)
            {
                int gx = pts[i].X / cellSize;
                int gy = pts[i].Y / cellSize;
                if (gx < 0) gx = 0; if (gx >= gridW) gx = gridW - 1;
                if (gy < 0) gy = 0; if (gy >= gridH) gy = gridH - 1;

                var localSet = new HashSet<int>();
                var neighbours = new List<KeyValuePair<int, int>>(); // (dist, idx)

                for (int r = 0; r <= maxRadius && neighbours.Count < k * 4; ++r)
                {
                    int minX = Math.Max(0, gx - r);
                    int maxX = Math.Min(gridW - 1, gx + r);
                    int minY = Math.Max(0, gy - r);
                    int maxY = Math.Min(gridH - 1, gy + r);

                    for (int xx = minX; xx <= maxX; ++xx)
                    {
                        for (int yy = minY; yy <= maxY; ++yy)
                        {
                            foreach (var idx in grid[xx, yy])
                            {
                                if (idx == i) continue;
                                if (localSet.Contains(idx)) continue;
                                localSet.Add(idx);
                                var dx = pts[i].X - pts[idx].X;
                                var dy = pts[i].Y - pts[idx].Y;
                                neighbours.Add(new KeyValuePair<int, int>(dx * dx + dy * dy, idx));
                            }
                        }
                    }
                }

                if (neighbours.Count == 0) continue;
                neighbours.Sort((a, b) => a.Key.CompareTo(b.Key));
                int take = Math.Min(k, neighbours.Count);
                for (int t = 0; t < take; ++t)
                {
                    int v = neighbours[t].Value;
                    int a = Math.Min(i, v);
                    int b = Math.Max(i, v);
                    long key = ((long)a << 32) | (uint)b;
                    if (added.Contains(key)) continue;
                    added.Add(key);
                    candidates.Add(new Edge(i, v, neighbours[t].Key));
                }
            }

            // shuffle candidates
            for (int i = candidates.Count - 1; i > 0; --i)
            {
                int j = (int)rand.Next((uint)(i + 1));
                var tmp = candidates[i]; candidates[i] = candidates[j]; candidates[j] = tmp;
            }

            // pick extra edges up到 target
            int extraTarget = Math.Min(n / 3, 100);
            var extraEdges = new List<Edge>();
            for (int i = 0; i < candidates.Count && extraEdges.Count < extraTarget; ++i)
            {
                if (!rand.Probability(0.2)) continue;
                extraEdges.Add(candidates[i]);
            }

            // 在 MST 边与额外边上开凿走廊
            foreach (var e in mst.Concat(extraEdges))
            {
                CarveCorridor(matrix, pts[e.U], pts[e.V], rogueLikeList.wayId, areaW, areaH);
            }

            return true;
        }

        /// <summary>
        /// 简单的拒绝采样：在给定区域内采样若干整数坐标点，保证彼此距离至少 minSep。
        /// 返回的坐标为相对于绘制范围（不含 startX/startY 偏移）的整数点。
        /// 支持传入已存在的点集合（例如固定房间中心），新采样点会避开这些点。
        /// </summary>
        /// <param name="areaW">绘制区域宽度（相对值）。param>
        /// <param name="areaH">绘制区域高度（相对值）。param>
        /// <param name="target">目标采样点数量。</param>
        /// <param name="minSep">采样点间最小欧式距离（整数）。</param>
        /// <param name="existing">可选的已存在点列表，新的点会避免这些点。</param>
        /// <returns>采样到的点列表（PairInt）</returns>
        private List<PairInt> SamplePoints(int areaW, int areaH, int target, int minSep, List<PairInt> existing = null)
        {
            var list = new List<PairInt>();
            int attempts = 0;
            int maxAttempts = Math.Max(target * 40, 2000);
            existing = existing ?? new List<PairInt>();
            while (list.Count < target && attempts < maxAttempts)
            {
                attempts++;
                // 在内部相对坐标中
                int x = (int)rand.Next(1, (uint)Math.Max(2, (uint)(areaW - 2)));
                int y = (int)rand.Next(1, (uint)Math.Max(2, (uint)(areaH - 2)));
                bool ok = true;
                foreach (var p in list)
                {
                    var dx = p.X - x;
                    var dy = p.Y - y;
                    if (dx * dx + dy * dy < minSep * minSep)
                    {
                        ok = false; break;
                    }
                }
                if (!ok) continue;
                foreach (var p in existing)
                {
                    var dx = p.X - x;
                    var dy = p.Y - y;
                    if (dx * dx + dy * dy < minSep * minSep)
                    {
                        ok = false; break;
                    }
                }
                if (ok) list.Add(new PairInt(x, y));
            }
            return list;
        }

        /// <summary>
        /// 在矩阵上放置房间（与 PlaceOutputNumber 类似但使用相对坐标），并在周边标记内墙。
        /// 若目标区域已有非 outsideWallId 的瓦片则放置失败。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="areaW">相对面积宽度。</param>
        /// <param name="areaH">相对区域高度。</param>
        /// <param name="rect">房间矩形（相对坐标）。param>
        /// <param name="tile">房间填充值（例如 roomId）。</param>
        /// <returns>放置是否成功。</returns>
        private bool PlaceRoom(int[,] matrix, int areaW, int areaH, RogueLikeOutputNumber rect, int tile)
        {
            if (rect.x < 1 || rect.y < 1 || rect.x + rect.w > areaW - 1 || rect.y + rect.h > areaH - 1)
            {
                return false;
            }
            // 检查区域是否为空
            for (int y = rect.y; y < rect.y + rect.h; ++y)
            {
                for (int x = rect.x; x < rect.x + rect.w; ++x)
                {
                    if (matrix[startY + y, startX + x] != rogueLikeList.outsideWallId)
                        return false;
                }
            }
            // 放置房间并标记内墙
            for (int y = rect.y - 1; y < rect.y + rect.h + 1; ++y)
            {
                for (int x = rect.x - 1; x < rect.x + rect.w + 1; ++x)
                {
                    if (y == rect.y - 1 || x == rect.x - 1 || y == rect.y + rect.h || x == rect.x + rect.w)
                    {
                        matrix[startY + y, startX + x] = rogueLikeList.insideWallId;
                    }
                    else
                    {
                        matrix[startY + y, startX + x] = tile;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 在两个点之间开凿 L 型走廊（先横后纵或先纵后横），并在走廊周边标记内墙。
        /// 坐标为相对于绘制区域（未加 startX/startY 偏移）。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="a">起点（相对坐标）。param>
        /// <param name="b">终点（相对坐标）。param>
        /// <param name="wayTile">走廊填充值（wayId）。param>
        /// <param name="areaW">相对区域宽度。</param>
        /// <param name="areaH">相对区域高度。</param>
        private void CarveCorridor(int[,] matrix, PairInt a, PairInt b, int wayTile, int areaW, int areaH)
        {
            int x1 = a.X, y1 = a.Y, x2 = b.X, y2 = b.Y;
            // 随机选择先横后纵或先纵后横
            if (rand.Probability(0.5))
            {
                // 先横后纵
                CarveStraight(matrix, x1, y1, x2, y1, wayTile, areaW, areaH);
                CarveStraight(matrix, x2, y1, x2, y2, wayTile, areaW, areaH);
            }
            else
            {
                // 先纵后横
                CarveStraight(matrix, x1, y1, x1, y2, wayTile, areaW, areaH);
                CarveStraight(matrix, x1, y2, x2, y2, wayTile, areaW, areaH);
            }
        }

        /// <summary>
        /// 在矩形直线段上开凿直走廊（水平或垂直），并把周边尚为 outside 的格子标记为 insideWallId。
        /// </summary>
        /// <param name="matrix">目标矩阵。</param>
        /// <param name="x1">起点 X（相对坐标）。</param>
        /// <param name="y1">起点 Y（相对坐标）。</param>
        /// <param name="x2">终点 X（相对坐标）。</param>
        /// <param name="y2">终点 Y（相对坐标）。</param>
        /// <param name="wayTile">走廊填充值。</param>
        /// <param name="areaW">相对区域宽度。</param>
        /// <param name="areaH">相对区域高度。</param>
        private void CarveStraight(int[,] matrix, int x1, int y1, int x2, int y2, int wayTile, int areaW, int areaH)
        {
            int sx = Math.Min(x1, x2);
            int ex = Math.Max(x1, x2);
            int sy = Math.Min(y1, y2);
            int ey = Math.Max(y1, y2);

            // 开凿走廊
            for (int y = sy; y <= ey; ++y)
            {
                for (int x = sx; x <= ex; ++x)
                {
                    if (x < 0 || y < 0 || x >= areaW || y >= areaH) continue;

                    // 仅当目标格不是房间地面时才开凿走廊，避免覆盖房间
                    if (matrix[startY + y, startX + x] != rogueLikeList.roomId)
                    {
                        matrix[startY + y, startX + x] = wayTile;
                    }
                }
            }

            // 标记内墙
            for (int y = sy - 1; y <= ey + 1; ++y)
            {
                for (int x = sx - 1; x <= ex + 1; ++x)
                {
                    if (x < 0 || y < 0 || x >= areaW || y >= areaH) continue;
                    // 仅标记尚为 outsideWallId 的格子
                    if (matrix[startY + y, startX + x] == rogueLikeList.outsideWallId)
                    {
                        matrix[startY + y, startX + x] = rogueLikeList.insideWallId;
                    }
                }
            }
        }

        /// <summary>
        /// 简单的整数坐标对结构，代表相对于绘制区域的点。
        /// </summary>
        private struct PairInt { public int X; public int Y; public PairInt(int x, int y) { X = x; Y = y; } }

        /// <summary>
        /// 边结构：用于存储两个顶点索引与权重（此处为距离平方）。
        /// </summary>
        private class Edge { public int U; public int V; public int Weight; public Edge(int u, int v, int w) { U = u; V = v; Weight = w; } }

        /// <summary>
        /// 最小堆节点。
        /// </summary>
        private struct HeapNode { public int Key; public int Vertex; public int Parent; public HeapNode(int k, int v, int p) { Key = k; Vertex = v; Parent = p; } }

        /// <summary>
        /// 简单二叉堆实现的最小堆（用于 Prim）。
        /// </summary>
        private class MinHeap
        {
            private List<HeapNode> data = new List<HeapNode>();
            public int Count => data.Count;
            public void Push(HeapNode node)
            {
                data.Add(node);
                int i = data.Count - 1;
                while (i > 0)
                {
                    int p = (i - 1) >> 1;
                    if (data[p].Key <= data[i].Key) break;
                    var tmp = data[p]; data[p] = data[i]; data[i] = tmp;
                    i = p;
                }
            }
            public HeapNode Pop()
            {
                var ret = data[0];
                int last = data.Count - 1;
                data[0] = data[last];
                data.RemoveAt(last);
                int i = 0;
                while (true)
                {
                    int l = i * 2 + 1;
                    int r = l + 1;
                    int smallest = i;
                    if (l < data.Count && data[l].Key < data[smallest].Key) smallest = l;
                    if (r < data.Count && data[r].Key < data[smallest].Key) smallest = r;
                    if (smallest == i) break;
                    var tmp = data[i]; data[i] = data[smallest]; data[smallest] = tmp;
                    i = smallest;
                }
                return ret;
            }
        }
    }
}