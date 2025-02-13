using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Shared.Dtos
{
    public class StorePieceDto
    {
        public string PieceNumber { get; set; }
        public double Weight { get; set; }
        public string PieceType { get; set; }
        public string TechDevice { get; set; }
        public string BatchNumber { get; set; }
        public string OrderNumber { get; set; }
    }

}
