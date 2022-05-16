using System.CommandLine;
using System.Diagnostics;
using System.Net.Mime;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AzureDevOpsUtils.ConsoleApp;
internal class Program
{
    public static int Main(string[] args)
    {

        if (args.Length == 0)
        {
            Console.Out.WriteLine($"Azure DevOps Utils (c) 2022{Environment.NewLine}");
        }
        else
        {
            Console.Write(Environment.NewLine);
        }

        var organisationOption = new Option<string>("--org", "The organisation to connect to") { IsRequired = true };
        organisationOption.AddAlias("-o");

        var projectOption = new Option<string>("--proj", "The project to connect to") { IsRequired = true };
        projectOption.AddAlias("-p");

        var personalAccessTokenOption =
            new Option<string>("--pat", "The personal access token to connect to DevOps with")
            { IsRequired = true };

        var applicationInsightsKeyOption = new Option<string>("--ai-key", "The Application Insights key that telemetry is sent to") { IsRequired = false };

        var pipelineIdOption = new Option<int>("--pipeline", "The id of the pipeline to check on") { IsRequired = true };
        var pipelineRunIdOption = new Option<int>("--pipelinerun", "The id of the pipeline run to check on") { IsRequired = true };

        var outputFileOption = new Option<string>("--out", "The output file") { IsRequired = true };

        var verboseOption = new Option<bool>("--verbose", "Enable verbose output") { IsRequired = false };
        #region Pipeline commands
        var pipelineCommand = new Command("pipeline");

        var pipelineGenerateCommand = new Command("generate")
        {
            outputFileOption,
            verboseOption
        };

        pipelineCommand.AddCommand(pipelineGenerateCommand);
        #endregion

        #region Metric commands

        var metricCommand = new Command("metric");
        var metricTimeFromCommitCommand = new Command("lead-time")
        {
            pipelineIdOption,
            pipelineRunIdOption,
            verboseOption
        };

        var metricTestCommand = new Command("test-summary")
        {
            pipelineIdOption,
            pipelineRunIdOption,
            verboseOption
        };

        var metricStepsCommand = new Command("build-steps")
        {
            pipelineIdOption,
            pipelineRunIdOption,
            verboseOption
        };

        var metricRegenerateAllCommand = new Command("regenerate-all")
        {
            pipelineIdOption,
            verboseOption
        };
        metricCommand.AddCommand(metricTimeFromCommitCommand);
        metricCommand.AddCommand(metricTestCommand);
        metricCommand.AddCommand(metricStepsCommand);
        metricCommand.AddCommand(metricRegenerateAllCommand);
        #endregion
        var rootCommand = new RootCommand("Azure DevOps Utils")
        {

            pipelineCommand,
            metricCommand
        };

        rootCommand.AddGlobalOption(organisationOption);
        rootCommand.AddGlobalOption(projectOption);
        rootCommand.AddGlobalOption(personalAccessTokenOption);
        rootCommand.AddGlobalOption(applicationInsightsKeyOption);

        pipelineGenerateCommand.SetHandler(
            (string organisation, string project, string pat, string outputFileName, bool verbose, string aiKey) =>
            {
                HandlePipelineGenerate(organisation, project, pat, outputFileName, verbose, aiKey);
            },
            organisationOption, projectOption, personalAccessTokenOption, outputFileOption, verboseOption, applicationInsightsKeyOption);

        metricTimeFromCommitCommand.SetHandler(
            (string organisation, string project, string pat, int pipelineId, int pipelineRunId, bool verbose,
                string aiKey) =>
            {
                HandleAccelerateTimeFromCommit(organisation, project, pat, pipelineId, pipelineRunId, verbose, aiKey);
            },
            organisationOption, projectOption, personalAccessTokenOption, pipelineIdOption, pipelineRunIdOption,
            verboseOption, applicationInsightsKeyOption);

        metricTestCommand.SetHandler(
            (string organisation, string project, string pat, int pipelineId, int pipelineRunId, bool verbose,
                string aiKey) =>
            {
                HandleTestMetrics(organisation, project, pat, pipelineId, pipelineRunId, verbose, aiKey);
            },
            organisationOption, projectOption, personalAccessTokenOption, pipelineIdOption, pipelineRunIdOption,
            verboseOption, applicationInsightsKeyOption);

        metricStepsCommand.SetHandler(
            (string organisation, string project, string pat, int pipelineId, int pipelineRunId, bool verbose,
                string aiKey) =>
            {
                HandleStepsMetrics(organisation, project, pat, pipelineId, pipelineRunId, verbose, aiKey);
            },
            organisationOption, projectOption, personalAccessTokenOption, pipelineIdOption, pipelineRunIdOption,
            verboseOption, applicationInsightsKeyOption);

        metricStepsCommand.SetHandler(
            (string organisation, string project, string pat, int pipelineId, bool verbose,
                string aiKey) =>
            {
                HandleRegenerateAllMetrics(organisation, project, pat, pipelineId, verbose, aiKey);
            },
            organisationOption, projectOption, personalAccessTokenOption, pipelineIdOption,
            verboseOption, applicationInsightsKeyOption);

        var result = rootCommand.Invoke(args);
        Thread.Sleep(5000);
        return result;
    }

