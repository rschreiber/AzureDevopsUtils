namespace AzureDevOpsUtils.ConsoleApp.Models;

public class Self
{
    public string Href { get; set; }
}

public class RepositorySelf
{
    public Repository repository { get; set; }
    public string refName { get; set; }
    public string version { get; set; }
}


