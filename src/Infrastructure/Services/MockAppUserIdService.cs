using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Application.Common.Interfaces;

namespace ImageBrowser.Infrastructure.Services;
public class MockAppUserIdService : IAppUserIdService
{
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public bool IsAdmin { get; set; }
}
