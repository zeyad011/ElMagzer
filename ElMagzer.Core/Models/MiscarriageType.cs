using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Core.Models
{
    public class MiscarriageType:BaseModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public ICollection<CowMiscarriage> CowMiscarriages { get; set; } = new HashSet<CowMiscarriage>();
    }
}
