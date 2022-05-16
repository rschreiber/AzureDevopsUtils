namespace AzureDevOpsUtils.ConsoleApp.Models;

public class PullRequest
{
    public Repository Repository { get; set; }
    public int PullRequestId { get; set; }
    public int CodeReviewId { get; set; }
    public string Status { get; set; }
    public CreatedBy CreatedBy { get; set; }
    public DateTime CreationDate { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string SourceRefName { get; set; }
    public string TargetRefName { get; set; }
    public string MergeStatus { get; set; }
    public bool IsDraft { get; set; }
    public string MergeId { get; set; }
    public LastMergeCommit LastMergeSourceCommit { get; set; }
    public LastMergeCommit LastMergeTargetCommit { get; set; }
    public LastMergeCommit LastMergeCommit { get; set; }
    public Reviewer[] Reviewers { get; set; }
    public string Url { get; set; }
    public CompletionOptions CompletionOptions { get; set; }
    public bool SupportsIterations { get; set; }
    public AutoCompleteSetBy AutoCompleteSetBy { get; set; }
}