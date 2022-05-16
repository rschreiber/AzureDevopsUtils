using System.Text.RegularExpressions;
using AzureDevOpsUtils.ConsoleApp.Models;
using RestSharp;
using RestSharp.Authenticators;


namespace AzureDevOpsUtils.ConsoleApp;

internal class AzureDevOpsFacade
{
    private readonly AzureDevOpsConfig _azureDevopsConfig;
    private readonly bool _verbose;
    public AzureDevOpsFacade(AzureDevOpsConfig config, bool verbose)
    {
        _azureDevopsConfig = config;
        _verbose = verbose;
    }

    private RestClient? _adoRestClient;
    private RestClient? _vsoRestClient;
    private RestClient? AzureDevopsRestClient => _adoRestClient ??= GetAzureDevopsJsonClient(_azureDevopsConfig);
    private RestClient? VSORestClient => _vsoRestClient ??= GetVSOJsonClient(_azureDevopsConfig);

    public IEnumerable<Pipeline> GetAllPipelines()
    {

        var request = CreateRestRequest("pipelines", Method.Get);

        if (AzureDevopsRestClient != null)
        {
            var response = AzureDevopsRestClient.Execute<PipelinesRequest>(request);
            if (response.IsSuccessful && response.Data != null)
            {
                return response.Data.Pipelines;
            }
        }

        return Enumerable.Empty<Pipeline>();
    }

    public BuildDefinition GetBuildDefinition(int definitionId)
    {
        //GET https://dev.azure.com/{organization}/{project}/_apis/build/definitions/{definitionId}?api-version=6.0

        var request = CreateRestRequest($"build/definitions/{definitionId}", Method.Get);
        if (AzureDevopsRestClient != null)
        {
            var response = AzureDevopsRestClient.Execute<BuildDefinition>(request);
            if (response.IsSuccessful && response.Data != null)
            {
                return response.Data;
            }
        }

        return new();
    }

    public PipelineRunHistory GetPipelineRunHistory(int pipelineId)
    {

        var request = CreateRestRequest($"pipelines/{pipelineId}/runs/?api-version=7.1-preview.1", Method.Get);
        if (AzureDevopsRestClient != null)
        {
            var response = AzureDevopsRestClient.Execute<PipelineRunHistory>(request);
            if (response.IsSuccessful && response.Data != null)
            {
                return response.Data;
            }
        }

        return new();
    }

    public PipelineRunInformationResponse GetPipelineRunInformation(int pipelineId, int runId)
    {

        var request = CreateRestRequest($"pipelines/{pipelineId}/runs/{runId}?api-version=7.1-preview.1", Method.Get);
        if (AzureDevopsRestClient != null)
        {
            var response = AzureDevopsRestClient.Execute<PipelineRunInformationResponse>(request);
            if (response.IsSuccessful && response.Data != null)
            {
                return response.Data;
            }
        }

        return new();

    }

    public int[] GetPullRequestIdArrayForCommit (Guid repositoryId, string commitId)
    {
        var request = CreateRestRequest($"git/repositories/{repositoryId}/pullrequestquery?api-version=7.1-preview.1", Method.Post);
        request.AddStringBody(@$"{{
	""queries"":[

        {{
            ""items"": [""{commitId}""],
            ""type"": ""lastMergeCommit""

        }}
        ]
    }}", DataFormat.Json);
        if (AzureDevopsRestClient != null)
        {
            var response = AzureDevopsRestClient.Execute(request);
            if (response.IsSuccessful && response.Content != null)
            {
                Regex pullRequestRegex = new Regex("\"pullRequestId\":\\s?(?<id>\\d{1,6})");
                var matches = pullRequestRegex.Matches(response.Content);
                return matches.Select(m => Convert.ToInt32(m.Groups[1].Value)).ToArray();
            }
        }
        return new[] {1};
    }


    public PullRequest GetPullRequestInfo(int pullRequestId)
    {

        var request = CreateRestRequest($"git/pullrequests/{pullRequestId}?api-version=7.1-preview.1", Method.Get);
        if (AzureDevopsRestClient != null)
        {
            var response = AzureDevopsRestClient.Execute<PullRequest>(request);
            if (response.IsSuccessful && response.Data != null)
            {
                return response.Data;
            }
        }

        return new();
    }


    public TestResultsByBuild GetTestResultsByPipelineRunId(int? runId)
    {
        var request = CreateRestRequest($"tcm/ResultsByBuild?buildId={runId}&publishContext=CI", Method.Get);
        if (VSORestClient != null)
        {
            var response = VSORestClient.Execute<TestResultsByBuild>(request);
            if (response.IsSuccessful && response.Data != null)
            {
                return response.Data;
            }
        }

        return new();
    }

    public void GetTestRunInfo(int? testRunId)
    {
        // https://dev.azure.com/henryscheinone/conversions/_apis/
        var request = CreateRestRequest($"test/runs/{testRunId}/Statistics?api-version=6.0", Method.Get);
        if (AzureDevopsRestClient != null)
        {
            var response = AzureDevopsRestClient.Execute(request);
            if (response.IsSuccessful && response.Content != null)
            {
                return;// response.Data;
            }
        }

        return;// new();
    }

    public PipelineTimeline GetTimeLineByPipelineRunId(int? runId)
    {
        var request = CreateRestRequest($"/build/builds/{runId}/timeline/?api-version=7.1-preview.2", Method.Get);
        if (AzureDevopsRestClient != null)
        {
            var response = AzureDevopsRestClient.Execute<PipelineTimeline>(request);
            if (response.IsSuccessful && response.Data != null)
            {
                return response.Data;
            }
        }

        return new();
    }

    private RestClient? GetVSOJsonClient(AzureDevOpsConfig config)
    {
        var rootUrl = $"https://{config.Organisation}.vstmr.visualstudio.com/{config.Project}/_apis/";
        return GetJsonClient(rootUrl, config.PersonalAccessToken);
    }

    private RestClient? GetAzureDevopsJsonClient(AzureDevOpsConfig config)
    {
        var rootUrl = $"https://dev.azure.com/{config.Organisation}/{config.Project}/_apis/";
        return GetJsonClient(rootUrl, config.PersonalAccessToken);
    }

    private RestClient? GetJsonClient(string rootUrl, string pat)
    {
        WriteConsoleMessageWithVerboseCheck($"Setting up HTTPS client to {rootUrl}...");
        var client = new RestClient(rootUrl);
        client.Authenticator = new HttpBasicAuthenticator(string.Empty, pat);
        WriteConsoleMessageWithVerboseCheck($"Setting up HTTPS client to {rootUrl} done.");
        return client;
    }

    private RestRequest CreateRestRequest(string path, RestSharp.Method method)
    {
        string apiPath = $"{path}";

        WriteConsoleMessageWithVerboseCheck($"Creating request to {apiPath}...");

        //_Log.Info($"Creating new rest request with url {path} for method {method}");
        var request = new RestRequest(apiPath, method);
        WriteConsoleMessageWithVerboseCheck($"Creating request to {apiPath} done.");
        return request;

    }


    private void WriteConsoleMessageWithVerboseCheck(string message)
    {
        if (_verbose)
        {
            Console.Out.WriteLine(message);
        }
    }
}