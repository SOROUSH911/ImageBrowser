using ImageBrowser.Domain.Entities;
using File = ImageBrowser.Domain.Entities.File;

namespace ImageBrowser.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    //DbSet<TodoList> TodoLists { get; }

    //DbSet<TodoItem> TodoItems { get; }
    DbSet<Client> Clients { get; set; }
    DbSet<User> AppUsers { get; set; }
    DbSet<File> Files { get; set; }
    DbSet<RefreshToken> RefreshTokens { get; set; }



    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
