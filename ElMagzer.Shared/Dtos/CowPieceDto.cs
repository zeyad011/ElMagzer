using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Shared.Dtos
{
    public class CowPieceDto
    {
        public string PieceId { get; set; }
        public double PieceWeight_In { get; set; }
        public double? PieceWeight_Out { get; set; }
        public string? PieceType { get; set; }
        public string? Status { get; set; }
    }
}
