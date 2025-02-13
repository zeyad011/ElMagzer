using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Shared.Dtos
{
    public class BatchDto
    {
        public string BatchNumber { get; set; }
        public int? Count { get; set; }
        public string BatchType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

}
