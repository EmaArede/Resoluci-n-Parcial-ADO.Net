using System.Collections.Generic;

namespace DronSimulator.Domain.Entities
{
    public class SimulationResult
    {
        public int N { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }
        public bool IsSuccess { get; set; }
        public int ReachableCells { get; set; }
        public int[,] Board { get; set; }
        public List<Movement> Movements { get; set; } = new List<Movement>();
    }
}