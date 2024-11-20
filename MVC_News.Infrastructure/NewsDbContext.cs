using Microsoft.EntityFrameworkCore;
using MVC_News.Infrastructure.DbEntities;


namespace MVC_News.Infrastructure;

public class NewsDbContext : DbContext
{
    public NewsDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<UserDbEntity> User { get; set; } = null!;
    public DbSet<ArticleDbEntity> Article { get; set; } = null!;
    public DbSet<SubscriptionDbEntity> Subscription { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NewsDbContext).Assembly);
    }
}