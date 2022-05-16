namespace AzureDevOpsUtils.ConsoleApp.Models;




public class PipelineRunInformationResponse
{
    public PipelineLinks Links { get; set; }
    public Pipeline1 Pipeline { get; set; }
    public string State { get; set; }
    public string Result { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime FinishedDate { get; set; }
    public string Url { get; set; }
    public Resources Resources { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
}

public class PipelineLinks
{
    public WebLink Self { get; set; }
    public WebLink Web { get; set; }
    public WebLink Pipelineweb { get; set; }
    public Pipeline Pipeline { get; set; }
}




public class Pipeline1
{
    public string Url { get; set; }
    public int Id { get; set; }
    public int Revision { get; set; }
    public string Name { get; set; }
    public string Folder { get; set; }
}

public class Resources
{
    public Repositories Repositories { get; set; }
}

public class Repositories
{
    public RepositorySelf Self { get; set; }
}