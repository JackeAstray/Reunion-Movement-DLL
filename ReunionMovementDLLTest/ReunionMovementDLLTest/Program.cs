using System;
using ReunionMovementDLL.Dungeon.Base;
using ReunionMovementDLL.Dungeon.Shape;
using ReunionMovementDLL.Dungeon.Range;

namespace ReunionMovementDLLTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                const int width = 80;
                const int height = 30;

                // 定义瓦片 ID：外墙、内墙、房间、入口、道路
                var drawList = new RogueLikeList(0, 1, 2, 3, 4);

                // 分别调用不同的生成示例
                RunRogueLikeClassic(drawList, width, height);
                RunRogueLikeTinyKeep(drawList, width, height);
                RunRogueLikeBSP(drawList, width, height);
                RunClusteringMaze(width, height + 1);
                RunBorder(width, height);
                RunRandomRect(width, height);

                // 其它 Shape 示例
                RunPerlinIsland(width, height);
                RunFractalIsland(width, height);
                RunDiamondSquareAverageIsland(width, height);
                RunDiamondSquareAverageCornerIsland(width, height);
            }
            catch (Exception ex)
            {
                Console.WriteLine("测试失败：" + ex.Message);
            }

            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }

        /// <summary>
        /// 运行经典 RogueLike 生成示例
        /// </summary>
        /// <param name="drawList"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        static void RunRogueLikeClassic(RogueLikeList drawList, int width, int height)
        {
            dynamic classic = new RogueLikeClassic(drawList);
            classic.SetPoint(0u, 0u).SetRange(0u, 0u, (uint)width, (uint)height).SetMaxWay(80u);
            GenerateAndPrint("RogueLikeClassic", classic, drawList, width, height);
        }

        /// <summary>
        /// 运行 TinyKeep RogueLike 生成示例
        /// </summary>
        /// <param name="drawList"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        static void RunRogueLikeTinyKeep(RogueLikeList drawList, int width, int height)
        {
            dynamic tiny = new RogueLikeTinyKeep(drawList, 80u);
            // 确保内部的 rogueLikeList 与我们绘制映射一致
            try { tiny.SetValue(drawList); } catch { }
            tiny.SetPoint(0u, 0u).SetRange(0u, 0u, (uint)width, (uint)height);
            GenerateAndPrint("RogueLikeTinyKeep", tiny, drawList, width, height);
        }

        /// <summary>
        /// 运行 BSP RogueLike 生成示例
        /// </summary>
        /// <param name="drawList"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        static void RunRogueLikeBSP(RogueLikeList drawList, int width, int height)
        {
            dynamic bsp = new RogueLikeBSP();
            // 将绘制映射应用到 BSP（注意：BSP 算法默认不一定会产生入口/出口标记）
            try { bsp.SetValue(drawList); } catch { }
            bsp.SetRoom(drawList.roomId).SetWay(drawList.wayId).SetPoint(0u, 0u).SetRange(0u, 0u, (uint)width, (uint)height);
            GenerateAndPrint("RogueLikeBSP", bsp, drawList, width, height);
        }

        /// <summary>
        /// 运行聚类迷宫生成示例
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        static void RunClusteringMaze(int width, int height)
        {
            // 使用明确的迷宫渲染器，因为通用着色会丢失迷宫结构
            var maze = new ClusteringMaze(1);
            maze.ExitCount = 2;
            maze.SetPoint(0u, 0u).SetRange(0u, 0u, (uint)width, (uint)height);

            // 准备一个初始化为0（空）的矩阵
            int[,] matrix = new int[height, width];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    matrix[y, x] = 0;

            // 生成迷宫
            maze.Draw(matrix);

            // 如有可用，请确定壁面值
            int wallValue = 1;
            try { wallValue = (int)maze.GetType().GetProperty("drawValue").GetValue(maze); } catch { }

            // 打印迷宫：墙 -> 完全遮挡，空 -> 空白
            Console.WriteLine();
            Console.WriteLine("=== 测试：ClusteringMaze ===");
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.Write(matrix[y, x] == wallValue ? ' ' : '@');
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 边框生成示例
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        static void RunBorder(int width, int height)
        {
            dynamic border = new Border(1);
            border.SetPoint(0u, 0u).SetRange(0u, 0u, (uint)width, (uint)height);
            GenerateAndPrintGeneric("Border", border, width, height);
        }

        /// <summary>
        /// 随机矩形生成示例
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        static void RunRandomRect(int width, int height)
        {
            var rnd = new RandomRect(3, 0.5);
            rnd.startX = 0u; rnd.startY = 0u; rnd.width = (uint)width; rnd.height = (uint)height;
            GenerateAndPrintGeneric("RandomRect", rnd, width, height);
        }

        // --- 其它 Shape 示例方法 ---

        static void RunPerlinIsland(int width, int height)
        {
            dynamic perlin = new PerlinIsland();
            perlin.SetPoint(0u, 0u).SetRange(0u, 0u, (uint)width, (uint)height);
            GenerateAndPrintGeneric("PerlinIsland", perlin, width, height);
        }

        static void RunFractalIsland(int width, int height)
        {
            dynamic fractal = new FractalIsland();
            fractal.SetPoint(0u, 0u).SetRange(0u, 0u, (uint)width, (uint)height);
            GenerateAndPrintGeneric("FractalIsland", fractal, width, height);
        }

        static void RunDiamondSquareAverageIsland(int width, int height)
        {
            dynamic ds = new DiamondSquareAverageIsland();
            ds.SetPoint(0u, 0u).SetRange(0u, 0u, (uint)width, (uint)height);
            GenerateAndPrintGeneric("DiamondSquareAverageIsland", ds, width, height);
        }

        static void RunDiamondSquareAverageCornerIsland(int width, int height)
        {
            dynamic dsCorner = new DiamondSquareAverageCornerIsland();
            dsCorner.SetPoint(0u, 0u).SetRange(0u, 0u, (uint)width, (uint)height);
            GenerateAndPrintGeneric("DiamondSquareAverageCornerIsland", dsCorner, width, height);
        }

        // 重用的打印函数（适合瓦片与高度图）
        static void GenerateAndPrintGeneric(string title, dynamic generator, int width, int height)
        {
            Console.WriteLine();
            Console.WriteLine("=== 测试：" + title + " ===");

            int[,] matrix = new int[height, width];

            // 默认初始化为 0
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    matrix[y, x] = 0;

            // 执行生成（大多数 Shape 提供 Create/Draw）
            try { generator.Create(matrix); } catch { generator.Draw(matrix); }

            // 找到最小/最大用于归一化
            int min = int.MaxValue, max = int.MinValue;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    var v = matrix[y, x];
                    if (v < min) min = v;
                    if (v > max) max = v;
                }

            string shades = " .:-=+*#%@";
            int range = Math.Max(1, max - min);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int idx = (int)((long)(matrix[y, x] - min) * (shades.Length - 1) / range);
                    Console.Write(shades[Math.Max(0, Math.Min(shades.Length - 1, idx))]);
                }
                Console.WriteLine();
            }
        }

        static void GenerateAndPrint(string title, dynamic generator, RogueLikeList drawList, int width, int height)
        {
            Console.WriteLine();
            Console.WriteLine("=== 测试：" + title + " ===");

            // 创建矩阵 [行, 列]
            int[,] matrix = new int[height, width];

            // 使用外墙瓦片填充
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    matrix[y, x] = drawList.outsideWallId;

            // 执行生成
            generator.Create(matrix);

            // 简单控制台渲染
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char c = TileChar(matrix[y, x], drawList);
                    Console.Write(c);
                }
                Console.WriteLine();
            }
        }

        static char TileChar(int value, RogueLikeList rl)
        {
            if (value == rl.outsideWallId) return ' ';
            if (value == rl.insideWallId) return '■';
            if (value == rl.roomId) return '□';
            if (value == rl.entranceId) return '△';
            if (value == rl.wayId) return '◇';
            // 未知瓦片的默认字符
            return '?';
        }
    }
}