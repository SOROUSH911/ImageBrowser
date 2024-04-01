using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Application.Common.Models;
using ImageBrowser.Domain.Common;
using ImageBrowser.Domain.Entities;
using ImageBrowser.Infrastructure.Configurations;
using ImageBrowser.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace ImageBrowser.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly SignInManager<ApplicationUser> _signinManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IApplicationDbContext _dbContext;
    private readonly TokenConfiguration tokenConfigs;
    private readonly IAuthorizationService _authorizationService;
    private readonly ITokenFactory tokenFactory;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        SignInManager<ApplicationUser> signinManager, RoleManager<IdentityRole> roleManager, IApplicationDbContext dbContext, IOptions<TokenConfiguration> tokenConfigs,
        IAuthorizationService authorizationService, ITokenFactory tokenFactory)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _signinManager = signinManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
        this.tokenConfigs = tokenConfigs.Value;
        _authorizationService = authorizationService;
        this.tokenFactory = tokenFactory;
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.UserName;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string password, string firstName, string lastName, string email, string phone, int appUserId)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                PhoneNumber = phone,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                SignUpDate = DateTime.UtcNow,
                AppUserId = appUserId
            };

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }


    public async Task<TokenDto> SignInUser(TokenRequest request, bool isAdminPanel, CancellationToken cancellationToken)
    {
        var client = await _dbContext.Clients
          .SingleOrDefaultAsync(a => a.Id.Equals(request.Client_id) && a.Secret == request.Client_secret && a.Active, cancellationToken);
        if (client == null)
        {
            return TokenDto.Failure("invalid_client");
        }

        var options = tokenConfigs;
        var secretToken = options.SecretToken;
        var secreturl = options.VarifiedSecretUrl;
        string encPassword = options.EncRefPassword;

        bool throughPassword = false;
        DateTime expireDate = new DateTime();
        ApplicationUser user = new ApplicationUser();
        IList<string> userRole = new List<string>();

        if (request.grant_type.ToLower().Equals("password"))
        {
            user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return TokenDto.Failure("wrong_pass");
            }

            var passwordIsCorrect = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!passwordIsCorrect)
            {
                return TokenDto.Failure("wrong_pass");
            }

            userRole = await _userManager.GetRolesAsync(user);
            throughPassword = true;
            // chek roles for admin section
        }
        else if (request.grant_type.ToLower().Equals("refresh_token"))
        {
            // refresh token db
            string encRefTokenFromUser = Cipher.Encrypt(request.Refresh_Token, encPassword);

            var refToken = await _dbContext.RefreshTokens.Where(a => a.ClientId == request.Client_id
            && a.ProtectedTicket.Equals(encRefTokenFromUser) && a.ExpiresUtc > DateTime.UtcNow)
            .SingleOrDefaultAsync(cancellationToken);

            if (refToken == null)
            {
                return TokenDto.Failure("token_not_found");
            }
            expireDate = refToken.ExpiresUtc;
            user = await _userManager.FindByNameAsync(refToken.Subject);
            userRole = await _userManager.GetRolesAsync(user);
            //  dbContext.RefreshTokens.Remove(refToken);
            //saveChanges
        }
        else
        {
            return TokenDto.Failure("grant_type_not_supported");
        }
        try
        {
            var role = userRole.ToList();
            if (isAdminPanel && (!role.Contains("Admin")))
            {
                return TokenDto.Failure("no_access");
            }

            int appUserId = 0;
            var appUser = await _dbContext.AppUsers.SingleOrDefaultAsync(u => u.Id == user.AppUserId && !u.IsDeleted, cancellationToken);
            if (appUser == null)
            {
                return TokenDto.Failure("wrong_pass");
            }
            //if (appUser.IsDisabled)
            //{
            //    return TokenDto.Failure("disabled_user");
            //}
            var isActive = await _userManager.IsEmailConfirmedAsync(user);
            if (!isActive)
            {
                //if (appUser.LastTimeSent == null || appUser.LastTimeSent.Value.AddMinutes(15) < DateTime.UtcNow)
                //{
                //    await RequestActiveUser(user.Email, default);
                //    return Result.Failure<TokenDto>("ایمیل اکانت شما تایید نشده ,  به ایمیل خود مراجعه نمایید");
                //}
                //return Result.Failure<TokenDto>("ایمیل شما تایید نشده");
            }

            appUserId = appUser.Id;


            #region login

            var tokens = tokenFactory.TokenGenerator(secretToken, secreturl, user.UserName, role, user.Id, appUserId, client.RefreshTokenLifeTime, client.AccessTokenLifeTime);
            var tokenString = tokens.Token;
            var refToken = tokens.RefreshToken;

            var encRefreshToken = Cipher.Encrypt(refToken, encPassword);


            // save refresh token with client
            var new_refreshtoken = new RefreshToken()
            {
                Id = Guid.NewGuid() + "",
                ClientId = client.Id,
                ExpiresUtc = throughPassword ? DateTime.UtcNow.AddMinutes(client.RefreshTokenLifeTime) : expireDate,
                IssuedUtc = DateTime.UtcNow,
                ProtectedTicket = encRefreshToken,
                Subject = user.UserName
            };

            _dbContext.RefreshTokens.Add(new_refreshtoken);

            var preRefreshTokens = await _dbContext.RefreshTokens.Where(a => a.ClientId == request.Client_id && a.Subject.ToUpper().Equals(user.NormalizedUserName))
                .ToListAsync(cancellationToken);

            if (preRefreshTokens.Count() > 0)
            {
                _dbContext.RefreshTokens.RemoveRange(preRefreshTokens);
            }
            await _dbContext.SaveChangesAsync(cancellationToken);

            #endregion

            return new TokenDto
            {
                Token = tokenString,
                RefreshToken = refToken,
                Roles = role,
                ExpiresIn = tokens.ExpiresIn,
                IsSuccess = true
            };
        }
        catch (Exception e)
        {
            return TokenDto.Failure("exception");
        }
    }
}
