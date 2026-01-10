using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Util
{
    /// <summary>
    /// 简单的二元组容器，用于在算法中携带两个相关对象（First, Second）。
    /// </summary>
    public class Pair
    {
        /// <summary>
        /// 第一个值，可存放任意对象。
        /// </summary>
        public object First;

        /// <summary>
        /// 第二个值，可存放任意对象。
        /// </summary>
        public object Second;

        /// <summary>
        /// 默认构造函数，初始化为空的二元组。
        /// </summary>
        public Pair()
        {
        }

        /// <summary>
        /// 使用指定的两个对象初始化二元组。
        /// </summary>
        /// <param name="x">第一个对象</param>
        /// <param name="y">第二个对象</param>
        public Pair(object x, object y)
        {
            First = x;
            Second = y;
        }
    }
}