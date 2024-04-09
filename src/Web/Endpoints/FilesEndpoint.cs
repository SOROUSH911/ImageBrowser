
using ImageBrowser.Application.Common.Models;
using ImageBrowser.Application.Files.Commands;
using ImageBrowser.Application.Files.Queries;
using ImageBrowser.Domain.SearchEngine;
using Microsoft.AspNetCore.Mvc;

namespace ImageBrowser.Web.Endpoints;

public class FilesEndpoint : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .RequireAuthorization()
            .MapPost(Upload)
            .MapPost(SearchFiles);
    }


    [HttpPost("upload")]
    [RequestFormLimits(MultipartBodyLengthLimit = 52428800)]
    [RequestSizeLimit(52428800)]
    public async Task<ServiceResult> Upload(ISender sender, IFormFile file)
    {
        var res = await sender.Send(new AddFileCommand { Upload = file, Path = null });
        return res;
    }

    public async Task<QueryResult<FileDto>> SearchFiles(ISender sendwer, SearchFilesQuery query)
    {
        var res = await sendwer.Send(query);
        return res;
    }



}
