using ElMagzer.Core.Models;
using Microsoft.EntityFrameworkCore;
namespace ElMagzer.Repository.Data
{
    public class ElMagzerContext(DbContextOptions<ElMagzerContext> options) : DbContext(options)
    {
        public DbSet<Cows> Cows { get; set; }
        //public DbSet<CowsNumber> CowsNumber { get; set; }
        public DbSet<CowsPieces> CowsPieces { get; set; }
        public DbSet<Cow_Pieces_2> Cow_Pieces_2 { get; set; }
        //public DbSet<Executions> Executions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Stores> Stores { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<TypeofCows> TypeofCows { get; set;}
        public DbSet<CowsSeed> cowsSeeds { get; set; }
        public DbSet<Cutting> cuttings { get; set; }
        public DbSet<Clients> clients { get; set; }
        public DbSet<MiscarriageType> miscarriageTypes { get; set; }
        public DbSet<CowMiscarriage> cowMiscarriages { get; set; }
        public DbSet<Suppliers> suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ElMagzerContext).Assembly);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CowMiscarriage>()
                .HasKey(cm => new { cm.CowsId, cm.MiscarriageTypeId });

            modelBuilder.Entity<CowMiscarriage>()
                .HasOne(cm => cm.Cow)
                .WithMany(c => c.CowMiscarriages)
                .HasForeignKey(cm => cm.CowsId);

            modelBuilder.Entity<CowMiscarriage>()
                .HasOne(cm => cm.MiscarriageType)
                .WithMany(mt => mt.CowMiscarriages)
                .HasForeignKey(cm => cm.MiscarriageTypeId);
        }
    }
}
