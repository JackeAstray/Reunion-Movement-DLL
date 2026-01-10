using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Shape
{
    /// <summary>
    /// 绘制器接口
    /// </summary>
    /// <typeparam name="TMatrixVar"> 矩阵变量 </typeparam>
    public interface IDrawer<in TMatrixVar>
    {
        /// <summary>
        /// 绘制矩阵
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        bool Draw(TMatrixVar[,] matrix, out string log);
    }
}
