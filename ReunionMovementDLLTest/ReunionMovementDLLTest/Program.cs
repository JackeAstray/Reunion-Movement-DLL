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

                // RogueLikeClassic
                dynamic classic = new RogueLikeClassic(drawList);
                classic.SetPoint(0u, 0u).SetRange(0u, 0u, (uint)width, (uint)height).SetMaxWay(80u);
                GenerateAndPrint("RogueLikeClassic", classic, drawList, width, height);

                // RogueLikeTinyKeep（使用 RogueLikeList 和 maxWay 的构造函数）
                dynamic tiny = new RogueLikeTinyKeep(drawList, 80u);
                // 确保内部的 rogueLikeList 与我们绘制映射一致
                try { tiny.SetValue(drawList); } catch { }
                tiny.SetPoint(0u, 0u).SetRange(0u, 0u, (uint)width, (uint)height);
                GenerateAndPrint("RogueLikeTinyKeep", tiny, drawList, width, height);

                // RogueLikeBSP（简单的 RogueLike，使用 SetRoom/SetWay 设置值）
                dynamic bsp = new RogueLikeBSP();
                // 将绘制映射应用到 BSP（注意：BSP 算法默认不一定会产生入口/出口标记）
                try { bsp.SetValue(drawList); } catch { }
                bsp.SetRoom(drawList.roomId).SetWay(drawList.wayId).SetPoint(0u, 0u).SetRange(0u, 0u, (uint)width, (uint)height);
                GenerateAndPrint("RogueLikeBSP", bsp, drawList, width, height);
            }
            catch (Exception ex)
            {
                Console.WriteLine("测试失败：" + ex.Message);
            }

            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
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
            if (value == rl.outsideWallId) return '□';
            if (value == rl.insideWallId) return '■';
            if (value == rl.roomId) return '☆';
            if (value == rl.entranceId) return '△';
            if (value == rl.wayId) return '◇';
            // 未知瓦片的默认字符
            return '?';
        }
    }
}