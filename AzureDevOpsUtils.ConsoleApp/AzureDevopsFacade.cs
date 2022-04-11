using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AzureDevOpsUtils.ConsoleApp.Models;
using RestSharp;
using RestSharp.Authenticators;


namespace AzureDevOpsUtils.ConsoleApp
{
    internal class AzureDevOpsFacade
    {
        private readonly string _organization;
        private readonly string _project;
        private readonly string _pat;

        public AzureDevOpsFacade(string organization, string project, string pat)
        {
            _organization = organization;
            _project = project;
            _pat = pat;
        }

        private RestClient? _restClient;
        private RestClient? RestClient => _restClient ??= GetJsonClient(_organization, _project, _pat);

        public IEnumerable<Pipeline> GetAllPipelines()
        {

            var request = CreateRestRequest("pipelines", Method.Get);

            if (RestClient != null)
            {
                var response = RestClient.Execute<PipelinesRequest>(request);
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
            if (RestClient != null)
            {
                var response = RestClient.Execute<BuildDefinition>(request);
                if (response.IsSuccessful && response.Data != null)
                {
                    return response.Data;
                }
            }

            return new();
        }


        private RestClient? GetJsonClient(string organization, string project, string pat)
        {
            var rootUrl = $"https://dev.azure.com/{organization}/{project}/_apis/";
            Console.Out.Write($"Setting up HTTPS client to {rootUrl}...");

            var client = new RestClient(rootUrl);
            client.Authenticator = new HttpBasicAuthenticator(string.Empty, pat);
            Console.Out.WriteLine("Done.");
            return client;
        }

        private RestRequest CreateRestRequest(string path, RestSharp.Method method)
        {
            string apiPath = $"{path}";
            Console.Out.Write($"Creating request to {apiPath}...");
            //_Log.Info($"Creating new rest request with url {path} for method {method}");
            var request = new RestRequest(apiPath, method);
            Console.Out.WriteLine("Done.");
            return request;

        }
    }
}
