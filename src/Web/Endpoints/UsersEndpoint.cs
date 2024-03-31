using ImageBrowser.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImageBrowser.Web.Endpoints;

public class UsersEndpoint : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
 
            .MapIdentityApi<ApplicationUser>();
        //.MapGet(SignIn);
        //app.MapPost("logout", async (SignInManager<IdentityUser> signInManager) =>
        // {
        //     await signInManager.SignOutAsync().ConfigureAwait(false);
        // }).RequireAuthorization();
    }


  

    //[AllowAnonymous]
    //[HttpPost("SignIn")]
    //public async Task<TokenDto> SignIn(SignInCommand command)
    //{
    //    command.IsAdminPanel = false;
    //    return await Mediator.Send(command);
    //}

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
