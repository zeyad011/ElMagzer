using ElMagzer.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Repository.Configurations
{
    public class SuppliersConfigrations : IEntityTypeConfiguration<Suppliers>
    {
        public void Configure(EntityTypeBuilder<Suppliers> builder)
        {
            builder.HasData(
                new Suppliers { Id = 1, SupplierName = "ElRawda", Code = "1" },
                new Suppliers { Id = 2, SupplierName = "ElMagzer", Code = "2" },
                new Suppliers { Id = 3, SupplierName = "Shatat", Code = "3" }
            );
        }
    }
}
