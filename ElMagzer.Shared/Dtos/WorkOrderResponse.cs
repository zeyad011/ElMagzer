using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Shared.Dtos
{
    public class WorkOrderResponse
    {
        public bool Status { get; set; }
        public string ErrorMessage { get; set; }
        public List<WorkOrderHeader> Sha_headerResponse { get; set; }
    }
}
