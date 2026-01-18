namespace CsvProcessor.Models.DbModels;

public partial class ValueDto
{    
    public DateTime Date { get; set; }

    public double ExecutionTime { get; set; }

    public double PointerValue { get; set; }

    public int? FileId { get; set; }
}
