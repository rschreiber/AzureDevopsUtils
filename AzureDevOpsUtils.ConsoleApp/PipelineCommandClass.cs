namespace AzureDevOpsUtils.ConsoleApp;

internal class PipelineCommandClass
{
    private readonly string _organization;
    private readonly string _project;
    private readonly string _pat;

    public PipelineCommandClass(string organization, string project, string pat)
    {
        _organization = organization;
        _project = project;
        _pat = pat;
    }

    public void GeneratePipelineStatus(string outFileName)
    {

        var adf = new AzureDevOpsFacade(_organization, _project, _pat);
        var pipelines = adf.GetAllPipelines();
        List<string> fileLines = new List<string>
        {
            "|Build|Pipeline Status|SonarCloud Status|",
            "|--|--|--|"
        };
        foreach (var pipeline in pipelines)
        {
            Console.Out.WriteLine($"Getting pipeline information for [{pipeline.Id}] {pipeline.Name}...");
            var buildDefinition = adf.GetBuildDefinition(pipeline.Id);

            fileLines.Add($"|{buildDefinition.name}|[![Build Status]({buildDefinition._links.badge.href})]({buildDefinition._links.badge.href})|--|");
        }
        Console.Out.WriteLine($"Writing output to {outFileName}...");
        File.WriteAllLines(outFileName, fileLines);
        Console.Out.WriteLine("Complete.");
    }
}