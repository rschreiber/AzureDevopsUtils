﻿using System.CommandLine;

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

        var outputFileOption = new Option<string>("--out", "The output file") {IsRequired = true};


        var pipelineCommand = new Command("pipeline");

        var pipelineGenerateCommand = new Command("generate")
        {

            organisationOption,
            projectOption,
            personalAccessTokenOption,
            outputFileOption
        };

        pipelineCommand.AddCommand(pipelineGenerateCommand);

        var rootCommand = new RootCommand
        {
            pipelineCommand
        };

        pipelineGenerateCommand.SetHandler(
            (string organisation, string project, string pat, string outputFileName) =>
            {
                HandlePipelineGenerate(organisation, project, pat, outputFileName);
            },
            organisationOption, projectOption, personalAccessTokenOption, outputFileOption);

        return rootCommand.Invoke(args);
    }

    static void HandlePipelineGenerate(string org, string proj, string pat, string outputFileName)
    {
        new PipelineCommandClass(org, proj, pat).GeneratePipelineStatus(outputFileName);
    }
}