using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Shared.Dtos
{
    public class AssignBatchesDto
    {
        public string OrderCode { get; set; }
        public List<BatchUpdateDto> Batches { get; set; }
    }
}
