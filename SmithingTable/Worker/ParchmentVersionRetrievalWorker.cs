using System.ComponentModel;
using SmithingTable.Model.Artifactory;
using SmithingTable.Model.Versioning;
using SmithingTable.Services;

namespace SmithingTable.Worker;

public class ParchmentVersionRetrievalWorker : BackgroundService
{
    private const string BASE_URL = "https://ldtteam.jfrog.io/artifactory/api/storage/parchmentmc-public/org/parchmentmc/data";
    
    private readonly IParchmentVersionService _parchmentVersionService;

    private readonly ILogger<ParchmentVersionRetrievalWorker> _logger;


    public ParchmentVersionRetrievalWorker(IParchmentVersionService parchmentVersionService, ILogger<ParchmentVersionRetrievalWorker> logger)
    {
        _parchmentVersionService = parchmentVersionService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateVersions();   
            }
            catch(Exception ex)
            {
                _logger.LogError("Failed to get versions, retrying in 1 minute", ex);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task UpdateVersions()
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetFromJsonAsync<RepositoryInformation>(BASE_URL);
        if (response == null)
            return;
        
        foreach(var child in response.Children)
        {
            if (!child.Folder || !child.Uri.Contains("parchment-"))
                continue;
            
            var gameVersion = child.Uri.Split("-").Last();
            var requestPath = BASE_URL + child.Uri;
            
            var gameVersionResponse = await httpClient.GetFromJsonAsync<RepositoryInformation>(requestPath);
            if (gameVersionResponse == null)
                continue;

            var versionChildren = gameVersionResponse.Children.Where(c =>
                c.Folder &&
                !c.Uri.Contains("BLEEDING-SNAPSHOT") &&
                !c.Uri.Contains("maven-metadata.xml")).ToList();

            var releaseVersionNames = versionChildren.Where(c => !c.Uri.Contains("-SNAPSHOT")).Select(c => c.Uri[1..]);
            var snapshotVersionNames = versionChildren.Where(c => c.Uri.Contains("-SNAPSHOT")).Select(c => c.Uri[1..]);

            var releaseVersions = new List<ParchmentVersion>();
            var snapshotVersions = new List<ParchmentVersion>();
            foreach(var releaseVersionName in releaseVersionNames)
            {
                if (DateTime.TryParse(releaseVersionName, out var releaseDate))
                    releaseVersions.Add(new ParchmentVersion()
                    {
                        Name = releaseVersionName,
                        ReleasedOn = releaseDate
                    });
            }
            
            foreach(var snapshotVersionName in snapshotVersionNames)
            {
                var snapshotVersionDate = snapshotVersionName.Split("-")[0];
                if (DateTime.TryParse(snapshotVersionDate, out var snapshotDate))
                    snapshotVersions.Add(new ParchmentVersion()
                    {
                        Name = snapshotVersionName,
                        ReleasedOn = snapshotDate
                    });
            }
            
            var latestRelease = releaseVersions.MaxBy(v => v.ReleasedOn);
            var latestSnapshot = snapshotVersions.MaxBy(v => v.ReleasedOn);
            
            if (latestRelease != null)
                _parchmentVersionService.SetVersion(gameVersion, latestRelease.Name);
            
            if (latestSnapshot != null)
                _parchmentVersionService.SetSnapshotVersion(gameVersion, latestSnapshot.Name);
        }
    }
}
