


using System.Threading.Tasks;
using ImageBrowser.Application.Common.Event;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ImageBrowser.Infrastructure;

public class ExtractImageAttributesConsumer :
IConsumer<ExtractImageAttributesEvent>
{
    readonly ILogger<ExtractImageAttributesConsumer> _logger;

    public ExtractImageAttributesConsumer(ILogger<ExtractImageAttributesConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<ExtractImageAttributesEvent> context)
    {
        _logger.LogInformation("Received Text: {Text}", context.Message.FileId);
        return Task.CompletedTask;
    }
}
