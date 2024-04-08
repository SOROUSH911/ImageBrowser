using System.Reflection;
using System.Reflection.Emit;
using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Domain.Entities;
using ImageBrowser.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;

namespace ImageBrowser.Infrastructure.Data;

public class ApplicationDbContext : AppDbContextBase, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    //public DbSet<TodoList> TodoLists => Set<TodoList>();

    //public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<Client> Clients { get; set; }
    public DbSet<User> AppUsers { get; set; }
    public DbSet<Domain.Entities.File> Files { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSnakeCaseNamingConvention();

    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<HistoryRow>().HasKey(h => h.MigrationId);
        builder.Entity<HistoryRow>().Property(h => h.MigrationId).HasColumnName("migration_id");
        builder.Entity<HistoryRow>().Property(h => h.ProductVersion).HasColumnName("product_version");


        builder.Entity<Domain.Entities.File>()
           .HasOne(i => i.Owner).WithMany();

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
