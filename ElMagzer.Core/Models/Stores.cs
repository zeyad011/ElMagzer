using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Core.Models
{
    public class Stores:BaseModel
    {
        public string Code { get; set; }
        public string storeName {  get; set; }
        public int? quantity { get; set; }
        public int HeightCapacity { get; set; }
        public ICollection<CowsPieces> CowsPieces { get; set; } = new HashSet<CowsPieces>();
        public ICollection<Cow_Pieces_2> CowPieces2 { get; set; } = new HashSet<Cow_Pieces_2>();
    }
}
