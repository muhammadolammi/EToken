using EToken.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace EToken.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerDevice>(entity =>
        {
            entity.ToTable("customer_devices");
            entity.HasKey(e => e.DeviceId);

            entity.Property(e => e.DeviceId).HasColumnName("device_id").HasDefaultValueSql("NEWID()");
            entity.Property(e => e.Cif).HasColumnName("cif").HasMaxLength(20).IsRequired();
            entity.Property(e => e.DeviceModel).HasColumnName("device_model").HasMaxLength(100);
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("active").IsRequired();
            entity.Property(e => e.RegisteredAt).HasColumnName("registered_at").HasDefaultValueSql("SYSDATETIMEOFFSET()");
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");

            entity.HasIndex(e => e.Cif, "ix_customer_devices_cif");
        });
    }


        public DbSet<CustomerDevice> CustomerDevices => Set<CustomerDevice>();

}