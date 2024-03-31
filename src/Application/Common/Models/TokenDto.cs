using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Application.Common.Models;
public class TokenRequest
{
    [Required]
    public string grant_type { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Refresh_Token { get; set; }
    public string? Client_id { get; set; }
    public string? Client_secret { get; set; }
}

public class TokenDto
{
    public bool IsSuccess { get; set; }
    public string? Error { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public string? ExpiresIn { get; set; }
    public List<string?> Roles { get; set; }
}

public class TokenResult
{
    public string? Token { get; set; }
    public string? ExpiresIn { get; set; }
    public string? RefreshToken { get; set; }
}

public class TokenGeneratorOptions
{
    public string? SecretToken { get; set; }
    public string? Secreturl { get; set; }
    public string? EncriptionPassword { get; set; }
}
