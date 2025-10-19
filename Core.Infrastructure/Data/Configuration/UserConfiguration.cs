// MeshNetwork.Infrastructure/Data/Configurations/UserConfiguration.cs
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Data.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Email)
            .HasMaxLength(255)
            .IsRequired();
        
        builder.Property(u => u.Username)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(u => u.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();
        
        builder.Property(u => u.IsRelayEnabled)
            .HasDefaultValue(false);
        
        builder.Property(u => u.RelayCredits)
            .HasPrecision(18, 2)
            .HasDefaultValue(0m);
        
        builder.Property(u => u.TotalBytesRelayed)
            .HasDefaultValue(0L);
        
        builder.Property(u => u.FirstName)
            .HasMaxLength(100);
        
        builder.Property(u => u.LastName)
            .HasMaxLength(100);
        
        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);
        
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);
        
        builder.Property(u => u.EmailConfirmed)
            .HasDefaultValue(false);
        
        builder.Property(u => u.CreatedAt)
            .IsRequired();
        
        // Relationships
        builder.HasMany(u => u.Nodes)
            .WithOne(n => n.Owner)
            .HasForeignKey(n => n.OwnerId);
        
        builder.HasMany(u => u.RelayTransactions)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId);
        
        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique();
        
        builder.HasIndex(u => u.Username)
            .IsUnique();
        
        builder.HasIndex(u => u.IsRelayEnabled);
    }
}