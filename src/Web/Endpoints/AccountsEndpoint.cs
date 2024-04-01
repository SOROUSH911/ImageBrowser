using ImageBrowser.Application.Accounts.Commands;
using ImageBrowser.Application.Accounts.Queries;
using ImageBrowser.Application.Common.Models;
using ImageBrowser.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImageBrowser.Web.Endpoints;

public class AccountsEndpoint : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)

        //.MapIdentityApi<ApplicationUser>();
        .MapPost(SignIn, "Login")
        .MapPost(SignUp, "SignUp")
        .MapGet(GetProfile, "GetProfile");
        //app.MapPost("logout", async (SignInManager<IdentityUser> signInManager) =>
        // {
        //     await signInManager.SignOutAsync().ConfigureAwait(false);
        // }).RequireAuthorization();
    }



    [AllowAnonymous]
    [HttpPost("SignUp")]
    public async Task<ServiceResult> SignUp(ISender sender, SignUpCommand command)
    {
        return await sender.Send(command);
    }





    [AllowAnonymous]
    [HttpPost("SignIn")]
    public async Task<TokenDto> SignIn(ISender sender, SignInCommand command)
    {
        command.IsAdminPanel = false;
        return await sender.Send(command);
    }



    [HttpGet("Get-Profile")]
    public async Task<UserDto> GetProfile(ISender sender)
    {
        var res = await sender.Send(new GetProfileQuery());
        return res;
    }

    //[AllowAnonymous]
    //[HttpPost("ForgetPassword")]
    //public async Task<CommandResult> ForgetPassowrd(string Email)
    //{
    //    return await Mediator.Send(new ForgetPasswordCommand() { Email = Email });
    //}

    //[HttpGet("Get-Profile")]
    //public async Task<UserDto> GetProfile()
    //{
    //    var res = await Mediator.Send(new GetProfileQuery());
    //    return res;
    //}

}
