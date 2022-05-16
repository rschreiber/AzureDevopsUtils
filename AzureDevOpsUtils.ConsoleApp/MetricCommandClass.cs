using Microsoft.ApplicationInsights;

namespace AzureDevOpsUtils.ConsoleApp;

internal class MetricCommandClass
{
    //This is a terrible solution to solve the problem of recurring testing. Not sure if it should be formalised, burnt, or calculated.
    //Will look at it again in the next iteration
    const string VersionString = "14";

    private readonly TelemetryClient _telemetryClient;


    public MetricCommandClass(TelemetryClient telemetryClient)
    {
        _telemetryClient = telemetryClient;
    }
    public void GetLeadTimeMetricForPipelineRun(AzureDevOpsConfig config, int? pipelineId, int? runId, bool verbose)
    {

        var adf = new AzureDevOpsFacade(config, verbose);
        //Get the information about the pipeline run
        var pipelineRunInfo = adf.GetPipelineRunInformation(pipelineId!.Value, runId!.Value);
        
        //Set an initial 'finish' date to the start of the run date for the pipeline (because this tool
        // might be run as part of the pipeline run
        var pipelineDateTime = pipelineRunInfo.CreatedDate;
        
        //And in the case of a finish date being there, use it
        if (pipelineRunInfo.FinishedDate != DateTime.MinValue)
        {
            pipelineDateTime = pipelineRunInfo.FinishedDate;
        }
        //Get the commit id the kicked off this pipeline run, and try to work back up to find the pull request
        var commitId = pipelineRunInfo.Resources.Repositories.Self.version;
        var repositoryId = pipelineRunInfo.Resources.Repositories.Self.repository.Id;
        var pullRequestIds = adf.GetPullRequestIdArrayForCommit(new Guid(repositoryId),
            commitId);
        //Exclude if this isn't linked to any pull requests
        if (!pullRequestIds.Any())
            return;
        //Exclude if this isn't into 'master' branch. Will need to look at parameterising this later.
        if (pipelineRunInfo.Resources.Repositories.Self.refName != "refs/heads/master")
            return;

        var pullRequestInfo = adf.GetPullRequestInfo(pullRequestIds[0]);
        var prCreatedDateTime = pullRequestInfo.CreationDate;
        var timeTaken = pipelineDateTime - prCreatedDateTime;

        var customProperties = new Dictionary<string, string>()
        {
            {"PipelineRunName", pipelineRunInfo.Name},
            {$"Pipeline Id", $"{pipelineId}"},
            {$"Pipeline Run Id", $"{runId}"},
            {"Completed", $"{pipelineDateTime:O}"},
            {"Commit", $"{commitId}"},
            {"PR Created", $"{prCreatedDateTime:O}"},
            {"ElapsedTime", $"{timeTaken.TotalSeconds}"},
            {"Version", VersionString}
        };
        //Send it to Application Insights
        _telemetryClient.TrackEvent($"LeadTime-{pipelineId}", customProperties);
        _telemetryClient.Flush();

        //And send it to the console so that there's something to read
        foreach (var customProperty in customProperties)
        {
            Console.Write(customProperty.Key.PadRight(15));
            Console.Write($": {customProperty.Value}");
        }
        Console.Write(Environment.NewLine);
    }

    public void GetTestMetricsForPipelineRun(AzureDevOpsConfig config, int? pipelineId, int? runId, bool verbose)
    {
        var adf = new AzureDevOpsFacade( config, verbose);
        //Get the information about the pipeline run
        var pipelineRunInfo = adf.GetPipelineRunInformation(pipelineId!.Value, runId!.Value);
        var pipelineDateTime = pipelineRunInfo.CreatedDate;
        //Get the test results for the pipeline
        var testResults = adf.GetTestResultsByPipelineRunId(runId);

        //Group the data based on the test result summary
        var testSummary = testResults.TestResults.GroupBy(tr => tr.Outcome)
            .Select(gr => new { Metric = gr.Key, Count = gr.Count() });

        var customProperties = new Dictionary<string, string>()
        {
            {"PipelineRunName", pipelineRunInfo.Name},
            {$"Pipeline Id", $"{pipelineId}"},
            {$"Pipeline Run Id", $"{runId}"},
            {$"Pipeline Run Start", $"{pipelineDateTime:O}"},
            {$"Total Tests", $"{testResults.Count}"},
            {"Version", VersionString}
        };

        foreach (var summary in testSummary)
        {
            customProperties.Add(summary.Metric, summary.Count.ToString());
        }

        _telemetryClient.TrackEvent($"TestSummary-{pipelineId}", customProperties);
        _telemetryClient.Flush();

        //TODO: RS - Should we be logging information about individual binaries
        /*

        //Group the data based on the binary (dll) that the test exists in
        var testArea = testResults.TestResults.GroupBy(tr => tr.AutomatedTestStorage)
            .Select(gr => new { Binary = gr.Key, Count = gr.Count() });

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
        */
        Console.Write(Environment.NewLine);
    }

