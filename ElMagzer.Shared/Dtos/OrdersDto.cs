using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Shared.Dtos
{
    public class OrdersDto
    {
        public string Ordersnumber { get; set; }
        public List<string> pieceNumbers { get; set; }
    }
}
