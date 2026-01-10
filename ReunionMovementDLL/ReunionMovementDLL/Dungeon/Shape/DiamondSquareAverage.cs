using ReunionMovementDLL.Dungeon.Random;
using System;

namespace ReunionMovementDLL.Dungeon.Shape
{
    public static class DiamondSquareAverage
    {
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
}