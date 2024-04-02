


using System.Threading.Tasks;
using ImageBrowser.Application.Common.Event;
using ImageBrowser.Application.Common.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ImageBrowser.Infrastructure;

public class ExtractImageAttributesConsumer :
IConsumer<ExtractImageAttributesEvent>
{
    readonly ILogger<ExtractImageAttributesConsumer> _logger;
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileProvider _fileServices;

    public ExtractImageAttributesConsumer(ILogger<ExtractImageAttributesConsumer> logger, IApplicationDbContext dbContext, IFileProvider fileServices)
    {
        _logger = logger;
        _dbContext = dbContext;
        _fileServices = fileServices;
    }

    public async Task Consume(ConsumeContext<ExtractImageAttributesEvent> context)
    {
        var files = await _dbContext.Files.ToListAsync(new CancellationToken());
        _logger.LogInformation("Received Text: {Text}", context.Message.FileId);
        //return Task.CompletedTask;
    }
}
