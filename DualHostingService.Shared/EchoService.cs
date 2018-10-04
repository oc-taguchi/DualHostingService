using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DualHostingService.Shared
{
    public class EchoService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly EchoConfig _config;

        private readonly IList<IDisposable> _disposables = new List<IDisposable>();
        private HttpListener _listener;

        public EchoService(ILogger<PrintTextToConsoleService> logger, IOptions<EchoConfig> config)
        {
            _logger = logger;
            _config = config.Value;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);

            _logger.LogInformation("Starting");
        }

        private async Task StartListener(CancellationToken cancellationToken)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(_config.Url);
            listener.Start();
            cancellationToken.Register(() => listener.Stop());

            while (!cancellationToken.IsCancellationRequested)
            {
                var context = await listener.GetContextAsync();
                var req = context.Request;
                var res = context.Response;

                _logger.LogInformation("Connected From: " + req.RemoteEndPoint);
                await Task.Delay(500);

                var response = $"{req.HttpMethod} {req.RawUrl} HTTP/{req.ProtocolVersion}\r\n";
                response += string.Join("\r\n", req.Headers.AllKeys.Select(k => $"{k}: {req.Headers[k]}"));

                var responsedata = Encoding.ASCII.GetBytes(response);
                res.OutputStream.Write(responsedata, 0, responsedata.Length);
                res.Close();
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            _logger.LogInformation("Stopping.");
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => StartListener(stoppingToken);

        public override void Dispose()
        {
            base.Dispose();

            foreach (var x in _disposables)
            {
                x?.Dispose();
            }
        }
    }
}
