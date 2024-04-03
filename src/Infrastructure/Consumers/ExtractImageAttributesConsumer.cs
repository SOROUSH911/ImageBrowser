


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
    private readonly IOCRService _ocrSevice;

    public ExtractImageAttributesConsumer(ILogger<ExtractImageAttributesConsumer> logger, IApplicationDbContext dbContext, IFileProvider fileServices, IOCRService ocrSevice)
    {
        _logger = logger;
        _dbContext = dbContext;
        _fileServices = fileServices;
        _ocrSevice = ocrSevice;
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

        //return Task.CompletedTask;
    }
}
