using ImageBrowser.Domain.Entities;
using File = ImageBrowser.Domain.Entities.File;

namespace ImageBrowser.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    //DbSet<TodoList> TodoLists { get; }

    //DbSet<TodoItem> TodoItems { get; }
    DbSet<User> AppUsers { get; }
    DbSet<File> Files { get; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
