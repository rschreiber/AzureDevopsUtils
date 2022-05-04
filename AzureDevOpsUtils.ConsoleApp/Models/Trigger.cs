namespace AzureDevOpsUtils.ConsoleApp.Models;

public class Trigger
{
    public object[] BranchFilters { get; set; }
    public object[] PathFilters { get; set; }
    public int SettingsSourceType { get; set; }
    public bool BatchChanges { get; set; }
    public int MaxConcurrentBuildsPerBranch { get; set; }
    public string TriggerType { get; set; }
}