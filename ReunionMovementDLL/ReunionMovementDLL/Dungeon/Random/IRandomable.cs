using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Random
{
    /// <summary>
    /// 随机数生成接口
    /// </summary>
    public interface IRandomable
    {
        /* [min, max) */
        uint Next(uint min, uint max);
        /* [0, max) */
        uint Next(uint max);
        /* [0, int::max) */
        uint Next();
    }
}
