namespace AzureDevOpsUtils.ConsoleApp.Models;

public class  Record
{
    public object[] PreviousAttempts { get; set; }
    public string Id { get; set; }
    public string ParentId { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? FinishTime { get; set; }
    public object CurrentOperation { get; set; }
    public int? PercentComplete { get; set; }
    public string State { get; set; }
    public string Result { get; set; }
    public string ResultCode { get; set; }
    public int ChangeId { get; set; }
    public DateTime LastModified { get; set; }
    public string WorkerName { get; set; }
    public object Details { get; set; }
    public int ErrorCount { get; set; }
    public int WarningCount { get; set; }
    public object Url { get; set; }
    public Log Log { get; set; }
    public Task Task { get; set; }
    public int Attempt { get; set; }
    public string Identifier { get; set; }
    public int Order { get; set; }
    public int QueueId { get; set; }
    public Issue[] Issues { get; set; }
}