using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Util;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Shape
{
    public class AscendingOrder : RectBaseWithValue<AscendingOrder>, IDrawer<int>
    {
        public bool Draw(int[,] matrix)
        {
            return DrawNormal(matrix);
        }

        public bool Draw(int[,] matrix, out string log)
        {
            throw new System.NotImplementedException();
        }

        public int[,] Create(int[,] matrix)
        {
            this.Draw(matrix);
            return matrix;
        }

        private bool DrawNormal(int[,] matrix)
        {
            var value = this.drawValue;
            var endX = this.CalcEndX(MatrixUtil.GetX(matrix));
            var endY = this.CalcEndY(MatrixUtil.GetY(matrix));
            for (var row = startY; row < endY; ++row)
                for (var col = startX; col < endX; ++col, value++)
                    matrix[row, col] = value;

            return true;
        }

        public AscendingOrder() { }

        public AscendingOrder(int drawValue, MatrixRange matrixRange) : base(drawValue, matrixRange)
        {
            this.drawValue = drawValue;
        }

        public AscendingOrder(int drawValue, uint startX, uint startY, uint width, uint height) : base(drawValue, startX, startY, width,
            height)
        {
            this.drawValue = drawValue;
        }

        public AscendingOrder(int drawValue) : base(drawValue)
        {
            this.drawValue = drawValue;
        }
    }
}