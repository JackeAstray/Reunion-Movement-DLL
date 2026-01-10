using ReunionMovementDLL.Dungeon.Shape;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Console
{
    /// <summary>
    /// 控制台输出器，用于将整型矩阵渲染为文本输出。
    /// 支持两种渲染模式：数字模式（直接输出每个单元的数字）和字符串模式（根据提供的判定函数选择 lhs 或 rhs）。
    /// </summary>
    public class OutputConsole : IDrawer<int>
    {
        /// <summary>
        /// 字符串模式下用于表示真（或选中）状态的文本。
        /// </summary>
        public string lhs { get; set; } = string.Empty;

        /// <summary>
        /// 字符串模式下用于表示假（或未选中）状态的文本。
        /// </summary>
        public string rhs { get; set; } = string.Empty;

        /// <summary>
        /// 状态判定函数；当非空时 Draw 会进入字符串模式并使用此函数判断每个单元应使用 lhs 还是 rhs。
        /// 该字段可能为 null（在默认构造器或未配置时）。
        /// </summary>
        private Func<int, bool>? func;

        /// <summary>
        /// 数字状态绘制工具类（私有）。
        /// 将矩阵中的整数按行列转换为文本，单元间用空格分隔。
        /// </summary>
        private static class StateNumber
        {
            /// <summary>
            /// 将整数矩阵渲染为文本输出（数字模式）。
            /// </summary>
            /// <param name="matrix">要渲染的矩阵。</param>
            /// <param name="log">输出的文本结果（通过 out 返回）。</param>
            /// <returns>若矩阵为 null 则返回 false 并将 log 置为空字符串；否则返回 true。</returns>
            public static bool Draw(int[,] matrix, out string log)
            {
                if (matrix == null)
                {
                    log = string.Empty;
                    return false;
                }

                var h = matrix.GetLength(0);
                var w = matrix.GetLength(1);

                var sb = new StringBuilder();
                sb.AppendLine();
                for (int i = 0; i < h; ++i)
                {
                    var row = new StringBuilder();
                    for (int j = 0; j < w; ++j)
                    {
                        row.Append(matrix[i, j].ToString());
                        if (j < w - 1) row.Append(' ');
                    }
                    sb.AppendLine(row.ToString());
                }

                log = sb.ToString();

                return true;
            }
        }

        /// <summary>
        /// 字符串状态绘制工具类（私有）。
        /// 根据判定函数将每个单元渲染为 lhs 或 rhs 字符串。
        /// </summary>
        private static class StateString
        {
            /// <summary>
            /// 将整数矩阵渲染为文本输出（字符串模式）。
            /// </summary>
            /// <param name="matrix">要渲染的矩阵。</param>
            /// <param name="lhs">判定为 true 时使用的字符串。</param>
            /// <param name="rhs">判定为 false 时使用的字符串。</param>
            /// <param name="func">用于判定单元状态的函数（必须非 null）。</param>
            /// <param name="log">输出的文本结果（通过 out 返回）。</param>
            /// <returns>若矩阵为 null 则返回 false 并将 log 置为空字符串；否则返回 true。</returns>
            public static bool Draw(int[,] matrix, string lhs, string rhs, Func<int, bool> func, out string log)
            {
                if (matrix == null)
                {
                    log = string.Empty;
                    return false;
                }

                var h = matrix.GetLength(0);
                var w = matrix.GetLength(1);

                var sb = new StringBuilder();
                sb.AppendLine();
                for (int i = 0; i < h; ++i)
                {
                    var row = new StringBuilder();
                    for (int j = 0; j < w; ++j)
                        row.Append(func(matrix[i, j]) ? lhs : rhs);
                    sb.AppendLine(row.ToString());
                }

                log = sb.ToString();

                return true;
            }
        }

        /// <summary>
        /// 根据当前配置绘制矩阵为文本。
        /// 当 lhs、rhs 均非空且已提供判定函数时，使用字符串模式；否则使用数字模式。
        /// </summary>
        /// <param name="matrix">要渲染的矩阵。</param>
        /// <param name="log">渲染结果文本（通过 out 返回）。</param>
        /// <returns>渲染是否成功。</returns>
        public bool Draw(int[,] matrix, out string log)
        {
            if (matrix == null)
            {
                log = string.Empty;
                return false;
            }

            if (!string.IsNullOrEmpty(lhs) && !string.IsNullOrEmpty(rhs) && func != null)
            {
                return StateString.Draw(matrix, lhs, rhs, func, out log);
            }
            else
            {
                return StateNumber.Draw(matrix, out log);
            }
        }

        public bool Draw(int[,] matrix)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 默认构造函数，创建一个默认配置的输出器（使用数字模式）。
        /// </summary>
        public OutputConsole() { } // default

        /// <summary>
        /// 构造函数，使用指定的判定函数与左右字符串创建输出器（将进入字符串模式）。
        /// </summary>
        /// <param name="func">用于判断单元状态的函数（非 null）。</param>
        /// <param name="lhs">判定为 true 时使用的字符串。</param>
        /// <param name="rhs">判定为 false 时使用的字符串。</param>
        public OutputConsole(Func<int, bool> func, string lhs, string rhs)
        {
            this.lhs = lhs;
            this.rhs = rhs;
            this.func = func;
        }
    }
}
