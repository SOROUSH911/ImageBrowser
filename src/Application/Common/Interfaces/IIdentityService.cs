﻿using ImageBrowser.Application.Common.Models;

namespace ImageBrowser.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);


    Task<Result> DeleteUserAsync(string userId);
    Task<TokenDto> SignInUser(TokenRequest request, bool isAdminPanel, CancellationToken cancellationToken);
    Task<(Result Result, string UserId)> CreateUserAsync(string password, string firstName, string lastName, string email, string phone, int appUserId);
}
