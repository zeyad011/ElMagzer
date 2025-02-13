using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Shared.Dtos
{
    public class TransferDto  //int sourceStoreId, int destinationStoreId, List<string> piecesList
    {
        public int sourceStoreId { get; set; }
        public int destinationStoreId { get; set; }
        public List<string> piecesList { get; set; }
    }
}
