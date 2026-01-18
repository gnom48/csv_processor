using CsvProcessor.Exceptions;

namespace CsvProcessor.Exceprtons;

public class ValidationException : BaseMsgException
{
    public ValidationException(string msg) : base(msg)
    {
    }
}