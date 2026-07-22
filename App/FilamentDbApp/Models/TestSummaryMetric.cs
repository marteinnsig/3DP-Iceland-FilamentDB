namespace FilamentDbApp.Models;

public sealed class TestSummaryMetric
{
    public required string TestType { get; init; }
    public required string MetricName { get; init; }
    public string? MetricValue { get; init; }
    public string? Unit { get; init; }
    public required string SourceSheet { get; init; }
    public required string SourceColumn { get; init; }
}
