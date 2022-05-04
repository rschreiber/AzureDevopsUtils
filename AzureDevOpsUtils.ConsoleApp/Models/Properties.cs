namespace AzureDevOpsUtils.ConsoleApp.Models;

public class Properties
{
    public string CloneUrl { get; set; }
    public string FullName { get; set; }
    public string DefaultBranch { get; set; }
    public string IsFork { get; set; }
    public string SafeRepository { get; set; }
    public string ReportBuildStatus { get; set; }
    public string CleanOptions { get; set; }
    public string FetchDepth { get; set; }
    public string GitLfsSupport { get; set; }
    public string SkipSyncSource { get; set; }
    public string CheckoutNestedSubmodules { get; set; }
    public string LabelSources { get; set; }
    public string LabelSourcesFormat { get; set; }
}