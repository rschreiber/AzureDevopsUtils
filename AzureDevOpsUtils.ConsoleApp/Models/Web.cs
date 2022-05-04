namespace AzureDevOpsUtils.ConsoleApp.Models;

public class Web
{
    public string Href { get; set; }
}



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
    public Self Self { get; set; }
    public Web Web { get; set; }
    public PipelineWebLink Pipelineweb { get; set; }
    public Pipeline Pipeline { get; set; }
}


public class PipelineWebLink
{
    public string Href { get; set; }
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
    public Self Self { get; set; }
}