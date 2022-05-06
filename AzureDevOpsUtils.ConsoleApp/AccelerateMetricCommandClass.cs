namespace AzureDevOpsUtils.ConsoleApp;

internal class AccelerateMetricCommandClass
{
    private readonly string _organization;
    private readonly string _project;
    private readonly string _pat;

    public AccelerateMetricCommandClass(string organization, string project, string pat)
    {
        _organization = organization;
        _project = project;
        _pat = pat;
    }


    //TODO: RS - Decided where to write the output
    public TimeSpan GetTimeToDeployForPipelineRun(int? pipelineId, int? runId)
    {

        var adf = new AzureDevOpsFacade(_organization, _project, _pat);
        var pipelineRunInfo = adf.GetPipelineRunInformation(pipelineId.Value, runId.Value);
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
        return timeTaken;
    }
} 