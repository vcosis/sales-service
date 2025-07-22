using Microsoft.EntityFrameworkCore;
using SalesService.Domain.Entities;

namespace SalesService.Infrastructure.Persistence;

public class SalesDbContext : DbContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }

    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Sale configuration
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("Sales");

            entity.HasKey(s => s.Id);

            entity.Property(s => s.SaleNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(s => s.Date)
                .IsRequired();

            entity.Property(s => s.CustomerId)
                .IsRequired();

            entity.Property(s => s.CustomerName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(s => s.BranchId)
                .IsRequired();

            entity.Property(s => s.BranchName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(s => s.TotalAmount)
                .HasColumnType("decimal(18,2)");

            entity.Property(s => s.Cancelled)
                .IsRequired();

            // One-to-many relationship with SaleItem
            entity.HasMany(s => s.Items)
                .WithOne()
                .HasForeignKey("SaleId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // SaleItem configuration
        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.ToTable("SaleItems");

            entity.HasKey(i => i.Id);

            entity.Property(i => i.ProductId)
                .IsRequired();

            entity.Property(i => i.ProductName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(i => i.Quantity)
                .IsRequired();

            entity.Property(i => i.UnitPrice)
                .HasColumnType("decimal(18,2)");

            entity.Property(i => i.Discount)
                .HasColumnType("decimal(18,2)");

            entity.Property(i => i.Total)
                .HasColumnType("decimal(18,2)");
        });
    }
} 