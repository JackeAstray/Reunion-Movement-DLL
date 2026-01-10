using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Random
{
    /// <summary>
    /// 随机数生成基类，封装对具体随机生成器的调用并提供常用的分布转换方法
    /// </summary>
    public class RandomBase : IRandomable
    {
        private IRandomable rand;

        /// <summary>
        /// 以概率 p 返回 true，否则返回 false。
        /// 当 p 在 [0,1) 范围内时，返回 true 的概率近似为 p。
        /// </summary>
        /// <param name="p">概率值，期望在 [0, 1) 区间</param>
        /// <returns>是否发生（true 表示发生）</returns>
        public bool Probability(double p)
        {
            var q = UniformRealDistribution();
            return p > q;
        }

        /// <summary>
        /// 均匀实数分布 [0, 1)，基于内部随机数生成器的输出做归一化
        /// </summary>
        /// <returns>落在 [0,1) 的双精度浮点数</returns>
        public double UniformRealDistribution()
        {
            return Normalize(rand.Next());
        }

        /// <summary>
        /// 将无符号整数映射到 [0,1) 区间的归一化函数
        /// </summary>
        /// <param name="x">无符号整数输入</param>
        /// <returns>对应的双精度值，落在 [0,1)</returns>
        private double Normalize(uint x)
        {
            // 使用 uint.MaxValue + 1.0 作为分母，确保结果落在 [0,1)
            return (double)x / ((double)uint.MaxValue + 1.0);
        }

        /// <summary>
        /// 将有符号整数映射到 [-1,1] 区间的归一化函数（保留以兼容可能的调用）
        /// </summary>
        /// <param name="x">有符号整数输入</param>
        /// <returns>对应的双精度值</returns>
        private double Normalize(int x)
        {
            return (double)x / int.MaxValue;
        }

        /// <summary>
        /// 区间随机数生成 [min, max)。调用内部随机数生成器的同名方法
        /// </summary>
        /// <param name="min">区间下界（包含）</param>
        /// <param name="max">区间上界（不包含）</param>
        /// <returns>落在 [min, max) 的随机无符号整数</returns>
        public uint Next(uint min, uint max)
        {
            return rand.Next(min, max);
        }

        /// <summary>
        /// 区间随机数生成 [0, max)。调用内部随机数生成器的同名方法
        /// </summary>
        /// <param name="max">上界（不包含）</param>
        /// <returns>落在 [0, max) 的随机无符号整数</returns>
        public uint Next(uint max)
        {
            return rand.Next(max);
        }

        /// <summary>
        /// 随机数生成 [0, uint::max)，调用内部随机数生成器的同名方法
        /// </summary>
        /// <returns>无符号整数随机值</returns>
        public uint Next()
        {
            return rand.Next();
        }

        /// <summary>
        /// 默认构造函数，使用 XorShift128 作为默认内部随机数实现
        /// </summary>
        public RandomBase()
        {
            this.rand = new XorShift128();
        }

        /// <summary>
        /// 构造函数，使用提供的随机数生成器作为内部实现。若传入 null，则回退使用默认的 XorShift128。
        /// </summary>
        /// <param name="rand">实现了 IRandomable 的随机数生成器</param>
        public RandomBase(IRandomable rand)
        {
            this.rand = rand ?? new XorShift128();
        }
    }
}
