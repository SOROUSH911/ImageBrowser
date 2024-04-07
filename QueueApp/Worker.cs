using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Application.Common.Event;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace QueueApp;
public class Worker : BackgroundService
{
    readonly IBus _bus;

    public Worker(IBus bus)
    {
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            //await _bus.Publish(new ExtractImageAttributesEvent { FileId = 11 }, stoppingToken);

            await Task.Delay(10000000, stoppingToken);
        }
    }
}
