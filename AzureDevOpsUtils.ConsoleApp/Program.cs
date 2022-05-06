using System.CommandLine;

namespace AzureDevOpsUtils.ConsoleApp;
internal class Program
{
    public static int Main(string[] args)
    {
        var organisationOption = new Option<string>("--org", "The organisation to connect to") {IsRequired = true};
        organisationOption.AddAlias("-o");

        var projectOption = new Option<string>("--proj", "The project to connect to") {IsRequired = true};
        projectOption.AddAlias("-p");

        var personalAccessTokenOption =
            new Option<string>("--pat", "The personal access token to connect to DevOps with")
                {IsRequired = true};

        var pipelineIdOption = new Option<int>("--pipeline", "The id of the pipeline to check on") { IsRequired = true };
        var pipelineRunIdOption = new Option<int>("--pipelinerun", "The id of the pipeline run to check on") { IsRequired = true };

        var outputFileOption = new Option<string>("--out", "The output file") {IsRequired = true};

        #region Pipeline commands
        var pipelineCommand = new Command("pipeline");

        var pipelineGenerateCommand = new Command("generate")
        {
            organisationOption,
            projectOption,
            personalAccessTokenOption,
            outputFileOption
        };

        pipelineCommand.AddCommand(pipelineGenerateCommand);
        #endregion

        #region Accelerate Metric commands

        var accelerateCommand = new Command("accelerate");
        var accelerateTimeFromCommitCommand = new Command("timefromcommit")
        {
            organisationOption,
            projectOption,
            personalAccessTokenOption,
            pipelineIdOption,
            pipelineRunIdOption
        };
        accelerateCommand.AddCommand(accelerateTimeFromCommitCommand);

        #endregion
        var rootCommand = new RootCommand
        {

            pipelineCommand,
            accelerateCommand,

        };

        pipelineGenerateCommand.SetHandler(
            (string organisation, string project, string pat, string outputFileName) =>
            {
                HandlePipelineGenerate(organisation, project, pat, outputFileName);
            },
            organisationOption, projectOption, personalAccessTokenOption, outputFileOption);

        accelerateTimeFromCommitCommand.SetHandler(
            (string organisation, string project, string pat, int pipelineId, int pipelineRunId) =>
            {
                HandleAccelerateTimeFromCommit(organisation, project, pat, pipelineId, pipelineRunId);
            },
            organisationOption, projectOption, personalAccessTokenOption, pipelineIdOption, pipelineRunIdOption);
        return rootCommand.Invoke(args);
    }

    static void HandlePipelineGenerate(string org, string proj, string pat, string outputFileName)
    {
        new PipelineCommandClass(org, proj, pat).GeneratePipelineStatus(outputFileName);
    }

    static void HandleAccelerateTimeFromCommit(string org, string proj, string pat, int pipelineId, int pipelineRunId)
    {
        new AccelerateMetricCommandClass(org, proj, pat).GetTimeToDeployForPipelineRun(pipelineId, pipelineRunId);
    }
}