using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Shared.Dtos
{
    public class BatchToPiecesUpdateDto
    {
        public string BatchCode { get; set; }
        public string cowType { get; set; }
        public int Quntity { get; set; }
        public List<PieceDto> Pieces { get; set; }
    }

}
