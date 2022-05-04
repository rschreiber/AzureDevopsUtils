using System.Text.Json.Serialization;

namespace AzureDevOpsUtils.ConsoleApp.Models;

public partial class PipelineLink
{
    public string href { get; set; }
}

public class Pipeline
{
    [JsonPropertyName("_links")] public Links Links { get; set; }
    public string Url { get; set; }
    public int Id { get; set; }
    public int Revision { get; set; }
    public string Name { get; set; }
    public string Folder { get; set; }
}