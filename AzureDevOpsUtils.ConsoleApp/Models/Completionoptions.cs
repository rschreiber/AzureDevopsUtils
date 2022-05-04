namespace AzureDevOpsUtils.ConsoleApp.Models;

public class Completionoptions
{
    public string MergeCommitMessage { get; set; }
    public bool DeleteSourceBranch { get; set; }
    public string MergeStrategy { get; set; }
    public bool TransitionWorkItems { get; set; }
    public object[] AutoCompleteIgnoreConfigIds { get; set; }
}