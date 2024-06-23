using SmithingTable.Model.Artifactory;
using SmithingTable.Model.Versioning;

namespace SmithingTable.Services;

public class ParchmentVersionUpdater : IParchmentVersionUpdater
{
    private readonly IParchmentVersionService _parchmentVersionService;

    public ParchmentVersionUpdater(IParchmentVersionService parchmentVersionService)
    {
        _parchmentVersionService = parchmentVersionService;
    }

    private const string BaseUrl = "https://ldtteam.jfrog.io/artifactory/api/storage/parchmentmc-public/org/parchmentmc/data";
    
    public async Task UpdateVersionsAsync(CancellationToken cancellationToken = default)
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetFromJsonAsync<RepositoryInformation>(BaseUrl, cancellationToken);
        if (response == null)
            return;
        
        foreach(var child in response.Children)
        {
            if (!child.Folder || !child.Uri.Contains("parchment-"))
                continue;
            
            var gameVersion = child.Uri.Split("-").Last();
            var requestPath = BaseUrl + child.Uri;
            
            var gameVersionResponse = await httpClient.GetFromJsonAsync<RepositoryInformation>(requestPath, cancellationToken);
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

    public async Task<bool> CanUpdateVersionsAsync(CancellationToken cancellationToken = default)
    {
        var httpClient = new HttpClient();
        var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, BaseUrl), cancellationToken);
        return response.IsSuccessStatusCode;
    }
}