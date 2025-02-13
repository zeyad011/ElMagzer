using ElMagzer.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElMagzer.Repository.Configurations
{
    public class ClientConfigrations : IEntityTypeConfiguration<Clients>
    {
        public void Configure(EntityTypeBuilder<Clients> builder)
        {
            builder.HasData(
                new Clients {Id = 1, Name = "Ahmed", Code = "1" },
                new Clients {Id = 2, Name = "Ziad", Code = "2" },
                new Clients {Id = 3, Name = "Rizk", Code = "3" }
            );               
        }                    
    }                        
}                            
                             