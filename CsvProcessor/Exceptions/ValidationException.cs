namespace CsvProcessor.Exceprtons;

public class ValidationException(string msg) : Exception
{
    public string Msg { get; } = msg;
}