using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ElMagzer.Core.Models;

namespace ElMagzer.Repository.Configurations
{
    public class CutsConfiguration : IEntityTypeConfiguration<Cutting>
    {
        public void Configure(EntityTypeBuilder<Cutting> builder)
        {
            builder.HasData(
                new Cutting {Id = 1, Code = "1", CutName = "Ribeye", Date = new DateTime(2024, 12, 01) },
                new Cutting {Id = 2, Code = "2", CutName = "Tenderloin", Date = new DateTime(2024, 12, 02) },
                new Cutting {Id = 3, Code = "3", CutName = "Sirloin", Date = new DateTime(2024, 12, 03) },
                new Cutting {Id = 4, Code = "4", CutName = "T-Bone", Date = new DateTime(2024, 12, 04) },
                new Cutting {Id = 5, Code = "5", CutName = "Chuck", Date = new DateTime(2024, 12, 05) },
                new Cutting {Id = 6, Code = "6", CutName = "Flank", Date = new DateTime(2024, 12, 06) },
                new Cutting {Id = 7, Code = "7", CutName = "Brisket", Date = new DateTime(2024, 12, 07) },
                new Cutting {Id = 8, Code = "8", CutName = "Shank", Date = new DateTime(2024, 12, 08) },
                new Cutting {Id = 9, Code = "9", CutName = "Rump", Date = new DateTime(2024, 12, 09) },
                new Cutting {Id = 10,Code = "10", CutName = "Short Loin", Date = new DateTime(2024, 12, 10) },
                new Cutting {Id = 11,Code = "11", CutName = "Top Sirloin", Date = new DateTime(2024, 12, 11) },
                new Cutting {Id = 12,Code = "12", CutName = "Bottom Sirloin", Date = new DateTime(2024, 12, 12) },
                new Cutting {Id = 13,Code = "13", CutName = "Plate", Date = new DateTime(2024, 12, 13) }
            );              
        }                   
    }                       
}
