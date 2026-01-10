using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Util
{
    /// <summary>
    /// 矩阵工具类，提供常用的二维矩阵尺寸与最值计算方法。
    /// 所有方法对输入做空检查，并在矩阵尺寸不满足要求时抛出异常以提醒调用方。
    /// </summary>
    public static class MatrixUtil
    {
        /// <summary>
        /// 静态构造函数：用于初始化静态资源（当前无实现，仅用于添加构造函数注释）。
        /// </summary>
        static MatrixUtil()
        {
        }

        /// <summary>
        /// 获取矩阵的列数（X 方向长度）。
        /// </summary>
        /// <typeparam name="T">矩阵元素类型</typeparam>
        /// <param name="matrix">目标二维矩阵</param>
        /// <returns>列数，类型为 uint</returns>
        /// <exception cref="ArgumentNullException">当 matrix 为 null 时抛出</exception>
        public static uint GetX<T>(T[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));
            return (uint)matrix.GetLength(1);
        }

        /// <summary>
        /// 在给定行索引 y_ 有效时返回矩阵列数，否则返回 0。
        /// </summary>
        /// <typeparam name="T">矩阵元素类型</typeparam>
        /// <param name="matrix">目标二维矩阵</param>
        /// <param name="y_">要检查的行索引（int）</param>
        /// <returns>当 y_ 在 [0, GetY(matrix)-1] 范围内时返回列数，否则返回 0</returns>
        /// <exception cref="ArgumentNullException">当 matrix 为 null 时抛出</exception>
        public static uint GetX<T>(T[,] matrix, int y_)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));
            if (y_ < 0) return 0;
            var lengthY = MatrixUtil.GetY(matrix);
            return (uint)y_ < lengthY ? MatrixUtil.GetX(matrix) : 0;
        }

        /// <summary>
        /// 获取矩阵的行数（Y 方向长度）。
        /// </summary>
        /// <typeparam name="T">矩阵元素类型</typeparam>
        /// <param name="matrix">目标二维矩阵</param>
        /// <returns>行数，类型为 uint</returns>
        /// <exception cref="ArgumentNullException">当 matrix 为 null 时抛出</exception>
        public static uint GetY<T>(T[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));
            return (uint)matrix.GetLength(0);
        }

        /// <summary>
        /// 计算整型矩阵的最大值。
        /// </summary>
        /// <param name="matrix">目标整型矩阵</param>
        /// <returns>矩阵中的最大元素</returns>
        /// <exception cref="ArgumentNullException">当 matrix 为 null 时抛出</exception>
        /// <exception cref="ArgumentException">当矩阵任一维度为 0 时抛出</exception>
        public static int GetMax(int[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));
            int y = matrix.GetLength(0);
            int x = matrix.GetLength(1);
            if (x == 0 || y == 0) throw new ArgumentException("矩阵的维度必须为正数", nameof(matrix));

            int mMax = matrix[0, 0];
            for (int row = 0; row < y; ++row)
            {
                for (int col = 0; col < x; ++col)
                {
                    var v = matrix[row, col];
                    if (v > mMax) mMax = v;
                }
            }

            return mMax;
        }

        /// <summary>
        /// 计算单精度浮点矩阵的最大值。
        /// </summary>
        /// <param name="matrix">目标浮点矩阵</param>
        /// <returns>矩阵中的最大元素</returns>
        /// <exception cref="ArgumentNullException">当 matrix 为 null 时抛出</exception>
        /// <exception cref="ArgumentException">当矩阵任一维度为 0 时抛出</exception>
        public static float GetMax(float[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));
            int y = matrix.GetLength(0);
            int x = matrix.GetLength(1);
            if (x == 0 || y == 0) throw new ArgumentException("矩阵的维度必须为正数", nameof(matrix));

            float mMax = matrix[0, 0];
            for (int row = 0; row < y; ++row)
            {
                for (int col = 0; col < x; ++col)
                {
                    var v = matrix[row, col];
                    if (v > mMax) mMax = v;
                }
            }

            return mMax;
        }

        /// <summary>
        /// 计算双精度浮点矩阵的最大值。
        /// </summary>
        /// <param name="matrix">目标双精度浮点矩阵</param>
        /// <returns>矩阵中的最大元素</returns>
        /// <exception cref="ArgumentNullException">当 matrix 为 null 时抛出</exception>
        /// <exception cref="ArgumentException">当矩阵任一维度为 0 时抛出</exception>
        public static double GetMax(double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));
            int y = matrix.GetLength(0);
            int x = matrix.GetLength(1);
            if (x == 0 || y == 0) throw new ArgumentException("矩阵的维度必须为正数", nameof(matrix));

            double mMax = matrix[0, 0];
            for (int row = 0; row < y; ++row)
            {
                for (int col = 0; col < x; ++col)
                {
                    var v = matrix[row, col];
                    if (v > mMax) mMax = v;
                }
            }

            return mMax;
        }

        /// <summary>
        /// 计算整型矩阵的最小值。
        /// </summary>
        /// <param name="matrix">目标整型矩阵</param>
        /// <returns>矩阵中的最小元素</returns>
        /// <exception cref="ArgumentNullException">当 matrix 为 null 时抛出</exception>
        /// <exception cref="ArgumentException">当矩阵任一维度为 0 时抛出</exception>
        public static int GetMin(int[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));
            int y = matrix.GetLength(0);
            int x = matrix.GetLength(1);
            if (x == 0 || y == 0) throw new ArgumentException("矩阵的维度必须为正数", nameof(matrix));

            int mMin = matrix[0, 0];
            for (int row = 0; row < y; ++row)
            {
                for (int col = 0; col < x; ++col)
                {
                    var v = matrix[row, col];
                    if (v < mMin) mMin = v;
                }
            }

            return mMin;
        }

        /// <summary>
        /// 计算单精度浮点矩阵的最小值。
        /// </summary>
        /// <param name="matrix">目标浮点矩阵</param>
        /// <returns>矩阵中的最小元素</returns>
        /// <exception cref="ArgumentNullException">当 matrix 为 null 时抛出</exception>
        /// <exception cref="ArgumentException">当矩阵任一维度为 0 时抛出</exception>
        public static float GetMin(float[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));
            int y = matrix.GetLength(0);
            int x = matrix.GetLength(1);
            if (x == 0 || y == 0) throw new ArgumentException("矩阵的维度必须为正数", nameof(matrix));

            float mMin = matrix[0, 0];
            for (int row = 0; row < y; ++row)
            {
                for (int col = 0; col < x; ++col)
                {
                    var v = matrix[row, col];
                    if (v < mMin) mMin = v;
                }
            }

            return mMin;
        }

        /// <summary>
        /// 计算双精度浮点矩阵的最小值。
        /// </summary>
        /// <param name="matrix">目标双精度浮点矩阵</param>
        /// <returns>矩阵中的最小元素</returns>
        /// <exception cref="ArgumentNullException">当 matrix 为 null 时抛出</exception>
        /// <exception cref="ArgumentException">当矩阵任一维度为 0 时抛出</exception>
        public static double GetMin(double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));
            int y = matrix.GetLength(0);
            int x = matrix.GetLength(1);
            if (x == 0 || y == 0) throw new ArgumentException("矩阵的维度必须为正数", nameof(matrix));

            double mMin = matrix[0, 0];
            for (int row = 0; row < y; ++row)
            {
                for (int col = 0; col < x; ++col)
                {
                    var v = matrix[row, col];
                    if (v < mMin) mMin = v;
                }
            }

            return mMin;
        }
    }
}