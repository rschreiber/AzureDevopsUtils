using System.Text.Json.Serialization;

namespace AzureDevOpsUtils.ConsoleApp.Models;

public class PullRequestResponse
{
    [JsonPropertyName("Value")]
    public PullRequest[] PullRequests { get; set; }
    public int Count { get; set; }
}