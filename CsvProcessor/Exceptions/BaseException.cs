using System;

namespace CsvProcessor.Exceptions;

public class BaseMsgException(string msg) : Exception
{
    public string Msg { get; } = msg;
}
