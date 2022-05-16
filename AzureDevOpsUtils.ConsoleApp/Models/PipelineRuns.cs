using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureDevOpsUtils.ConsoleApp.Models
{

    public class PipelineRunHistory
    {
        public int Count { get; set; }
        [JsonPropertyName("value")]
        public PipelineRun[] PipelineRuns { get; set; }
    }

    public class PipelineRun
    {
        public Links Links { get; set; }
        public PipelineInfo Pipeline { get; set; }
        public string State { get; set; }
        public string Result { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime FinishedDate { get; set; }
        public string Url { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public Variables Variables { get; set; }
    }


    public class PipelineInfo
    {
        public string Url { get; set; }
        public int Id { get; set; }
        public int Revision { get; set; }
        public string Name { get; set; }
        public string Folder { get; set; }
    }

    public class Variables
    {
        public SystemPullRequestPullRequestId SystemPullRequestPullRequestId { get; set; }
        public SystemPullRequestSourceBranch SystemPullRequestSourceBranch { get; set; }
        public SystemPullRequestTargetBranch SystemPullRequestTargetBranch { get; set; }
        public SystemPullRequestSourceCommitId SystemPullRequestSourceCommitId { get; set; }
        public SystemPullRequestSourceRepositoryUri SystemPullRequestSourceRepositoryUri { get; set; }
        public SystemPullRequestPullRequestIteration SystemPullRequestPullRequestIteration { get; set; }
    }

    public class SystemPullRequestPullRequestId
    {
        public string Value { get; set; }
    }

    public class SystemPullRequestSourceBranch
    {
        public string Value { get; set; }
    }

    public class SystemPullRequestTargetBranch
    {
        public string Value { get; set; }
    }

    public class SystemPullRequestSourceCommitId
    {
        public string Value { get; set; }
    }

    public class SystemPullRequestSourceRepositoryUri
    {
        public string Value { get; set; }
    }

    public class SystemPullRequestPullRequestIteration
    {
        public string Value { get; set; }
    }

}
