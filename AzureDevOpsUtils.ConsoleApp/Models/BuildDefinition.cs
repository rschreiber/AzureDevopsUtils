namespace AzureDevOpsUtils.ConsoleApp.Models;

public class BuildDefinition
{
    public Option[] options { get; set; }
    public Trigger[] triggers { get; set; }
    public Properties properties { get; set; }
    public object[] tags { get; set; }
    public _Links _links { get; set; }
    public string jobAuthorizationScope { get; set; }
    public int jobTimeoutInMinutes { get; set; }
    public int jobCancelTimeoutInMinutes { get; set; }
    public Process process { get; set; }
    public Repository repository { get; set; }
    public string quality { get; set; }
    public Authoredby authoredBy { get; set; }
    public object[] drafts { get; set; }
    public Queue queue { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string url { get; set; }
    public string uri { get; set; }
    public string path { get; set; }
    public string type { get; set; }
    public string queueStatus { get; set; }
    public int revision { get; set; }
    public DateTime createdDate { get; set; }
    public Project project { get; set; }
}