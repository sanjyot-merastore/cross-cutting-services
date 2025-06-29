using MeraStore.Services.Cross.Cutting.Domain.Entities;
using MeraStore.Shared.Kernel.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace MeraStore.Services.Cross.Cutting.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContextBase(options)
{
    public DbSet<RequestLog> Requests { get; set; } = null!;
    public DbSet<ResponseLog> Responses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RequestLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(26); // ULIDs are 26 chars long
            entity.Property(e => e.HttpMethod).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Url).IsRequired();
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.Payload).HasColumnType("VARBINARY(MAX)");
            entity.Property(e => e.Timestamp).HasDefaultValueSql("GETUTCDATE()"); // ✅ Fix for SQL Server

            // Indexes
            entity.HasIndex(e => e.CorrelationId);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => new { e.HttpMethod, e.Timestamp }); // Optimized for method & time-based queries
        });

        modelBuilder.Entity<ResponseLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(26);
            entity.Property(e => e.RequestId).IsRequired();
            entity.Property(e => e.StatusCode).IsRequired();
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.Payload).HasColumnType("VARBINARY(MAX)");
            entity.Property(e => e.Timestamp).HasDefaultValueSql("GETUTCDATE()"); // ✅ Fix for SQL Server

            // Indexes
            entity.HasIndex(e => e.CorrelationId);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.StatusCode);
            entity.HasIndex(e => new { e.RequestId, e.Timestamp }); // Fast lookup by request & time
        });
    }
}