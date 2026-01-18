namespace CsvProcessor.Models.DbModels;

public partial class ResultDto
{
    public int? FileId { get; set; }

    public TimeSpan? DeltaSeconds { get; set; }

    public DateTime? StartTime { get; set; }

    public double? AverageExecutionTime { get; set; }

    public double? AverageValue { get; set; }

    public double? MedianValue { get; set; }

    public double? MaxValue { get; set; }

    public double? MinValue { get; set; }
}
