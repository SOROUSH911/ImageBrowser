


using System.Threading.Tasks;
using ImageBrowser.Application.Common.Event;
using ImageBrowser.Application.Common.Helpers;
using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Domain.SearchEngine;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolrNet;

namespace ImageBrowser.Infrastructure;

public class ExtractImageAttributesConsumer :
IConsumer<ExtractImageAttributesEvent>
{
    readonly ILogger<ExtractImageAttributesConsumer> _logger;
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileProvider _fileServices;
    private readonly IOCRService _ocrSevice;
    private readonly ISolrOperations<IBDataSchema> _solrOperations;

    public ExtractImageAttributesConsumer(ILogger<ExtractImageAttributesConsumer> logger, IApplicationDbContext dbContext, IFileProvider fileServices, IOCRService ocrSevice, ISolrOperations<IBDataSchema> solrOperations)
    {
        _logger = logger;
        _dbContext = dbContext;
        _fileServices = fileServices;
        _ocrSevice = ocrSevice;
        _solrOperations = solrOperations;
    }

    public async Task Consume(ConsumeContext<ExtractImageAttributesEvent> context)
    {
        _logger.LogInformation("Received Text: {Text}", context.Message.FileId);
        var file = await _dbContext.Files.SingleOrDefaultAsync(f => f.Id == context.Message.FileId);
        if (file == null)
        {
            //_logger.LogInformation("File not found");
            throw new Exception("File not found");
        }

  

        var wordsList = await _ocrSevice.ExtractText(file.Path);



        var p = new IBDataSchema
        {
            Id = SolrHelper.GetEntitySolrId(file.Id),

            Categories = null,
            TextContent = wordsList,
            Size = file.Size,
            Name = file.Name,
            OwnerId = file.OwnerId,
            Path = file.Path
        };

        _solrOperations.Add(p);
        _solrOperations.Commit();

        //return Task.CompletedTask;
    }
}
