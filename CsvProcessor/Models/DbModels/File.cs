namespace CsvProcessor.Models.DbModels;

public partial class File
{
    public int Id { get; set; }

    public string Filename { get; set; } = null!;

    public DateTime? UploadTime { get; set; }

    public ProcessingStatus? ProcessingStatus { get; set; }
}

public enum ProcessingStatus : byte
{
    Success,
    Failure,
    Processing
}
