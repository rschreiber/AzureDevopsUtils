namespace AzureDevOpsUtils.ConsoleApp.Models;

public class BuildDefinition
{
    public Option[] Options { get; set; }
    public Trigger[] Triggers { get; set; }
    public Properties Properties { get; set; }
    public object[] Tags { get; set; }
    public Links Links { get; set; }
    public string JobAuthorizationScope { get; set; }
    public int JobTimeoutInMinutes { get; set; }
    public int JobCancelTimeoutInMinutes { get; set; }
    public Process Process { get; set; }
    public Repository Repository { get; set; }
    public string Quality { get; set; }
    public Authoredby AuthoredBy { get; set; }
    public object[] Drafts { get; set; }
    public Queue Queue { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public string Uri { get; set; }
    public string Path { get; set; }
    public string Type { get; set; }
    public string QueueStatus { get; set; }
    public int Revision { get; set; }
    public DateTime CreatedDate { get; set; }
    public Project Project { get; set; }
}