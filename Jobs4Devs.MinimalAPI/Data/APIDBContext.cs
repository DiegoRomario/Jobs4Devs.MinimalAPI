using Jobs4Devs.MinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Jobs4Devs.MinimalAPI.Data
{
    public class APIDBContext : DbContext
    {
        public APIDBContext(DbContextOptions<APIDBContext> options) : base(options) { }

        public DbSet<Vacancy> Vacancies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vacancy>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Vacancy>()
                .Property(p => p.Title)
                .IsRequired()
                .HasColumnType("varchar(240)");

            modelBuilder.Entity<Vacancy>()
                .Property(p => p.Description)
                .IsRequired()
                .HasColumnType("varchar(1024)");

            modelBuilder.Entity<Vacancy>()
                .Property(p => p.Company)
                .IsRequired()
                .HasColumnType("varchar(120)");

            modelBuilder.Entity<Vacancy>()
                .Property(p => p.IsOpen)
                .IsRequired()
                .HasDefaultValue(true);

            modelBuilder.Entity<Vacancy>()
                .Property(p => p.MinSalary)
                .IsRequired();

            modelBuilder.Entity<Vacancy>()
                .Property(p => p.MaxSalary)
                .IsRequired();

            modelBuilder.Entity<Vacancy>()
                .Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);

            modelBuilder.Entity<Vacancy>()
                .ToTable("Vacancies");

            base.OnModelCreating(modelBuilder);
        }
    }
}