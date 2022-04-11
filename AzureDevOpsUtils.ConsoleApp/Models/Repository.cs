namespace AzureDevOpsUtils.ConsoleApp.Models;

public class Repository
{
    public Properties1 properties { get; set; }
    public string id { get; set; }
    public string type { get; set; }
    public string name { get; set; }
    public string url { get; set; }
    public string defaultBranch { get; set; }
    public object clean { get; set; }
    public bool checkoutSubmodules { get; set; }
}