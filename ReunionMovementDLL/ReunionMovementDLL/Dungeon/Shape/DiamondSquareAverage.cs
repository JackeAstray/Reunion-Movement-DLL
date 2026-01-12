using ReunionMovementDLL.Dungeon.Random;
using System;

namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// Diamond-Square 平均高度生成算法的辅助静态类。
    /// 用于在整数矩阵上以分形方式生成高度（或权重）值。
    /// </summary>
    public static class DiamondSquareAverage
    {
        /// <summary>
        /// 递归地在给定矩阵上执行 Diamond-Square 算法的一个步骤：
        /// 在以 (startX + x, startY + y) 为中心、边长为 size*2 的正方形内计算中心点和边中点的高度值，
        /// 并根据给定的四个顶点高度 t1..t4 以及一个随机偏移量将结果写回矩阵。
        /// 然后将区域细分为四个子区域并对每个子区域递归调用自身。
        /// </summary>
        /// <typeparam name="TRand">实现了 <see cref="IRandomable"/> 的随机数生成器类型。</typeparam>
        /// <param name="matrix">目标高度矩阵，调用者负责保证索引访问合法。</param>
        /// <param name="startX">矩阵中用于偏移的起始 X 坐标（列偏移）。</param>
        /// <param name="startY">矩阵中用于偏移的起始 Y 坐标（行偏移）。</param>
        /// <param name="x">当前子区域中心相对于 startX 的 X 偏移（列索引）。</param>
        /// <param name="y">当前子区域中心相对于 startY 的 Y 偏移（行索引）。</param>
        /// <param name="size">当前步长的一半（用于定位边中点），当 size == 0 时结束递归。</param>
        /// <param name="t1">左上角顶点的高度值（或权重）。</param>
        /// <param name="t2">右上角顶点的高度值（或权重）。</param>
        /// <param name="t3">左下角顶点的高度值（或权重）。</param>
        /// <param name="t4">右下角顶点的高度值（或权重）。</param>
        /// <param name="maxValue">（未在当前实现中使用）保留的最大值参数。</param>
        /// <param name="addAltitude">用于控制随机偏移范围的参数；传入到 func 以缩减或变化偏移。
        /// 在递归调用中会通过 func(addAltitude) 传入子级。
        /// </param>
        /// <param name="rand">用于生成随机偏移的随机数生成器，必须实现 <see cref="IRandomable"/>。</param>
        /// <param name="func">用于调整 addAltitude 的函数（例如衰减函数），函数接受当前 addAltitude 并返回子级的值。</param>
        /// <remarks>
        /// 算法要点：
        /// 1. 计算中心点 X = (t1 + t2 + t3 + t4) / 4，并加入随机偏移（范围由 addAltitude 控制）；
        /// 2. 计算四个边的中点 s1..s4（相邻顶点的平均值）;
        /// 3. 将中心点与边中点写回矩阵；
        /// 4. 将边长折半并对四个子正方形递归执行该过程。
        /// 注意：调用者需负责矩阵边界检查与初始顶点值的设置。
        /// </remarks>
        public static void CreateDiamondSquareAverage<TRand>(
            int[,] matrix,
            uint startX,
            uint startY,
            uint x,
            uint y,
            uint size,
            int t1,
            int t2,
            int t3,
            int t4,
            int maxValue,
            int addAltitude,
            TRand rand,
            Func<int, int> func) where TRand : IRandomable
        {

            if (size == 0) return;
            int vertexRand = (int)rand.Next((uint)addAltitude);
            int vertexHeight = t1 / 4 + t2 / 4 + t3 / 4 + t4 / 4;
            matrix[startY + y, startX + x] = vertexHeight + vertexRand;

            int s1 = (int)t1 / 2 + t2 / 2;
            int s2 = (int)t1 / 2 + t3 / 2;
            int s3 = (int)t2 / 2 + t4 / 2;
            int s4 = (int)t3 / 2 + t4 / 2;

            matrix[startY + y + size, startX + x] = s3;
            matrix[startY + y - size, startX + x] = s2;
            matrix[startY + y, startX + x + size] = s4;
            matrix[startY + y, startX + x - size] = s1;
            size /= 2;

            CreateDiamondSquareAverage(matrix, startX, startY, x - size, y - size, size, t1, s1, s2,
                matrix[startY + y, startX + x], maxValue, func(addAltitude), rand, func);
            CreateDiamondSquareAverage(matrix, startX, startY, x - size, y + size, size, s1, t2,
                matrix[startY + y, startX + x], s3, maxValue, func(addAltitude), rand, func);
            CreateDiamondSquareAverage(matrix, startX, startY, x + size, y - size, size, s2,
                matrix[startY + y, startX + x], t3, s4, maxValue, func(addAltitude), rand, func);
            CreateDiamondSquareAverage(matrix, startX, startY, x + size, y + size, size,
                matrix[startY + y, startX + x], s3, s4, t4, maxValue, func(addAltitude), rand, func);
        }
    }
}
/**
 * Algorithm
 * t1 &&&&&&&&&&&&&&&&&&& t2
 * &                       &
 * &                       &
 * &                       &
 * &           X           &
 * &                       &
 * &                       &
 * &                       &
 * t3 &&&&&&&&&&&&&&&&&&& t4
 *
 * 1. calculate X value. (t1 + t2 + t3 + t4) / 4
 * 2. add offset. X + rand. rand depends on addAltitude.
 *
 * t1 &&&&&&&&&s1&&&&&&&& t2
 * &                       &
 * &                       &
 * &                       &
 * s1           X          s3
 * &                       &
 * &                       &
 * &                       &
 * t3 &&&&&&&&&s4&&&&&&&& t4
 *
 * 3. calculate s1 & s2 & s3 & s4. midpoint of (ti, tj) (i, j) is 0 ~ 3 
 * 4. recursive
 */