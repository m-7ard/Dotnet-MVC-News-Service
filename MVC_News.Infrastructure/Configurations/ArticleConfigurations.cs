using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries),
                new ValueComparer<string[]>(
                    (c1, c2) =>
                        (c1 == null && c2 == null) ||
                        (c1 != null && c2 != null && c1.SequenceEqual(c2)), // Compare arrays, handle null
                    c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), // Generate hash code, handle null
                    c => c == null ? new string[0] : c.ToArray() // Copy array, return empty array if null
                ));
    }
}