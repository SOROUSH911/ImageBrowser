using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ImageBrowser.Infrastructure.Services;
public class AppUserIdService : IAppUserIdService
{

    private const string ClaimType = "UserId";

    public AppUserIdService(IHttpContextAccessor httpContextAccessor)
    {
        var value = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimType);

        if (!string.IsNullOrEmpty(value))
        {
            UserId = int.Parse(value);
            UserName = httpContextAccessor.HttpContext?.User?.Identity?.Name;
        }

        try
        {
            var isAdmin = httpContextAccessor?.HttpContext?.User.IsInRole("Admin");
            IsAdmin = isAdmin != null && isAdmin.Value;
        }
        catch (Exception e)
        {
        }
    }

    public int UserId { get; set; }
    public string? UserName { get; set; }
    public bool IsAdmin { get; set; }
}
