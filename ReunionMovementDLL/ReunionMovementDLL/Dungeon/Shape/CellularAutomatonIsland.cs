using ReunionMovementDLL.Dungeon.Retouch;

namespace ReunionMovementDLL.Dungeon.Shape
{
    public class CellularAutomatonIsland : IDrawer<int>
    {
        private CellularAutomaton cellularAutomaton = new CellularAutomaton();

        public bool Draw(int[,] matrix)
        {
            return true;
        }

        public bool Draw(int[,] matrix, out string log)
        {
            throw new System.NotImplementedException();
        }
    }
}