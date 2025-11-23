using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Entities;

namespace Users.Infrastructure.Data.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .HasColumnName("user_id")
                .ValueGeneratedOnAdd();
            builder.Property(u => u.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();
            builder.Property(u => u.Email)
                .HasColumnName("email")
                .HasMaxLength(40)
                .IsRequired();
            builder.Property(u => u.Role)
                .HasColumnName("role")
                .HasMaxLength(20)
                .IsRequired();
            builder.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(u => u.EmailConfirmed)
               .HasColumnName("email_confirmed")
               .IsRequired();

        }
    }
}
