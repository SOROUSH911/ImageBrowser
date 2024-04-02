using ImageBrowser.Application.Common.Interfaces;

namespace ImageBrowser.Infrastructure.Services;

public class MockCurrentUser : IUser
{
    public string? Id => null;
}
