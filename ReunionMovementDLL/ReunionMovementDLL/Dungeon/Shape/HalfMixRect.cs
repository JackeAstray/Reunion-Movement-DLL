using System.Collections.Generic;
using ReunionMovementDLL.Dungeon.Random;
using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Util;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;


namespace ReunionMovementDLL.Dungeon.Shape
{

    public class HalfMixRect : RectBaseWithIList<HalfMixRect>, IDrawer<int>
    {
        RandomBase rand = new RandomBase();

        public bool Draw(int[,] matrix)
        {
            return this.DrawNormal(matrix);
        }

        public bool Draw(int[,] matrix, out string log)
        {
            throw new System.NotImplementedException();
        }

        private bool DrawNormal(int[,] matrix)
        {
            uint drawValueCount = (uint)this.drawValue.Count;
            var endX = this.CalcEndX(MatrixUtil.GetX(matrix));
            var endY = this.CalcEndY(MatrixUtil.GetY(matrix));
            for (var row = this.startY; row < endY; ++row)
                for (var col = this.startX; col < endX; ++col)
                    matrix[row, col]
                        = this.rand.Probability(0.5) ?
                        this.drawValue[0] : this.drawValue[(int)rand.Next(drawValueCount)];
            return true;
        }

        /* Constructors */

        public HalfMixRect() { } // default

        public HalfMixRect(IList<int> drawValue) : base(drawValue)
        {
        }

        public HalfMixRect(MatrixRange matrixRange, IList<int> drawValue) : base(matrixRange, drawValue)
        {
        }
    }
}