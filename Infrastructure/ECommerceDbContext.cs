using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class EcommerceDbContext : DbContext
{
    public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : base(options) { }

    public DbSet<ItemDomain> Items => Set<ItemDomain>();
    public DbSet<OrderDomain> Orders => Set<OrderDomain>();
    public DbSet<TransactionDomain> Transactions => Set<TransactionDomain>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ItemDomain>().HasKey(i => i.Id);

        modelBuilder.Entity<OrderDomain>()
            .HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<OrderItemDomain>()
            .HasKey(oi => new { oi.OrderId, oi.ItemId });

        modelBuilder.Entity<OrderItemDomain>()
            .HasOne(oi => oi.Item)
            .WithMany();
    }
}
