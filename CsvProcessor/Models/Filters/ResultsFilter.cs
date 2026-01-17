using System;

namespace CsvProcessor.Models.Filters;

public class ResultFilter
{
    public string Filename { get; set; } = null!;
    public DateTime? StartTimeFrom { get; set; }
    public DateTime? StartTimeTo { get; set; }
    public double? AvgValueMin { get; set; }
    public double? AvgValueMax { get; set; }
    public double? AvgExecTimeMin { get; set; }
    public double? AvgExecTimeMax { get; set; }
}