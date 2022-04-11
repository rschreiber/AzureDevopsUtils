namespace AzureDevOpsUtils.ConsoleApp.Models;

public class Queue
{
    public _Links2 _links { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string url { get; set; }
    public Pool pool { get; set; }
}