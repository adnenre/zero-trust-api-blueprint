using Microsoft.EntityFrameworkCore;
using ZeroTrustAPI.Api.Entities;

namespace ZeroTrustAPI.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<UploadedFile> UploadedFiles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // UserRole junction
        modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany()
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany()
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // RefreshToken
        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ApiKey
        modelBuilder.Entity<ApiKey>()
            .HasOne(ak => ak.User)
            .WithMany()
            .HasForeignKey(ak => ak.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Session
        modelBuilder.Entity<Session>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Device
        modelBuilder.Entity<Device>()
            .HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // AuditLog – FIXED: ensure foreign key is nullable when using SetNull
        modelBuilder.Entity<AuditLog>()
            .HasOne(al => al.User)
            .WithMany()
            .HasForeignKey(al => al.UserId)
            .IsRequired(false)               // <-- Allows NULL when User is deleted
            .OnDelete(DeleteBehavior.SetNull);

        // UploadedFile
        modelBuilder.Entity<UploadedFile>()
            .HasOne(uf => uf.UploadedByUser)
            .WithMany()
            .HasForeignKey(uf => uf.UploadedByUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Table names
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Role>().ToTable("Roles");
        modelBuilder.Entity<UserRole>().ToTable("UserRoles");
        modelBuilder.Entity<RefreshToken>().ToTable("RefreshTokens");
        modelBuilder.Entity<ApiKey>().ToTable("ApiKeys");
        modelBuilder.Entity<Session>().ToTable("Sessions");
        modelBuilder.Entity<Device>().ToTable("Devices");
        modelBuilder.Entity<AuditLog>().ToTable("AuditLogs");
        modelBuilder.Entity<UploadedFile>().ToTable("UploadedFiles");
    }
}