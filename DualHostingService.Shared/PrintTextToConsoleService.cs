using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DualHostingService.Shared
{
    public class PrintTextToConsoleService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<ServiceConfig> _config;

        private readonly IList<IDisposable> _disposables = new List<IDisposable>();

        public PrintTextToConsoleService(ILogger<PrintTextToConsoleService> logger, IOptionsMonitor<ServiceConfig> configMonitor)
        {
            _logger = logger;
            _config = configMonitor;

            _disposables.Add(configMonitor.OnChange(c =>
            {
                _logger.LogInformation(c.ToJsonString());
            }));
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);

            _logger.LogInformation("Starting");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            _logger.LogInformation("Stopping.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Background work with text: {_config.CurrentValue.TextToPrint}");

                await Task.Delay(_config.CurrentValue.Delay);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach(var x in _disposables)
            {
                x?.Dispose();
            }
        }
    }
}
