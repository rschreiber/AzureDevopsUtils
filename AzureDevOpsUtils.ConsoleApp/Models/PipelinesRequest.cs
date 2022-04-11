using System.Text.Json.Serialization;

namespace AzureDevOpsUtils.ConsoleApp.Models;

public class PipelinesRequest
{
    public int Count { get; set; }
    [JsonPropertyName("Value")]
    public Pipeline[] Pipelines { get; set; }
}