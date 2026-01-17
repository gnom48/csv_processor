using System.Globalization;
using CsvProcessor.Exceprtons;
using CsvProcessor.Models.DbModels;

namespace CsvProcessor.Services;

public static class Validations
{
    public static Value MapAndThrowIfInvalid(string[] rowParts)
    {
        if (rowParts.Length != 3)
            throw new ValidationException("Некорректный формат строки CSV.");

        var dateStr = rowParts[0];
        var execTimeStr = rowParts[1];
        var valueStr = rowParts[2];

        if (string.IsNullOrWhiteSpace(dateStr) ||
            string.IsNullOrWhiteSpace(execTimeStr) ||
            string.IsNullOrWhiteSpace(valueStr))
            throw new ValidationException("Отсутствуют обязательные поля в строке.");

        if (!DateTime.TryParseExact(dateStr, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var parsedDate))
            throw new ValidationException("Неверный формат даты.");

        var nowUtc = DateTime.UtcNow;
        if (parsedDate < new DateTime(2000, 1, 1) || parsedDate > nowUtc)
            throw new ValidationException("Недопустимое значение даты.");

        if (!double.TryParse(execTimeStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double execTime))
            throw new ValidationException("Неверный формат времени выполнения.");

        if (execTime < 0)
            throw new ValidationException("Время выполнения не может быть отрицательным.");

        if (!double.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            throw new ValidationException("Неверный формат значения показателя.");

        if (value < 0)
            throw new ValidationException("Значение показателя не может быть отрицательным.");

        return new Value
        {
            Date = parsedDate,
            ExecutionTime = execTime,
            PointerValue = value,
        };
    }

    public static double CalculateMedian(IEnumerable<double> numbers)
    {
        var sortedNumbers = numbers.OrderBy(n => n).ToArray();
        int count = sortedNumbers.Length;
        if (count == 0) return 0;

        if (count % 2 == 1)
            return sortedNumbers[count / 2];
        else
            return (sortedNumbers[(count - 1) / 2] + sortedNumbers[count / 2]) / 2;
    }
}
