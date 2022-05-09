using System.CommandLine;

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

        var organisationOption = new Option<string>("--org", "The organisation to connect to") {IsRequired = true};
        organisationOption.AddAlias("-o");

        var projectOption = new Option<string>("--proj", "The project to connect to") {IsRequired = true};
        projectOption.AddAlias("-p");

        var personalAccessTokenOption =
            new Option<string>("--pat", "The personal access token to connect to DevOps with")
                {IsRequired = true};

        var applicationInsightsKey = new Option<int>("--aikey", "The Application Insights key that telemetry is sent to") { IsRequired = true };

        var pipelineIdOption = new Option<int>("--pipeline", "The id of the pipeline to check on") { IsRequired = true };
        var pipelineRunIdOption = new Option<int>("--pipelinerun", "The id of the pipeline run to check on") { IsRequired = true };

        var outputFileOption = new Option<string>("--out", "The output file") {IsRequired = true};

        var verboseOption = new Option<bool>("--verbose", "Enable verbose output") { IsRequired = false };
        #region Pipeline commands
        var pipelineCommand = new Command("pipeline");

        var pipelineGenerateCommand = new Command("generate")
        {
            organisationOption,
            projectOption,
            personalAccessTokenOption,
            outputFileOption,
            verboseOption
        };

        pipelineCommand.AddCommand(pipelineGenerateCommand);
        #endregion

        #region Metric commands

        var metricCommand= new Command("metric");
        var metricTimeFromCommitCommand = new Command("lead-time")
        {
            organisationOption,
            projectOption,
            personalAccessTokenOption,
            pipelineIdOption,
            pipelineRunIdOption,
            verboseOption
        };

        var metricTestCommand = new Command("test-summary")
        {
            organisationOption,
            projectOption,
            personalAccessTokenOption,
            pipelineIdOption,
            pipelineRunIdOption,
            verboseOption
        };

        metricCommand.AddCommand(metricTimeFromCommitCommand);
        metricCommand.AddCommand(metricTestCommand);

        #endregion
        var rootCommand = new RootCommand ("Azure DevOps Utils")
        {

            pipelineCommand,
            metricCommand
        };


        pipelineGenerateCommand.SetHandler(
            (string organisation, string project, string pat, string outputFileName, bool verbose) =>
            {
                HandlePipelineGenerate(organisation, project, pat, outputFileName, verbose);
            },
            organisationOption, projectOption, personalAccessTokenOption, outputFileOption, verboseOption);

        metricTimeFromCommitCommand.SetHandler(
            (string organisation, string project, string pat, int pipelineId, int pipelineRunId, bool verbose) =>
            {
                HandleAccelerateTimeFromCommit(organisation, project, pat, pipelineId, pipelineRunId, verbose);
            },
            organisationOption, projectOption, personalAccessTokenOption, pipelineIdOption, pipelineRunIdOption, verboseOption);

        metricTestCommand.SetHandler(
            (string organisation, string project, string pat, int pipelineId, int pipelineRunId, bool verbose) =>
            {
                HandleTestMetrics(organisation, project, pat, pipelineId, pipelineRunId, verbose);
            },
            organisationOption, projectOption, personalAccessTokenOption, pipelineIdOption, pipelineRunIdOption, verboseOption);
        return rootCommand.Invoke(args);
    }

    static void HandlePipelineGenerate(string org, string proj, string pat, string outputFileName, bool verbose)
    {
        new PipelineCommandClass(org, proj, pat, verbose).GeneratePipelineStatus(outputFileName);
    }

    static void HandleAccelerateTimeFromCommit(string org, string proj, string pat, int pipelineId, int pipelineRunId, bool verbose)
    {
        new MetricCommandClass(org, proj, pat, verbose).GetLeadTimeMetricForPipelineRun(pipelineId, pipelineRunId);
    }

    static void HandleTestMetrics(string org, string proj, string pat, int pipelineId, int pipelineRunId, bool verbose)
    {
        new MetricCommandClass(org, proj, pat, verbose).GetTestMetricsForPipelineRun(pipelineId, pipelineRunId);
    }
}