using System;

namespace DronSimulator.Data.Models
{
    public class MasterControl
    {
        public int Id { get; set; }
        public DateTime FechaEjecucion { get; set; }
        public int TamanioTerreno { get; set; }
        public int CoordenadaX { get; set; }
        public int CoordenadaY { get; set; }
    }
}