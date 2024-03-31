using Microsoft.AspNetCore.Identity;

namespace ImageBrowser.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public int AppUserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime SignUpDate { get; set; }
}
