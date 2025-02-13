using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Shared.Dtos
{
    public class CowsSeedDto
    {
        public string CowsId { get; set; }
        public double Weight { get; set; }
        public int TypeofCowsId { get; set; }
        public int? BatchId { get; set; }
    }

}
