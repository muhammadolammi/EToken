using EToken.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EToken.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<CustomerDevice> CustomerDevices => Set<CustomerDevice>();
    public DbSet<TokenSecret> TokenSecrets => Set<TokenSecret>();
    public DbSet<VerificationLog> VerificationLogs => Set<VerificationLog>();
    
    public DbSet<VerificationAttempt> VerificationAttempts => Set<VerificationAttempt>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

        public DbSet<Account> Accounts => Set<Account>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Essential for Identity schema mapping
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CustomerDevice>(entity =>
        {
            entity.ToTable("customer_devices");
            entity.HasKey(e => e.DeviceId);
            entity.Property(e => e.DeviceId).HasColumnName("device_id").HasDefaultValueSql("NEWID()");
            entity.Property(e => e.Cif).HasColumnName("cif").IsRequired();
            entity.Property(e => e.DeviceModel).HasColumnName("device_model").HasMaxLength(100);
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("active").IsRequired();
            entity.Property(e => e.RegisteredAt).HasColumnName("registered_at").HasDefaultValueSql("SYSDATETIMEOFFSET()");
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");

            entity.HasIndex(e => e.Cif, "ix_customer_devices_cif");
        });

       modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
           entity.Ignore(u => u.Cif); // Prevents EF Core from creating a 2nd column
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasColumnName("cif");

            entity.Property(u => u.UserName).HasColumnName("username");
            entity.Property(u => u.NormalizedUserName).HasColumnName("normalized_username");
            entity.Property(u => u.Email).HasColumnName("email");
            entity.Property(u => u.NormalizedEmail).HasColumnName("normalized_email");
            entity.Property(u => u.EmailConfirmed).HasColumnName("email_confirmed");
            entity.Property(u => u.PasswordHash).HasColumnName("password_hash");
            entity.Property(u => u.SecurityStamp).HasColumnName("security_stamp");
            entity.Property(u => u.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            entity.Property(u => u.PhoneNumber).HasColumnName("phone_number");
            entity.Property(u => u.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
            entity.Property(u => u.TwoFactorEnabled).HasColumnName("two_factor_enabled");
            entity.Property(u => u.LockoutEnd).HasColumnName("lockout_end");
            entity.Property(u => u.LockoutEnabled).HasColumnName("lockout_enabled");
            entity.Property(u => u.AccessFailedCount).HasColumnName("access_failed_count");
            
            entity.Property(u => u.LastName).HasColumnName("last_name");
            entity.Property(u => u.FirstName).HasColumnName("first_name");

            entity.HasIndex(e => e.UserName, "ix_users_username");
            entity.HasIndex(e => e.Email, "ix_users_email");

            // Custom extra property
        });

        modelBuilder.Entity<IdentityRole<Guid>>(entity =>
        {
            entity.ToTable("roles");
            entity.Property(r => r.Id).HasColumnName("id");
            entity.Property(r => r.Name).HasColumnName("name");
            entity.Property(r => r.NormalizedName).HasColumnName("normalized_name");
            entity.Property(r => r.ConcurrencyStamp).HasColumnName("concurrency_stamp");

                        entity.HasIndex(e => e.NormalizedName, "ix_roles_normalized_name");

        });

        // Map UserRoles join table
        modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
        {
            entity.ToTable("user_roles");
            entity.Property(ur => ur.UserId).HasColumnName("user_id");
            entity.Property(ur => ur.RoleId).HasColumnName("role_id");
                                    entity.HasIndex(e => e.RoleId, "ix_user_roles_role_id");

        });



        modelBuilder.Entity<TokenSecret>(entity =>
    {
        entity.ToTable("token_secrets");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("NEWID()");
        entity.Property(e => e.Cif).HasColumnName("cif").IsRequired();
        entity.Property(e => e.DeviceId).HasColumnName("device_id").IsRequired();
        entity.Property(e => e.EncryptedSecret).HasColumnName("encrypted_secret").IsRequired();
        entity.Property(e => e.LastAcceptedBucket).HasColumnName("last_accepted_bucket").HasDefaultValue(0);
        entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("active");
        entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("SYSDATETIMEOFFSET()");
        
        entity.HasIndex(e => e.Cif, "ix_token_secrets_cif");
    });

    modelBuilder.Entity<VerificationLog>(entity =>
    {
        entity.ToTable("verification_log");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        entity.Property(e => e.Cif).HasColumnName("cif").IsRequired();
        entity.Property(e => e.DeviceId).HasColumnName("device_id").IsRequired();
        entity.Property(e => e.ActionType).HasColumnName("action_type").HasMaxLength(20).IsRequired();
        entity.Property(e => e.Result).HasColumnName("result").HasMaxLength(20).IsRequired();
        entity.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(45);
        entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("SYSDATETIMEOFFSET()");

        entity.HasIndex(e => new { e.Cif, e.CreatedAt }, "ix_verification_log_cif_created");
    });

    modelBuilder.Entity<VerificationAttempt>(entity =>
    {
        entity.ToTable("verification_attempts");
        entity.HasKey(e => e.Cif);
        entity.Property(e => e.Cif).HasColumnName("cif");
        entity.Property(e => e.FailedCount).HasColumnName("failed_count").HasDefaultValue(0);
        entity.Property(e => e.LockedUntil).HasColumnName("locked_until");
    });


      modelBuilder.Entity<Account>(entity =>
    {
        entity.ToTable("accounts");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("NEWID()");
        entity.Property(e => e.Cif).HasColumnName("cif").IsRequired();
        entity.Property(e => e.Number).HasColumnName("number").IsRequired().HasMaxLength(10);
        entity.Property(e => e.Balance).HasColumnName("balance").IsRequired();
        entity.Property(e => e.Type).HasColumnName("type").HasDefaultValue(0);
        entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("active");
        entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("SYSDATETIMEOFFSET()");
        
        entity.HasIndex(e => e.Cif, "ix_accounts_cif");
    });

     modelBuilder.Entity<Transaction>(entity =>
    {
        entity.ToTable("transactions");
        entity.HasKey(e => e.Id); 
        entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("NEWID()");
        entity.Property(e => e.SourceAccountId).HasColumnName("source_account_id").IsRequired();
        entity.Property(e => e.DestinationAccountId).HasColumnName("destination_account_id").IsRequired();

        entity.Property(e => e.Amount).HasColumnName("amount").IsRequired();
        entity.Property(e => e.SourceAccountId).HasColumnName("narration").IsRequired();
        entity.Property(e => e.SourceAccountId).HasColumnName("reference").IsRequired();
        entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("successful");
        entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("SYSDATETIMEOFFSET()");
        
        entity.HasIndex(e => e.SourceAccountId, "ix_transaction_source_account");
    });
    }
}