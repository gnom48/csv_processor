using System.Globalization;
using CsvProcessor.Exceprtons;
using CsvProcessor.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CsvProcessor.Services;

public class CsvProcessorService(
    CsvContext dbContext,
    ILogger<CsvProcessorService> logger)
{
    private Models.DbModels.File? currentFile = null;

    public async Task<int> ProcessInputFile(Stream fileStream, string filename = "file", char separator = ';')
    {
        int counter = 0;

        try
        {
            await SaveFileNameAsync(filename);
            if (currentFile == null) throw new Exception();

            List<Value> batch = new List<Value>();
            DateTime? firstDate = null, lastDate = null;
            double sumExecutionTime = 0, sumValue = 0;
            double minExecutionTime = double.PositiveInfinity, maxExecutionTime = double.NegativeInfinity;
            double minValue = double.PositiveInfinity, maxValue = double.NegativeInfinity;
            List<double> valuesForMedian = new List<double>();

            using (var reader = new StreamReader(fileStream))
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (line == null) break;

                    var parts = line.Split(separator);

                    var valueItem = MapAndThrowIfInvalid(parts);
                    batch.Add(valueItem);

                    #region statistics

                    if (firstDate == null || valueItem.Date < firstDate)
                        firstDate = valueItem.Date;
                    if (lastDate == null || valueItem.Date > lastDate)
                        lastDate = valueItem.Date;

                    sumExecutionTime += valueItem.ExecutionTime;
                    sumValue += valueItem.PointerValue;

                    minExecutionTime = Math.Min(minExecutionTime, valueItem.ExecutionTime);
                    maxExecutionTime = Math.Max(maxExecutionTime, valueItem.ExecutionTime);

                    minValue = Math.Min(minValue, valueItem.PointerValue);
                    maxValue = Math.Max(maxValue, valueItem.PointerValue);

                    valuesForMedian.Add(valueItem.PointerValue);

                    #endregion statistics

                    if (batch.Count >= 1000)
                    {
                        await SaveBatchAsync(batch);
                        counter += batch.Count;
                        batch.Clear();
                    }
                }

                if (batch.Any())
                {
                    await SaveBatchAsync(batch);
                    counter += batch.Count;
                }
            }

            #region statistics calc

            var resultEntry = new Result
            {
                FileId = currentFile.Id,
                DeltaSeconds = TimeSpan.FromSeconds((lastDate!.Value - firstDate!.Value).TotalSeconds),
                StartTime = firstDate!.Value,
                AverageExecutionTime = sumExecutionTime / counter,
                AverageValue = sumValue / counter,
                MedianValue = CalculateMedian(valuesForMedian),
                MaxValue = maxValue,
                MinValue = minValue
            };

            dbContext.Results.Add(resultEntry);
            await dbContext.SaveChangesAsync();

            #endregion statistics calc

            await dbContext.Files
                .Where(x => x.Id == currentFile.Id)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.ProcessingStatus, ProcessingStatus.Success));
        }
        catch (ValidationException)
        {
            logger.LogError("Выявлена ошибка формата файла");
            await RollbackFileAsync();
            throw;
        }
        catch (Exception)
        {
            logger.LogError("Непредвиденная ошибка при обработке файла");
            await RollbackFileAsync();
            throw new ProcessException();
        }

        return counter;
    }

    private async Task SaveFileNameAsync(string filename)
    {
        var oldFile = await dbContext.Files.FirstOrDefaultAsync(x => x.Filename == filename);
        if (oldFile != null)
            await dbContext.Files
                .Where(x => x.Id == oldFile.Id)
                .ExecuteDeleteAsync();

        currentFile = new Models.DbModels.File
        {
            Filename = filename,
            ProcessingStatus = ProcessingStatus.Processing,
            UploadTime = DateTime.Now
        };
        dbContext.Files.Add(currentFile);
        await dbContext.SaveChangesAsync();
    }

    private Value MapAndThrowIfInvalid(string[] rowParts)
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
            FileId = currentFile?.Id ?? throw new ValidationException("Не указан файл.")
        };
    }

    private async Task SaveBatchAsync(List<Value> batch)
    {
        using (var transaction = dbContext.Database.BeginTransaction())
        {
            dbContext.Values.AddRange(batch);
            await dbContext.SaveChangesAsync();
            transaction.Commit();
        }
    }
    private async Task RollbackFileAsync()
    {
        if (currentFile != null)
        {
            await dbContext.Files
                .Where(x => x.Id == currentFile.Id)
                .ExecuteDeleteAsync();
            logger.LogError("Откат");
        }
    }

    private static double CalculateMedian(IEnumerable<double> numbers)
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
