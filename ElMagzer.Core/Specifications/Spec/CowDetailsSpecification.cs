using ElMagzer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Core.Specifications.Spec
{
    public class CowDetailsSpecification : BaseSpecifications<Cows>
    {
        public CowDetailsSpecification(string cowId)
        : base(c => c.CowsId == cowId)
        {
            Includes.Add(c => c.Batch);
            Includes.Add(c => c.TypeofCows);
            Includes.Add(c => c.Batch.Order);
            Includes.Add(c => c.CowsSeed);
        }
    }
}
