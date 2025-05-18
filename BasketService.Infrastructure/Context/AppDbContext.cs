using Microsoft.EntityFrameworkCore;

namespace BasketService.Domain.Entities;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
    
    public DbSet<BasketReservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<BasketReservation>().Property(r => r.Id).HasDefaultValueSql("gen_random_uuid()");
    }
    
}