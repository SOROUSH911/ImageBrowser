﻿using System.Reflection;
using System.Reflection.Emit;
using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Domain.Entities;
using ImageBrowser.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ImageBrowser.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    //public DbSet<TodoList> TodoLists => Set<TodoList>();

    //public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<User> AppUsers { get; }
    public DbSet<Domain.Entities.File> Files { get; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Domain.Entities.File>()
           .HasOne(i => i.Owner).WithOne();

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
