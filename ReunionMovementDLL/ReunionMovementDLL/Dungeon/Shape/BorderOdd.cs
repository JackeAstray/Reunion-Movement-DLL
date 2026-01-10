using ReunionMovementDLL.Dungeon.Range;
using ReunionMovementDLL.Dungeon.Util;
using MatrixRange = ReunionMovementDLL.Dungeon.Base.Coordinate2DMatrix;

namespace ReunionMovementDLL.Dungeon.Shape
{
    public class BorderOdd : RectBaseWithValue<BorderOdd>, IDrawer<int>
    {
        public bool Draw(int[,] matrix)
        {
            return DrawNormal(matrix);
        }

        public bool Draw(int[,] matrix, out string log)
        {
            throw new System.NotImplementedException();
        }

        public bool DrawNormal(int[,] matrix)
        {
            var endX = this.CalcEndX(MatrixUtil.GetX(matrix));
            var endY = this.CalcEndY(MatrixUtil.GetY(matrix));
            if (endX <= startX || endY <= this.startY) return true;
            for (var col = startX; col < endX; ++col)
            {
                matrix[this.startY, col] = this.drawValue;
                if ((endY - this.startY) % 2 == 0) matrix[endY - 2, col] = this.drawValue;
                matrix[endY - 1, col] = this.drawValue;
            }

            for (var row = this.startY; row < endY; ++row)
            {
                matrix[row, startX] = this.drawValue;
                if ((endX - this.startX) % 2 == 0) matrix[row, endX - 2] = this.drawValue;
                matrix[row, endX - 1] = this.drawValue;
            }

            return true;
        }

        public BorderOdd()
        {
        }

        public BorderOdd(int drawValue, MatrixRange matrixRange) : base(drawValue, matrixRange)
        {
            this.drawValue = drawValue;
        }

        public BorderOdd(int drawValue, uint startX, uint startY, uint width, uint height) : base(drawValue, startX,
            startY, width, height)
        {
            this.drawValue = drawValue;
        }

        public BorderOdd(int drawValue) : base(drawValue)
        {
            this.drawValue = drawValue;
        }
    }
}