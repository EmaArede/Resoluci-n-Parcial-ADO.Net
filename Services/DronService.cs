using DronSimulator.Domain.Entities;
using System;
using System.Collections.Generic;

namespace DronSimulator.Services
{
    public class DronService : IDronService
    {
        private readonly (int dx, int dy)[] _movements = new (int, int)[]
        {
            (2, 1), (2, -1), (-2, 1), (-2, -1),
            (1, 2), (1, -2), (-1, 2), (-1, -2)
        };
        
        private int _attempts = 0;
        private DateTime _startTime;
        private bool _showProgress = true;

        public SimulationResult Simulate(int n, int startX, int startY)
        {
            _attempts = 0;
            _startTime = DateTime.Now;
            
            Console.WriteLine($"⏳ Simulando N={n}, inicio=({startX},{startY})...");
            Console.WriteLine("   (el programa puede tardar varios segundos)");
            
            var result = new SimulationResult
            {
                N = n,
                StartX = startX,
                StartY = startY,
                IsSuccess = false
            };

            int[,] board = new int[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    board[i, j] = -1;

            int reachableCount = CountReachable(n, startX, startY);
            result.ReachableCells = reachableCount;

            board[startX, startY] = 0;
            
            bool success = Explore(board, n, startX, startY, 1, reachableCount);
            
            if (success)
            {
                result.IsSuccess = true;
                result.Board = board;
                result.Movements = ExtractMovements(board, n);
                
                var elapsed = DateTime.Now - _startTime;
                Console.WriteLine($"✅ Simulación completada en {elapsed.TotalSeconds:F1}s, {_attempts:N0} intentos");
            }
            else
            {
                Console.WriteLine("❌ No se encontró solución.");
            }

            return result;
        }

        private bool Explore(int[,] board, int n, int currentX, int currentY, int step, int targetSteps)
        {
            _attempts++;
            
            if (_showProgress && _attempts % 100000 == 0)
            {
                var elapsed = DateTime.Now - _startTime;
                Console.WriteLine($"⏳ Intentos: {_attempts:N0}, Paso: {step}/{targetSteps}, Tiempo: {elapsed.TotalSeconds:F1}s");
            }
            
            if (_showProgress && (DateTime.Now - _startTime).TotalSeconds > 30 && _attempts % 10000 == 0)
            {
                Console.WriteLine($"⚠️ La simulación lleva {((DateTime.Now - _startTime).TotalSeconds):F0} segundos...");
                Console.WriteLine($"   Intentos: {_attempts:N0}, puede tardar varios minutos");
            }
            
            // Timeout a los 5 minutos
            if ((DateTime.Now - _startTime).TotalSeconds > 300)
            {
                Console.WriteLine("❌ Tiempo de simulación excedido (5 minutos). Cancelando...");
                return false;
            }
            
            if (step == targetSteps)
                return true;

            var candidates = GetCandidatesOrderedByDegree(board, n, currentX, currentY);

            foreach (var (nextX, nextY) in candidates)
            {
                board[nextX, nextY] = step;
                
                if (Explore(board, n, nextX, nextY, step + 1, targetSteps))
                    return true;
                
                board[nextX, nextY] = -1;
            }

            return false;
        }

        private List<(int x, int y)> GetCandidatesOrderedByDegree(int[,] board, int n, int x, int y)
        {
            var candidates = new List<(int x, int y)>();

            foreach (var (dx, dy) in _movements)
            {
                int newX = x + dx;
                int newY = y + dy;

                if (newX >= 0 && newX < n && newY >= 0 && newY < n && board[newX, newY] == -1)
                {
                    candidates.Add((newX, newY));
                }
            }

            candidates.Sort((a, b) => 
                GetDegree(board, n, a.x, a.y).CompareTo(GetDegree(board, n, b.x, b.y))
            );

            return candidates;
        }

        private int GetDegree(int[,] board, int n, int x, int y)
        {
            int degree = 0;
            foreach (var (dx, dy) in _movements)
            {
                int newX = x + dx;
                int newY = y + dy;
                if (newX >= 0 && newX < n && newY >= 0 && newY < n && board[newX, newY] == -1)
                    degree++;
            }
            return degree;
        }

        private int CountReachable(int n, int startX, int startY)
        {
            bool[,] visited = new bool[n, n];
            var queue = new Queue<(int x, int y)>();
            queue.Enqueue((startX, startY));
            visited[startX, startY] = true;
            int count = 0;

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                count++;

                foreach (var (dx, dy) in _movements)
                {
                    int newX = x + dx;
                    int newY = y + dy;
                    if (newX >= 0 && newX < n && newY >= 0 && newY < n && !visited[newX, newY])
                    {
                        visited[newX, newY] = true;
                        queue.Enqueue((newX, newY));
                    }
                }
            }

            return count;
        }

        private List<Movement> ExtractMovements(int[,] board, int n)
        {
            var movements = new List<Movement>();
            int totalCells = n * n;

            for (int step = 0; step < totalCells; step++)
            {
                bool found = false;
                for (int i = 0; i < n && !found; i++)
                {
                    for (int j = 0; j < n && !found; j++)
                    {
                        if (board[i, j] == step)
                        {
                            movements.Add(new Movement
                            {
                                Step = step,
                                X = i,
                                Y = j
                            });
                            found = true;
                        }
                    }
                }
                if (!found) break;
            }

            return movements;
        }

        public void PrintMatrix(int[,] board, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (board[i, j] == -1)
                        Console.Write("  . ");
                    else
                        Console.Write($" {board[i, j],2} ");
                }
                Console.WriteLine();
            }
        }
    }
}