namespace AzureDevOpsUtils.ConsoleApp.Models;

public class Reviewer
{
    public string ReviewerUrl { get; set; }
    public int Vote { get; set; }
    public bool HasDeclined { get; set; }
    public bool IsRequired { get; set; }
    public bool IsFlagged { get; set; }
    public string DisplayName { get; set; }
    public string Url { get; set; }
    public Links2 Links { get; set; }
    public string Id { get; set; }
    public string UniqueName { get; set; }
    public string ImageUrl { get; set; }
    public bool IsContainer { get; set; }
    public Votedfor[] VotedFor { get; set; }
}