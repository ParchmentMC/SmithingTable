namespace SmithingTable.Services;

/// <summary>
/// Defines a service that updates the versions of parchment.
/// </summary>
public interface IParchmentVersionUpdater
{
    /// <summary>
    /// Performs the updates of the versions.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation</param>
    Task UpdateVersionsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if the versions can be updated.
    /// </summary>
    /// <returns>True if possible</returns>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation</param>
    Task<bool> CanUpdateVersionsAsync(CancellationToken cancellationToken = default);
}