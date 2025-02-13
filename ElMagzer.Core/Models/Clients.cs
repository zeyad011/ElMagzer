using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Core.Models
{
    public class Clients:BaseModel
    {
        public string Name { get; set; }
        public string? NameENG { get; set; }
        public string Code { get; set; }

        public ICollection<Orders> Orders { get; set; } = new HashSet<Orders>();
    }
}
