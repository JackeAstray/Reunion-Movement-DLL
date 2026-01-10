using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Random
{
    /// <summary>
    /// XorShift128 随机数生成器的实现。
    /// 基于 xorshift128 算法，提供无符号整数范围的随机数输出，并支持区间采样。
    /// </summary>
    public class XorShift128 : IRandomable
    {
        private uint x = 123456789;
        private uint y = 362436069;
        private uint z = 521288629;
        private uint w = 88675123;

        /// <summary>
        /// 获取生成器可产生的最小值（uint 最小值）。
        /// </summary>
        /// <returns>uint.MinValue</returns>
        public uint Min()
        {
            return System.UInt32.MinValue;
        }

        /// <summary>
        /// 获取生成器可产生的最大值（uint 最大值）。
        /// </summary>
        /// <returns>uint.MaxValue</returns>
        public uint Max()
        {
            return System.UInt32.MaxValue;
        }

        /// <summary>
        /// 生成下一个无符号整数随机值，等价于 xorshift128 算法的下一步。
        /// </summary>
        /// <returns>生成的无符号整数</returns>
        public uint Next()
        {
            // xorshift128 算法实现
            var t = x ^ (x << 11);
            x = y;
            y = z;
            z = w;
            w = (w ^ (w >> 19)) ^ (t ^ (t << 8));
            return w;
        }

        /// <summary>
        /// 生成落在 [0, max) 的随机无符号整数。
        /// </summary>
        /// <param name="max">上界（不包含）。必须大于 0。</param>
        /// <returns>随机无符号整数，范围在 [0, max)</returns>
        /// <exception cref="ArgumentException">当 max 为 0 时抛出</exception>
        public uint Next(uint max)
        {
            if (max == 0) throw new ArgumentException("max 必须大于 0", nameof(max));
            return Next() % max;
        }

        /// <summary>
        /// 生成落在 [min, max) 的随机无符号整数。
        /// </summary>
        /// <param name="min">下界（包含）</param>
        /// <param name="max">上界（不包含）</param>
        /// <returns>落在 [min, max) 的随机无符号整数</returns>
        /// <exception cref="ArgumentException">当 min >= max 时抛出</exception>
        public uint Next(uint min, uint max)
        {
            if (min >= max) throw new ArgumentException("min 必须小于 max", nameof(min));
            return min + Next() % (max - min);
        }

        /// <summary>
        /// 用于根据输入种子初始化内部状态。该方法将种子与初始状态异或混合，并确保状态不全部为 0。
        /// </summary>
        /// <param name="gen">用于混合的种子</param>
        private void Init(uint gen)
        {
            // 将提供的种子混入状态中，避免直接替换已有状态以保持较好的初始熵
            x ^= gen;
            y ^= gen << 13 | gen >> 19; // 对不同状态使用略有不同的混合以增加散列效果
            z ^= gen << 17 | gen >> 15;
            w ^= gen << 5 | gen >> 27;

            // 确保状态不全为零，若发生则设定为非零默认值
            if (x == 0 && y == 0 && z == 0 && w == 0)
            {
                // 选择不为零的常量初始化
                x = 123456789u;
                y = 362436069u;
                z = 521288629u;
                w = 88675123u;
            }
        }

        /// <summary>
        /// 构造函数。可选地以提供的种子初始化生成器；若不提供则使用当前时间刻度的低 32 位作为种子。
        /// 避免使用 0 作为直接种子，会导致状态退化。
        /// </summary>
        /// <param name="seed">可选的无符号整数种子</param>
        public XorShift128(uint? seed = null)
        {
            // 如果没有提供种子，使用当前高精度时间的低 32 位
            uint gen = seed ?? (uint)(DateTime.Now.Ticks & 0xFFFFFFFFu);

            // 若 gen 为 0，使用一个非零常量作为后备种子
            if (gen == 0)
            {
                gen = 88675123u;
            }

            Init(gen);

            // 预热生成器若干次以扩大初始状态的混合
            for (int i = 0; i < 8; i++) Next();
        }
    }
}
