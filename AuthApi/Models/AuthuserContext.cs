using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Models;

public partial class AuthuserContext : DbContext
{
    public AuthuserContext()
    {
    }

    public AuthuserContext(DbContextOptions<AuthuserContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Roles", "AuthUser");

            entity.HasIndex(e => e.Name, "Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(40);
            entity.Property(e => e.RegTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Users", "AuthUser");

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(40);
            entity.Property(e => e.Password).HasColumnType("text");
            entity.Property(e => e.Regtime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Salt).HasColumnType("text");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("userrole_ibfk_2"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("userrole_ibfk_1"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PRIMARY");
                        j.ToTable("UserRole", "AuthUser");
                        j.HasIndex(new[] { "RoleId" }, "RoleId");
                        j.HasIndex(new[] { "UserId", "RoleId" }, "UserId");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
