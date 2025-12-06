using InBev.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InBev.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<PhoneNumber> PhoneNumbers => Set<PhoneNumber>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Employee
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.HasIndex(e => e.Email)
                .IsUnique();
            
            entity.Property(e => e.DocNumber)
                .IsRequired()
                .HasMaxLength(14);
            
            entity.HasIndex(e => e.DocNumber)
                .IsUnique();
            
            entity.Property(e => e.PasswordHash)
                .IsRequired();
            
            entity.Property(e => e.BirthDate)
                .IsRequired();
            
            entity.Property(e => e.Role)
                .IsRequired();
            
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Relacionamento auto-referencial (Manager -> Subordinates)
            entity.HasOne(e => e.Manager)
                .WithMany(e => e.Subordinates)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com PhoneNumbers
            entity.HasMany(e => e.PhoneNumbers)
                .WithOne(p => p.Employee)
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da entidade PhoneNumber
        modelBuilder.Entity<PhoneNumber>(entity =>
        {
            entity.ToTable("PhoneNumbers");
            
            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.Number)
                .IsRequired()
                .HasMaxLength(20);
            
            entity.Property(p => p.Type)
                .IsRequired();
        });
    }
}

