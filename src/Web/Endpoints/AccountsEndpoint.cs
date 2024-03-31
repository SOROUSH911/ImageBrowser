using ImageBrowser.Application.Accounts.Command;
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
        .MapPost(SignIn, "Login");
        //app.MapPost("logout", async (SignInManager<IdentityUser> signInManager) =>
        // {
        //     await signInManager.SignOutAsync().ConfigureAwait(false);
        // }).RequireAuthorization();
    }




    [AllowAnonymous]
    [HttpPost("SignIn")]
    public async Task<TokenDto> SignIn(ISender sender, SignInCommand command)
    {
        command.IsAdminPanel = false;
        return await sender.Send(command);
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
