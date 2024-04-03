
using ImageBrowser.Application.Common.Models;
using ImageBrowser.Application.Files.Commands;
using Microsoft.AspNetCore.Mvc;

namespace ImageBrowser.Web.Endpoints;

public class FilesEndpoint : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .RequireAuthorization()
            .MapPost(Upload);
    }


    [HttpPost("upload")]
    [RequestFormLimits(MultipartBodyLengthLimit = 52428800)]
    [RequestSizeLimit(52428800)]
    public async Task<ServiceResult> Upload(ISender sender, IFormFile file)
    {
        var res = await sender.Send(new AddFileCommand { Upload = file, Path = null });
        return res;
    }

}
