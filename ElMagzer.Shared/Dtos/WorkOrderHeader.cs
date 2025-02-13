using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Shared.Dtos
{
    public class WorkOrderHeader
    {
        public string WorkOrder { get; set; }
        public string TransDate { get; set; }
        public string CustmerAccount { get; set; }
        public string CustmerName { get; set; }
        public List<WorkOrderLine> linesResponse { get; set; }
    }
}