    public void RegenerateAllMetricsForAllPipelineRuns(AzureDevOpsConfig config, int? pipelineId, bool verbose)
    {
        var adf = new AzureDevOpsFacade(config, verbose);
        var pipelineRuns = adf.GetPipelineRunHistory(pipelineId!.Value);
        foreach (var pipelineRun in pipelineRuns.PipelineRuns.OrderBy(pr => pr.Id))
        {
            GetTestMetricsForPipelineRun(config, pipelineId, pipelineRun.Id, verbose);
            GetLeadTimeMetricForPipelineRun(config, pipelineId, pipelineRun.Id, verbose);
            GetStepsMetricsForPipelineRun(config, pipelineId, pipelineRun.Id, verbose);
        }
    }

    public void GetStepsMetricsForPipelineRun(AzureDevOpsConfig config, int? pipelineId, int? runId, bool verbose)
    {

        var adf = new AzureDevOpsFacade(config, verbose);
        var pipelineRunInfo = adf.GetPipelineRunInformation(pipelineId!.Value, runId!.Value);
        var timeline = adf.GetTimeLineByPipelineRunId(runId);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (timeline.Records == null || timeline.Records.Length==0)
        {
             return;
        }

        var recordsToProcess = timeline.Records.Where(r => r.Type == "Job").OrderBy(r => r.Order).ToArray();
        //Track the whole pipeline run
        var customPropertiesForPipeline = new Dictionary<string, string>
        {
            {$"Pipeline Run Name", $"{pipelineRunInfo.Name}"},
            {$"Pipeline Id", $"{pipelineId}"},
            {$"Pipeline Run Id", $"{runId}"},
            {$"Current Step", "0"},
            {$"Total Steps", $"{recordsToProcess.Length}"},
            {"RecordType", "PipelineRun"},
            {"Name", "Pipeline"},
            {"Version", VersionString}
        };





        var minDateTime = timeline.Records.MinBy(r => r.StartTime)?.StartTime;
        var maxDateTime = timeline.Records.MaxBy(r => r.FinishTime)?.FinishTime;
        if (minDateTime.HasValue)
            customPropertiesForPipeline.Add("StartTime", minDateTime.Value.ToString("O"));

        if (maxDateTime.HasValue)
            customPropertiesForPipeline.Add("FinishTime", maxDateTime.Value.ToString("O"));

        //
        if (minDateTime.HasValue && maxDateTime.HasValue)
        {
            var elapsedTime = recordsToProcess.Sum(r =>
                {
                    if (r.FinishTime.HasValue)
                    {
                        if (r.StartTime.HasValue)
                        {
                            return (r.FinishTime - r.StartTime).Value.TotalSeconds;
                        }
                    }

                    return 0.0;
                }
            );
            customPropertiesForPipeline.Add("ElapsedTime", elapsedTime.ToString());
        }

        var overAllResult = timeline.Records.All(r => r.Result != "failed");
        customPropertiesForPipeline.Add("Result", overAllResult ? "succeeded":"failed");

        customPropertiesForPipeline.Add("Branch", pipelineRunInfo.Resources.Repositories.Self.refName);

        _telemetryClient.TrackEvent($"PipelineStep-{pipelineId}-Summary", customPropertiesForPipeline);

        int stepNumber = 0;
        foreach (var record in recordsToProcess)
        {
            stepNumber++;
            var customProperties = new Dictionary<string, string>
            {
                {$"Pipeline Run Name", $"{pipelineRunInfo.Name}"},
                {$"Pipeline Id", $"{pipelineId}"},
                {$"Pipeline Run Id", $"{runId}"},
                {$"Current Step", $"{stepNumber}"},
                {$"Total Steps", $"{recordsToProcess.Length}"},
                {"RecordType", record.Type},
                {"Name", record.Name},
                {"Version", VersionString}
            };

            if (record.StartTime.HasValue && record.FinishTime.HasValue)
            {
                customProperties.Add("ElapsedTime", (record.FinishTime.Value - record.StartTime.Value).TotalSeconds.ToString() );

            }
            if (record.StartTime.HasValue)
            {
                customProperties.Add("StartTime", record.StartTime.Value.ToString("O"));
            }
            if (record.FinishTime.HasValue)
            {
                customProperties.Add("FinishTime", record.FinishTime.Value.ToString("O"));
            }
            if (!string.IsNullOrWhiteSpace(record.Result))
            {
                customProperties.Add("Result", record.Result);
            }
            _telemetryClient.TrackEvent($"PipelineStep-{pipelineId}-{record.Name}", customProperties);
        }

        _telemetryClient.Flush();
    }
}