using ElMagzer.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ElMagzer.Repository.Configurations
{
    public class MiscarriageTypesConfiguration : IEntityTypeConfiguration<MiscarriageType>
    {
        public void Configure(EntityTypeBuilder<MiscarriageType> builder)
        {
            builder.HasData(
                new MiscarriageType { Id = 1,Name = "ممبار", Code = "1" },
                new MiscarriageType { Id = 2,Name = "فشه", Code = "2" },
                new MiscarriageType { Id = 3,Name = "كرشه", Code = "3" },
                new MiscarriageType { Id = 4,Name = "طحال", Code = "4" },
                new MiscarriageType { Id = 5,Name = "بنكرياس", Code = "5" },
                new MiscarriageType { Id = 6,Name = "كبده", Code = "6" },
                new MiscarriageType { Id = 7,Name = "مخ", Code = "7" },
                new MiscarriageType { Id = 8,Name = "قلب", Code = "8" }
            );
        }
    }
}
