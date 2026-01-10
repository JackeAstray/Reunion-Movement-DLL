using ReunionMovementDLL.Dungeon.Random;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Util
{
    /// <summary>
    /// 常用数组/列表工具类，包含交换与洗牌算法。
    /// 提供泛型实现并依赖外部随机器接口 IRandomable，以便可插拔随机实现。
    /// </summary>
    public static class ArrayUtil
    {
        /// <summary>
        /// 交换两个值的引用。
        /// </summary>
        private static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        /// <summary>
        /// 交换列表中两个索引位置的元素。
        /// </summary>
        private static void Swap<T>(IList<T> list, int a, int b)
        {
            T tmp = list[a];
            list[a] = list[b];
            list[b] = tmp;
        }

        /// <summary>
        /// 使用 Fisher-Yates 算法随机打乱列表（就地修改）。
        /// 通过传入实现了 IRandomable 的随机数生成器获得可重复的随机序列。
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <typeparam name="TRand">随机生成器类型，实现 IRandomable</typeparam>
        /// <param name="list">要洗牌的列表</param>
        /// <param name="rand">随机数生成器（非 null）</param>
        public static void Shuffle<T, TRand>(IList<T> list, TRand rand) where TRand : IRandomable
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            int n = list.Count;
            for (int i = n - 1; i > 0; --i)
            {
                // 选择 [0, i] 之间的随机索引
                int j = (int)rand.Next((uint)(i + 1));
                Swap(list, i, j);
            }
        }

        /// <summary>
        /// 使用 Fisher-Yates 算法随机打乱数组（就地修改），并返回该数组引用。
        /// </summary>
        public static T[] Shuffle<T, TRand>(T[] array, TRand rand) where TRand : IRandomable
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            int n = array.Length;
            for (int i = n - 1; i > 0; --i)
            {
                int j = (int)rand.Next((uint)(i + 1));
                Swap(ref array[i], ref array[j]);
            }

            return array;
        }

        /// <summary>
        /// 将数组的前 max 个位置与数组随机位置交换以生成部分乱序。
        /// 若 max 超过数组长度，则以数组长度为上限。
        /// 返回被修改后的数组引用。
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <typeparam name="TRand">随机生成器类型</typeparam>
        /// <param name="array">要部分洗牌的数组</param>
        /// <param name="max">要操作的前缀长度（上限为 array.Length）</param>
        /// <param name="rand">随机生成器</param>
        public static T[] Shuffle<T, TRand>(T[] array, uint max, TRand rand) where TRand : IRandomable
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            uint arrayLength = (uint)array.Length;
            uint limit = Math.Min(max, arrayLength);
            for (uint i = 0; i < limit; ++i)
            {
                // 将前 limit 个元素与随机位置交换
                var j = rand.Next(arrayLength);
                Swap(ref array[i], ref array[j]);
            }

            return array;
        }
    }
}
