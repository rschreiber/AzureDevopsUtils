namespace AzureDevOpsUtils.ConsoleApp.Models;

public class Votedfor
{
    public string ReviewerUrl { get; set; }
    public int Vote { get; set; }
    public string DisplayName { get; set; }
    public string Url { get; set; }
    public Links Links { get; set; }
    public string Id { get; set; }
    public string UniqueName { get; set; }
    public string ImageUrl { get; set; }
    public bool IsContainer { get; set; }
}