    private static void HandlePipelineGenerate(string org, string proj, string pat, string outputFileName, bool verbose, string aiKey)
    {
        var pipelineCommandClass = new PipelineCommandClass(CreateTelemetryClient(aiKey));
        pipelineCommandClass.GeneratePipelineStatus(
            new AzureDevOpsConfig {Organisation = org, Project = proj, PersonalAccessToken = pat}, outputFileName);
    }

    private static void HandleAccelerateTimeFromCommit(string org, string proj, string pat, int pipelineId, int pipelineRunId, bool verbose, string aiKey)
    {
        var metricCommandClass = new MetricCommandClass(CreateTelemetryClient(aiKey));
        metricCommandClass.GetLeadTimeMetricForPipelineRun(
            new AzureDevOpsConfig { Organisation = org, Project = proj, PersonalAccessToken = pat }, pipelineId,
            pipelineRunId, verbose);
    }

    private static void HandleTestMetrics(string org, string proj, string pat, int pipelineId, int pipelineRunId, bool verbose, string aiKey)
    {
        var metricCommandClass = new MetricCommandClass(CreateTelemetryClient(aiKey));
        metricCommandClass.GetTestMetricsForPipelineRun(
           new AzureDevOpsConfig { Organisation = org, Project = proj, PersonalAccessToken = pat }, pipelineId,
           pipelineRunId, verbose);
    }

    private static void HandleStepsMetrics(string org, string proj, string pat, int pipelineId, int pipelineRunId, bool verbose, string aiKey)
    {
        var metricCommandClass = new MetricCommandClass(CreateTelemetryClient(aiKey)); 
        metricCommandClass.GetStepsMetricsForPipelineRun(
            new AzureDevOpsConfig { Organisation = org, Project = proj, PersonalAccessToken = pat }, pipelineId,
            pipelineRunId, verbose);
    }

    private static void HandleRegenerateAllMetrics(string org, string proj, string pat, int pipelineId, bool verbose,
        string aiKey)
    {
        var metricCommandClass = new MetricCommandClass(CreateTelemetryClient(aiKey));
        metricCommandClass.RegenerateAllMetricsForAllPipelineRuns(
            new AzureDevOpsConfig {Organisation = org, Project = proj, PersonalAccessToken = pat}, pipelineId,
            verbose);
    }

    private static TelemetryClient CreateTelemetryClient(string aiKey)
    {
        TelemetryConfiguration telemetryConfiguration = TelemetryConfiguration.CreateDefault();
        telemetryConfiguration.InstrumentationKey = aiKey;
        return new TelemetryClient(telemetryConfiguration);
    }
}