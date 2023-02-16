namespace SmithingTable.Services;

/// <summary>
/// Defines the interface for the parchment version service.
/// </summary>
public interface IParchmentVersionService
{
    /// <summary>
    /// Retrieves the current version of parchment for that game.
    /// </summary>
    /// <param name="gameVersion">The game version.</param>
    /// <returns>The released parchment version.</returns>
    public string? GetVersion(string gameVersion);
    
    /// <summary>
    /// Sets the current version of parchment for that game.
    /// </summary>
    /// <param name="gameVersion">The game version.</param>
    /// <param name="version">The newly released parchment version.</param>
    public void SetVersion(string gameVersion, string version);
    
    /// <summary>
    /// Retrieves the current snapshot version of parchment for that game.
    /// </summary>
    /// <param name="gameVersion">The game version.</param>
    /// <returns>The released parchment version.</returns>
    public string? GetSnapshotVersion(string gameVersion);
    
    /// <summary>
    /// Sets the current snapshot version of parchment for that game.
    /// </summary>
    /// <param name="gameVersion">The game version.</param>
    /// <param name="version">The snapshot parchment version.</param>
    public void SetSnapshotVersion(string gameVersion, string version);
    
    /// <summary>
    /// Returns the list of known game versions.
    /// </summary>
    /// <returns>The list of known game versions.</returns>
    public IEnumerable<string> GetVersions();
}
