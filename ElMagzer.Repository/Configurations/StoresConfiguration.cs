using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElMagzer.Core.Models;

namespace ElMagzer.Repository.Configurations
{
    public class StoresConfiguration : IEntityTypeConfiguration<Stores>
    {
        public void Configure(EntityTypeBuilder<Stores> builder)
        {
            builder.HasData(
                new Stores { Id = 1,Code = "1",storeName = "مخزن 1", HeightCapacity = 300 },
                new Stores { Id = 2,Code = "2",storeName = "مخزن 2", HeightCapacity = 200 },
                new Stores { Id = 3,Code = "3",storeName = "مخزن 3", HeightCapacity = 400 },
                new Stores { Id = 4,Code = "4",storeName = "مخزن 4", HeightCapacity = 430 },
                new Stores { Id = 5,Code = "5",storeName = "مخزن 5", HeightCapacity = 120 },
                new Stores { Id = 6,Code = "6",storeName = "مخزن 6", HeightCapacity = 140 },
                new Stores { Id = 7,Code = "7",storeName = "مخزن 7", HeightCapacity = 500 },
                new Stores { Id = 8,Code = "8",storeName = "مخزن 8", HeightCapacity = 320 },
                new Stores { Id = 9,Code = "9",storeName = "مخزن 9", HeightCapacity = 250 }
            );
        }
    }
}
