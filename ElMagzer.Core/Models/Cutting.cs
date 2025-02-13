using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Core.Models
{
    public class Cutting:BaseModel
    {
        public string Code { get; set; }
        public string CutName { get; set; }
        public DateTime Date { get; set; }= DateTime.Now;
    }
}
