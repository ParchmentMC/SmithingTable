namespace SmithingTable.Services;

public class ParchmentVersionService : IParchmentVersionService
{
    
    private readonly Dictionary<string, string> _versions = new();
    private readonly Dictionary<string, string> _snapshotVersions = new();

    public string? GetVersion(string gameVersion)
    {
        return _versions.TryGetValue(gameVersion, out var version) ? version : null;
    }

    public void SetVersion(string gameVersion, string version)
    {
        _versions[gameVersion] = version;
    }

    public string? GetSnapshotVersion(string gameVersion)
    {
        return _snapshotVersions.TryGetValue(gameVersion, out var version) ? version : null;
    }

    public void SetSnapshotVersion(string gameVersion, string version)
    {
        _snapshotVersions[gameVersion] = version;
    }

    public IEnumerable<string> GetVersions()
    {
        return _versions.Keys.Union(_snapshotVersions.Keys).Distinct();
    }
}
