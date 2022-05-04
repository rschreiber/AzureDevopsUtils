namespace AzureDevOpsUtils.ConsoleApp.Models;

public partial class Repository
{
    public Project Project { get; set; }
    public bool IsFork { get; set; }
    public Properties Properties { get; set; }
    public string Id { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public string DefaultBranch { get; set; }
    public object Clean { get; set; }
    public bool CheckoutSubmodules { get; set; }
}