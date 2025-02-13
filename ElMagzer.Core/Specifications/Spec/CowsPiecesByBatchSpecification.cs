using ElMagzer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Core.Specifications.Spec
{
    public class CowsPiecesSpecification : BaseSpecifications<CowsPieces>
    {
        public CowsPiecesSpecification(string batchCode)
            : base(p => p.Cow.Id == p.CowId && p.Cow.Batch.BatchCode == batchCode)
        {
            Includes.Add(p => p.Cow);
            Includes.Add(p => p.Cow.Batch);
        }
    }

}
