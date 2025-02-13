using ElMagzer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Core.Specifications.Spec
{
    public class CowsByBatchSpecification : BaseSpecifications<Cows>
    {
        public CowsByBatchSpecification(string batchCode)
            : base(cs => cs.Batch.BatchCode == batchCode) 
        {
            Includes.Add(cs => cs.Batch); 
        }
    }

}
