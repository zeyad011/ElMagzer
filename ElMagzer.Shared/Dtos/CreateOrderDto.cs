using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Shared.Dtos
{
    public class CreateOrderDto
    {
        public string Code { get; set; } 
        public string OrderType { get; set; } 
        public int NoOfCows { get; set; } 
        public int NoOfBatches { get; set; }
        public string? ClientName { get; set; }
        public DateTime date { get; set; }
    }
}
