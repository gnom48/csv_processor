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

    public async Task<int> ProcessInputFile(StreamReader reader, string filename, char separator = ';')
    {
        int counter = 0;

        reader.BaseStream.Seek(0, SeekOrigin.Begin);

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

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (line == null) break;

                var parts = line.Split(separator);

                var valueItem = Validations.MapAndThrowIfInvalid(parts);
                valueItem.FileId = currentFile?.Id
                    ?? throw new ValidationException("Не указан файл.");
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

            #region statistics calc

            var resultEntry = new Result
            {
                FileId = currentFile.Id,
                DeltaSeconds = TimeSpan.FromSeconds((lastDate!.Value - firstDate!.Value).TotalSeconds),
                StartTime = firstDate!.Value,
                AverageExecutionTime = sumExecutionTime / counter,
                AverageValue = sumValue / counter,
                MedianValue = Validations.CalculateMedian(valuesForMedian),
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
        catch (ValidationException ex)
        {
            logger.LogError(ex, "Выявлена ошибка формата файла");
            await RollbackFileAsync();
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Непредвиденная ошибка при обработке файла");
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
            UploadTime = DateTime.UtcNow
        };
        dbContext.Files.Add(currentFile);
        await dbContext.SaveChangesAsync();
    }

    private async Task SaveBatchAsync(List<Value> batch)
    {
        dbContext.Values.AddRange(batch);
        await dbContext.SaveChangesAsync();
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
}
