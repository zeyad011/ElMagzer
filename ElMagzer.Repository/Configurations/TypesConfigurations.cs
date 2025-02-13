using ElMagzer.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElMagzer.Repository.Configurations
{
    public class TypesConfigurations : IEntityTypeConfiguration<TypeofCows>
    {
        public void Configure(EntityTypeBuilder<TypeofCows> builder)
        {
            builder.HasData(
                new TypeofCows { Id = 1, TypeName = "بلدي" },
                new TypeofCows { Id = 2, TypeName = "برازيلي" },
                new TypeofCows { Id = 3, TypeName = "هولندي" },
                new TypeofCows { Id = 4, TypeName = "انجليزي" },
                new TypeofCows { Id = 5, TypeName = "سويسري" },
                new TypeofCows { Id = 6, TypeName = "هندي" }
            );
        }
    }
}
