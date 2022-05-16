namespace AzureDevOpsUtils.ConsoleApp.Models;

public class Issue
{
    public string Type { get; set; }
    public string Category { get; set; }
    public string Message { get; set; }
    public Data Data { get; set; }
}