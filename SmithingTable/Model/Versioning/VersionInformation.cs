namespace SmithingTable.Model.Versioning;

public class VersionInformation
{
    public Dictionary<string, string> Releases { get; set; }
    
    public Dictionary<string, string> Snapshots { get; set; }
}
