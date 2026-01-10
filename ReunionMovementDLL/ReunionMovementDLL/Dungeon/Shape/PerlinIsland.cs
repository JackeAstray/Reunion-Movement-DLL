using ReunionMovementDLL.Dungeon.Random;
using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Util;
using System;
using System.Linq;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Shape
{
    public sealed class PerlinIsland : RectBasePerlin<PerlinIsland>, IDrawer<int>, ITerrainDrawer
    {
        private XorShift128 rand = new XorShift128();

        public bool Draw(int[,] matrix)
        {
            return DrawNormal(matrix);
        }

        public bool Draw(int[,] matrix, out string log)
        {
            throw new NotImplementedException();
        }

        public bool DrawNormalize(float[,] matrix)
        {
            int[,] convertedMatrix = new int[matrix.GetLength(0), matrix.GetLength(1)];

            // I cannot use LINQ for 2 dim array. Please tell me how to use LINQ for 2 dim array...orz
            for (int y = 0; y < MatrixUtil.GetY(matrix); ++y)
            {
                for (int x = 0; x < MatrixUtil.GetX(matrix); ++x)
                {
                    convertedMatrix[y, x] = (int)matrix[y, x];
                }
            }
            DrawNormal(convertedMatrix);
            Normalize(convertedMatrix, matrix);
            return true;
        }

        private bool DrawNormal(int[,] matrix)
        {
            uint endX = CalcEndX(MatrixUtil.GetX(matrix));
            uint endY = CalcEndY(MatrixUtil.GetY(matrix));

            PerlinNoise perlin = new PerlinNoise((int)rand.Next());

            double frequencyX = (endX - startX) / frequency;
            double frequencyY = (endY - startY) / frequency;

            //            Debug.Log(frequencyX + " " + maxHeight);

            for (uint row = startY; row < endY; ++row)
            {
                for (uint col = startX; col < endX; ++col)
                {
                    matrix[row, col] = minHeight + minHeight + (int)((double)(maxHeight - minHeight) *
                                       perlin.OctaveNoise(octaves, (col / frequencyX),
                                           (row / frequencyY)));
                }
            }

            return true;
        }

        private void Normalize(int[,] matrix, float[,] retMatrix)
        {
            // use maxHeight from derived class.
            for (int y = 0; y < MatrixUtil.GetY(matrix); ++y)
            {
                for (int x = 0; x < MatrixUtil.GetX(matrix); ++x)
                {
                    retMatrix[y, x] = (float)matrix[y, x] / maxHeight;
                }
            }
        }

        public PerlinIsland()
        {
        } // = default()

        public PerlinIsland(uint startX, uint startY, uint width, uint height) : base(startX, startY, width, height)
        {
        }

        public PerlinIsland(double frequency)
        {
            this.frequency = frequency;
        }

        public PerlinIsland(double frequency, uint octaves) : base(frequency, octaves)
        {
        }

        public PerlinIsland(double frequency, uint octaves, int maxHeight)
        {
            this.frequency = frequency;
            this.octaves = octaves;
            this.maxHeight = maxHeight;
        }

        public PerlinIsland(double frequency, uint octaves, int maxHeight, int minHeight) : base(frequency, octaves,
            maxHeight, minHeight)
        {
        }

        public PerlinIsland(MatrixRange matrixRange, double frequency) : base(matrixRange, frequency)
        {
        }

        public PerlinIsland(MatrixRange matrixRange, double frequency, uint octaves) : base(matrixRange, frequency,
            octaves)
        {
        }

        public PerlinIsland(MatrixRange matrixRange, double frequency, uint octaves, int maxHeight) : base(
            matrixRange, frequency, octaves, maxHeight)
        {
        }

        public PerlinIsland(MatrixRange matrixRange, double frequency, uint octaves, int maxHeight, int minHeight) :
            base(matrixRange, frequency, octaves, maxHeight, minHeight)
        {
        }

        public PerlinIsland(uint startX, uint startY, uint width, uint height, double frequency) : base(startX, startY,
            width, height, frequency)
        {
        }

        public PerlinIsland(uint startX, uint startY, uint width, uint height, double frequency, uint octaves) : base(
            startX, startY, width, height, frequency, octaves)
        {
        }

        public PerlinIsland(uint startX, uint startY, uint width, uint height, double frequency, uint octaves,
            int maxHeight) : base(startX, startY, width, height, frequency, octaves, maxHeight)
        {
        }

        public PerlinIsland(uint startX, uint startY, uint width, uint height, double frequency, uint octaves,
            int maxHeight, int minHeight) : base(startX, startY, width, height, frequency, octaves, maxHeight,
            minHeight)
        {
        }
    }
}