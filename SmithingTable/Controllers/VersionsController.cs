using Microsoft.AspNetCore.Mvc;
using SmithingTable.Model.Versioning;
using SmithingTable.Services;

namespace SmithingTable.Controllers;

[ApiController]
[Route("[controller]")]
public class VersionsController : ControllerBase
{

    private readonly IParchmentVersionService _versionService;    
    private readonly ILogger<VersionsController> _logger;

    
    public VersionsController(ILogger<VersionsController> logger, IParchmentVersionService versionService)
    {
        _logger = logger;
        _versionService = versionService;
    }

    [HttpGet(Name = "Versions")]
    public VersionInformation Versions()
    {
        var versions = _versionService.GetVersions();
        var releases = new Dictionary<string, string>();
        var snapshots = new Dictionary<string, string>();
        
        foreach(var version in versions)
        {
            var releasedName = _versionService.GetVersion(version);
            if (releasedName != null)
                releases[version] = releasedName;

            var snapshotName = _versionService.GetSnapshotVersion(version);
            if (snapshotName != null)
                snapshots[version] = snapshotName;
        }

        var result = new VersionInformation()
        {
            Releases = releases,
            Snapshots = snapshots
        };

        return result;
    }
}
