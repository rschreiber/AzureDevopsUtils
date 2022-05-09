using System.Text.Json.Serialization;

namespace AzureDevOpsUtils.ConsoleApp.Models
{

    public class TestResultsByBuild
    {
        [JsonPropertyName("value")]
        public TestResult[] TestResults { get; set; }
        public int Count { get; set; }
    }

    public class TestResult
    {
        public int Id { get; set; }
        public int RunId { get; set; }
        public int RefId { get; set; }
        public string Outcome { get; set; }
        public int Priority { get; set; }
        public string AutomatedTestName { get; set; }
        public string AutomatedTestStorage { get; set; }
        public string Owner { get; set; }
        public string TestCaseTitle { get; set; }
        public float DurationInMs { get; set; }
    }

}
