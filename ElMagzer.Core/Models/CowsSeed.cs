using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Core.Models
{
    public class CowsSeed:BaseModel
    {
        public string CowsId { get; set; }

        public double weight { get; set; }
        public bool IsPrinted { get; set; } = false;
        public int TypeofCowsId { get; set; }
        public TypeofCows TypeofCows { get; set; }
        public int clientId { get; set; }
        public Clients client { get; set; }
        public int suppliersId { get; set; }
        public Suppliers suppliers { get; set; }
        public int? BatchId { get; set; }
        public Batch Batch { get; set; }
    }
}
