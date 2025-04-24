using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure MarksPercentage precision and scale
            modelBuilder.Entity<Qualification>()
                .Property(q => q.MarksPercentage)
                .HasColumnType("decimal(5,2)");

            // Explicitly configure Qualification Id as identity
            modelBuilder.Entity<Qualification>()
                .Property(q => q.Id)
                .ValueGeneratedOnAdd();

            // Configure ASP.NET Identity table column types
            modelBuilder.Entity<IdentityUser>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("nvarchar(450)");
            });

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("nvarchar(450)");
                entity.Property(e => e.Name).HasColumnType("nvarchar(256)");
                entity.Property(e => e.NormalizedName).HasColumnType("nvarchar(256)");
                entity.Property(e => e.ConcurrencyStamp).HasColumnType("nvarchar(max)");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnType("nvarchar(450)");
                entity.Property(e => e.RoleId).HasColumnType("nvarchar(450)");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int");
                entity.Property(e => e.UserId).HasColumnType("nvarchar(450)");
                entity.Property(e => e.ClaimType).HasColumnType("nvarchar(max)");
                entity.Property(e => e.ClaimValue).HasColumnType("nvarchar(max)");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(e => e.LoginProvider).HasColumnType("nvarchar(450)");
                entity.Property(e => e.ProviderKey).HasColumnType("nvarchar(450)");
                entity.Property(e => e.UserId).HasColumnType("nvarchar(450)");
                entity.Property(e => e.ProviderDisplayName).HasColumnType("nvarchar(max)");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnType("nvarchar(450)");
                entity.Property(e => e.LoginProvider).HasColumnType("nvarchar(450)");
                entity.Property(e => e.Name).HasColumnType("nvarchar(450)");
                entity.Property(e => e.Value).HasColumnType("nvarchar(max)");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int");
                entity.Property(e => e.RoleId).HasColumnType("nvarchar(450)");
                entity.Property(e => e.ClaimType).HasColumnType("nvarchar(max)");
                entity.Property(e => e.ClaimValue).HasColumnType("nvarchar(max)");
            });
        }
    }
}