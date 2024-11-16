using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC_News.Infrastructure.DbEntities;

namespace MVC_News.Infrastructure.Configurations;

public class ArticleConfigurations : IEntityTypeConfiguration<ArticleDbEntity>
{
    public void Configure(EntityTypeBuilder<ArticleDbEntity> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .ValueGeneratedNever();

        builder.Property(d => d.Title)
            .HasMaxLength(255);

        builder.Property(d => d.HeaderImage)
            .HasMaxLength(1028);

        builder.Property(d => d.Content)
            .HasMaxLength(10000);

        builder.Property(d => d.DateCreated)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(e => e.Tags)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
    }
}