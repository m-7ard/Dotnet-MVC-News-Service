using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC_News.Infrastructure.DbEntities;

namespace MVC_News.Infrastructure.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<UserDbEntity>
{
    public void Configure(EntityTypeBuilder<UserDbEntity> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .ValueGeneratedNever();

        builder.Property(d => d.Email)
            .HasMaxLength(255);

        builder.Property(d => d.DisplayName)
            .HasMaxLength(64);
    }
}