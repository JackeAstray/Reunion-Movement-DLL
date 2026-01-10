using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Random
{
    /// <summary>
    /// 地牢随机数生成器，基于 System.Random 提供兼容的整数/浮点随机方法
    /// </summary>
    public class DungeonRandom
    {
        private System.Random rand;

        /// <summary>
        /// 返回一个非负的随机有符号整数（等同于 System.Random.Next()）。
        /// </summary>
        /// <returns>非负随机整数</returns>
        public int Next()
        {
            return rand.Next();
        }

        /// <summary>
        /// 返回 [0, x) 之间的随机有符号整数。
        /// </summary>
        /// <param name="x">上界（不包含），必须大于 0</param>
        /// <returns>随机整数，范围 [0, x)</returns>
        public int Next(int x)
        {
            return rand.Next(x);
        }

        /// <summary>
        /// 返回 [0, x) 之间的随机无符号整数。
        /// 注意：当 x 大于 int.MaxValue 时会溢出为负数，因此调用者应保证 x <= int.MaxValue。
        /// </summary>
        /// <param name="x">上界（不包含）</param>
        /// <returns>随机无符号整数，范围 [0, x)</returns>
        public uint Next(uint x)
        {
            return (uint)rand.Next((int)x);
        }

        /// <summary>
        /// 返回 [min, max) 之间的随机有符号整数。
        /// </summary>
        /// <param name="min">下界（包含）</param>
        /// <param name="max">上界（不包含）</param>
        /// <returns>随机整数，范围 [min, max)</returns>
        public int Next(int min, int max)
        {
            return rand.Next(min, max);
        }

        /// <summary>
        /// 返回 [min, max) 之间的随机有符号整数，min 为无符号整数类型的重载。
        /// </summary>
        /// <param name="min">下界（包含）</param>
        /// <param name="max">上界（不包含）</param>
        /// <returns>随机整数，范围 [min, max)</returns>
        public int Next(uint min, int max)
        {
            return rand.Next((int)min, max);
        }

        /// <summary>
        /// 返回 [min, max) 之间的随机有符号整数，max 为无符号整数类型的重载。
        /// </summary>
        /// <param name="min">下界（包含）</param>
        /// <param name="max">上界（不包含）</param>
        /// <returns>随机整数，范围 [min, max)</returns>
        public int Next(int min, uint max)
        {
            return rand.Next(min, (int)max);
        }

        /// <summary>
        /// 返回 [min, max) 之间的随机无符号整数。
        /// 注意：当边界超出 int 范围时可能导致转换问题，调用者应保证边界合适。
        /// </summary>
        /// <param name="min">下界（包含）</param>
        /// <param name="max">上界（不包含）</param>
        /// <returns>随机无符号整数，范围 [min, max)</returns>
        public uint Next(uint min, uint max)
        {
            return (uint)rand.Next((int)min, (int)max);
        }

        /// <summary>
        /// 返回 [0.0, 1.0) 之间的随机双精度浮点数。
        /// </summary>
        /// <returns>位于 [0.0, 1.0) 的随机双精度浮点数</returns>
        public double NextDouble()
        {
            return rand.NextDouble();
        }

        /// <summary>
        /// 以给定概率返回 true，否则返回 false。概率值为 [0,1) 时效果最佳。
        /// </summary>
        /// <param name="probability">期望概率值</param>
        /// <returns>是否发生（true 表示发生）</returns>
        public bool Probability(double probability)
        {
            return probability >= NextDouble();
        }

        /// <summary>
        /// 构造函数。
        /// 若未提供种子则使用无参 System.Random 的默认初始化，否则使用给定种子构造 System.Random。
        /// </summary>
        /// <param name="seed">可选的整数种子</param>
        public DungeonRandom(int? seed = null)
        {
            rand = seed == null ? new System.Random() : new System.Random((int)seed);
        }
    }
}
