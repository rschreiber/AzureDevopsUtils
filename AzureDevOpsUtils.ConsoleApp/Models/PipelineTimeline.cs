namespace AzureDevOpsUtils.ConsoleApp.Models;

public class PipelineTimeline
{
    public Record[] Records { get; set; }
    public string LastChangedBy { get; set; }
    public DateTime LastChangedOn { get; set; }
    public string Id { get; set; }
    public int ChangeId { get; set; }
    public string Url { get; set; }
}