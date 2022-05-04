namespace AzureDevOpsUtils.ConsoleApp.Models;

public class Queue
{
    public Links Links { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public Pool Pool { get; set; }
}