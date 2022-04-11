namespace AzureDevOpsUtils.ConsoleApp.Models;

public class Trigger
{
    public object[] branchFilters { get; set; }
    public object[] pathFilters { get; set; }
    public int settingsSourceType { get; set; }
    public bool batchChanges { get; set; }
    public int maxConcurrentBuildsPerBranch { get; set; }
    public string triggerType { get; set; }
}