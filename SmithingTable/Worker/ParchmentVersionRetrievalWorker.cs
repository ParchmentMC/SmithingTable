using System.ComponentModel;
using SmithingTable.Model.Artifactory;
using SmithingTable.Model.Versioning;
using SmithingTable.Services;

namespace SmithingTable.Worker;

public class ParchmentVersionRetrievalWorker : BackgroundService
{
    private readonly IParchmentVersionUpdater _parchmentVersionUpdater;

    private readonly ILogger<ParchmentVersionRetrievalWorker> _logger;


    public ParchmentVersionRetrievalWorker(IParchmentVersionUpdater parchmentVersionUpdater, ILogger<ParchmentVersionRetrievalWorker> logger)
    {
        _parchmentVersionUpdater = parchmentVersionUpdater;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateVersions(stoppingToken);   
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"Failed to get versions, retrying in 1 minute");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task UpdateVersions(CancellationToken cancellationToken)
    {
        await _parchmentVersionUpdater.UpdateVersionsAsync(cancellationToken);
    }
}
