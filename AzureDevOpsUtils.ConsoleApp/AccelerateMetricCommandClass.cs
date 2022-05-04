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

    //TODO: Parameterise
    public TimeSpan GetTimeToDeployForPipelineRun(int pipelineId, int runId)
    {
        var adf = new AzureDevOpsFacade(_organization, _project, _pat);
        var pipelineRunInfo = adf.GetPipelineRunInformation(pipelineId, runId);
        var pullRequestId = adf.GetPullRequestIdArrayForCommit(new Guid("c4ea4f5b-ce53-4b6a-87ae-32baf62d6d9b"),
            "9285b32264904ab79fafb3ea3d2365ae86396e6c");
        return new TimeSpan(1,1,1,1);
    }
}