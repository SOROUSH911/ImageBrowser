using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace ImageBrowser.Infrastructure.Data;
public abstract class AppDbContextBase : IdentityDbContext<ApplicationUser>
{
    private IDbContextTransaction currentTransaction;
    protected AppDbContextBase(DbContextOptions options) : base(options)
    {
    }

    //public IEnumerable<EventBase> GetDomainEvents()
    //{
    //    var domainEntities = ChangeTracker
    //        .Entries<EntityRootBase>()
    //        .Where(x =>
    //            x.Entity.DomainEvents != null &&
    //            x.Entity.DomainEvents.Any())
    //        .ToList();

    //    var domainEvents = domainEntities
    //        .SelectMany(x => x.Entity.DomainEvents)
    //        .ToList();

    //    domainEntities.ForEach(entity => entity.Entity.DomainEvents.Clear());

    //    return domainEvents;
    //}

    public async Task BeginTransactionAsync()
    {
        if (currentTransaction != null)
        {
            return;
        }

        currentTransaction = await base.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted).ConfigureAwait(false);
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync().ConfigureAwait(false);

            currentTransaction?.Commit();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (currentTransaction != null)
            {
                currentTransaction.Dispose();
                currentTransaction = null;
            }
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            currentTransaction?.Rollback();
        }
        finally
        {
            if (currentTransaction != null)
            {
                currentTransaction.Dispose();
                currentTransaction = null;
            }
        }
    }
    public IDbConnection GetDbConnection()
    {
        return Database.GetDbConnection();
    }
    //public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    //{
    //    foreach (var entry in ChangeTracker.Entries<AuditableEntityBase>())
    //    {
    //        switch (entry.State)
    //        {
    //            case EntityState.Added:
    //                entry.Entity.CreateDate = DateTime.UtcNow;
    //                entry.Entity.LastModifiedDate = DateTime.UtcNow;
    //                break;

    //            case EntityState.Modified:
    //                entry.Entity.LastModifiedDate = DateTime.UtcNow;
    //                break;
    //        }
    //    }
    //    Task<int> result = base.SaveChangesAsync(cancellationToken);
    //    return result;
    //}
}
