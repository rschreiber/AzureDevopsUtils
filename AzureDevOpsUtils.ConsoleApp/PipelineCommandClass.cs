using Microsoft.ApplicationInsights;

namespace AzureDevOpsUtils.ConsoleApp;

internal class PipelineCommandClass
{

    private readonly TelemetryClient _telemetryClient;


    public PipelineCommandClass(TelemetryClient telemetryClient)
    {
        _telemetryClient = telemetryClient;
    }


    public void GeneratePipelineStatus(AzureDevOpsConfig config, string outFileName)
    {
        var adf = new AzureDevOpsFacade(config, false);
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

            fileLines.Add($"|[{buildDefinition.Name}]({buildDefinition.Links.Web.HRef})|[![Build Status]({buildDefinition.Links.Badge.HRef})]({buildDefinition.Links.Badge.HRef})|--|");
        }
        Console.Out.WriteLine($"Writing output to {outFileName}...");
        File.WriteAllLines(outFileName, fileLines);
        Console.Out.WriteLine("Complete.");
    }

}