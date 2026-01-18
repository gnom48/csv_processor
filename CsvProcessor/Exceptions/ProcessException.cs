using CsvProcessor.Exceptions;

namespace CsvProcessor.Exceprtons;

public class ProcessException : BaseMsgException
{
    public ProcessException(string msg = "Ошибка при работе с файлом") : base(msg)
    {
    }
}