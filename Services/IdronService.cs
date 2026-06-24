using DronSimulator.Domain.Entities;

namespace DronSimulator.Services
{
    public interface IDronService
    {
        SimulationResult Simulate(int n, int startX, int startY);
        void PrintMatrix(int[,] board, int n);
    }
}