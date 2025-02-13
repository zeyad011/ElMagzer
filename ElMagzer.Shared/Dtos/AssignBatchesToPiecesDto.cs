using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Shared.Dtos
{
    public class AssignBatchesToPiecesDto
    {
        public string OrderCode { get; set; }
        public List<BatchToPiecesUpdateDto> Batches { get; set; }
    }

}
