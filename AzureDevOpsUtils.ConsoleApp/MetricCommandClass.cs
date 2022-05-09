namespace AzureDevOpsUtils.ConsoleApp;

internal class MetricCommandClass
{
    private readonly string _organization;
    private readonly string _project;
    private readonly string _pat;
    private readonly bool _verbose;
    public MetricCommandClass(string organization, string project, string pat, bool verbose)
    {
        _organization = organization;
        _project = project;
        _pat = pat;
        _verbose = verbose;
    }


    //TODO: RS - Decided where to write the output
    public void GetLeadTimeMetricForPipelineRun(int? pipelineId, int? runId)
    {

        var adf = new AzureDevOpsFacade(_organization, _project, _pat, _verbose);
        var pipelineRunInfo = adf.GetPipelineRunInformation(pipelineId!.Value, runId!.Value);
        var pipelineDateTime = pipelineRunInfo.CreatedDate;
        if (pipelineRunInfo.FinishedDate != DateTime.MinValue)
        {
            pipelineDateTime = pipelineRunInfo.FinishedDate;
        }

        var commitId = pipelineRunInfo.Resources.Repositories.Self.version;
        var repositoryId = pipelineRunInfo.Resources.Repositories.Self.repository.Id;
        var pullRequestId = adf.GetPullRequestIdArrayForCommit(new Guid(repositoryId),
            commitId);
        var pullRequestInfo = adf.GetPullRequestInfo(pullRequestId[0]);
        var prCreatedDateTime = pullRequestInfo.CreationDate;
        var timeTaken = pipelineDateTime - prCreatedDateTime;
        Console.WriteLine($"Pipeline Run Name  : {pipelineRunInfo.Name}");
        Console.WriteLine($"Completed          : {pipelineDateTime.ToLocalTime():yyyy/MM/dd HH:mm}");
        Console.WriteLine($"Commit             : {commitId}");
        Console.WriteLine($"PR Created         : {prCreatedDateTime.ToLocalTime():yyyy/MM/dd HH:mm}");
        Console.WriteLine($"Elapsed            : {timeTaken}");
        Console.Write(Environment.NewLine);
    }

    public void GetTestMetricsForPipelineRun(int? pipelineId, int? runId)
    {
        var adf = new AzureDevOpsFacade(_organization, _project, _pat, _verbose);
        var pipelineRunInfo = adf.GetPipelineRunInformation(pipelineId!.Value, runId!.Value);
        var pipelineDateTime = pipelineRunInfo.CreatedDate;
        var testResults = adf.GetTestResultsByPipelineRunId(runId);

        var testSummary = testResults.TestResults.GroupBy(tr => tr.Outcome)
            .Select(gr => new { Metric = gr.Key, Count = gr.Count() });

        var testArea = testResults.TestResults.GroupBy(tr => tr.AutomatedTestStorage)
            .Select(gr => new { Binary = gr.Key, Count = gr.Count() });

        Console.WriteLine($"Pipeline Run Name   : {pipelineRunInfo.Name}");
        Console.WriteLine($"Pipeline Run Start  : {pipelineDateTime.ToLocalTime():yyyy/MM/dd HH:mm}");
        Console.WriteLine($"Total Tests         : {testResults.Count}");

        foreach (var summary in testSummary)
        {
            Console.Write($"- {summary.Metric}".PadRight(20));
            Console.WriteLine($": {summary.Count}");
        }

        foreach (var area in testArea)
        {
            Console.WriteLine($"Tests By Binary     : {area.Binary}");
            var binaryTestSummary = testResults.TestResults.Where(tr => tr.AutomatedTestStorage == area.Binary).GroupBy(tr => tr.Outcome)
                .Select(gr => new { Metric = gr.Key, Count = gr.Count() });
            foreach (var summary in binaryTestSummary)
            {
                Console.Write($"- {summary.Metric}".PadRight(20));
                Console.WriteLine($": {summary.Count}");
            }
        }
        Console.Write(Environment.NewLine);
    }
}