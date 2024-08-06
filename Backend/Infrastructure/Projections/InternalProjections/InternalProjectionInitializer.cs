using System.Reflection;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Projections.InternalProjections
{
    public class InternalProjectionInitializer : IHostedService
    {
        private readonly ILogger<InternalProjectionInitializer> _logger;
        private readonly EventStoreProjectionManagementClient _eventStoreClient;
        private readonly IReadOnlyCollection<(string Name, string FileName)> _projections;

        public InternalProjectionInitializer(
            ILogger<InternalProjectionInitializer> logger,
            EventStoreProjectionManagementClient eventStoreClient,
            [FromKeyedServices("InternalProjections")]
                IReadOnlyCollection<(string Name, string FileName)> projections
        )
        {
            _logger = logger;
            _eventStoreClient = eventStoreClient;
            _projections = projections;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var existingProjections = new HashSet<string>();
            try
            {
                existingProjections = await _eventStoreClient
                    .ListContinuousAsync()
                    .Select(e => e.Name)
                    .ToHashSetAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            foreach (var projection in _projections)
            {
                try
                {
                    var code = LoadJavaScriptFile(projection.FileName);

                    if (code == null)
                    {
                        _logger.LogWarning($"{projection.FileName} file not found!");
                        continue;
                    }

                    if (existingProjections.Contains(projection.Name))
                    {
                        await _eventStoreClient.UpdateAsync(
                            projection.Name,
                            code,
                            emitEnabled: true
                        );
                    }
                    else
                    {
                        await _eventStoreClient.CreateContinuousAsync(
                            projection.Name,
                            code,
                            cancellationToken: cancellationToken
                        );
                        await _eventStoreClient.UpdateAsync(
                            projection.Name,
                            code,
                            emitEnabled: true
                        ); // update after create to enable emit
                    }

                    await _eventStoreClient.EnableAsync(projection.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private string? LoadJavaScriptFile(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName =
                $"Infrastructure.Projections.InternalProjections.JavaScript.{fileName}.js";

            string result;

            using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    return null;
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }
    }
}
