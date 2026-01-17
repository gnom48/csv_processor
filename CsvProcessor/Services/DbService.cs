using CsvProcessor.Models.DbModels;
using CsvProcessor.Models.Filters;
using Microsoft.EntityFrameworkCore;

namespace CsvProcessor.Services;

public class DbService(
    CsvContext dbContext)
{
    public async Task<IEnumerable<Result>> GetResultsFiltredAsync(ResultFilter filter)
    {
        var query = dbContext.Results.AsQueryable();

        if (!string.IsNullOrEmpty(filter.Filename))
        {
            query = query.Where(r => r.File!.Filename == filter.Filename);
        }

        if (filter.StartTimeFrom.HasValue)
        {
            query = query.Where(r => r.StartTime >= filter.StartTimeFrom.Value);
        }
        if (filter.StartTimeTo.HasValue)
        {
            query = query.Where(r => r.StartTime <= filter.StartTimeTo.Value);
        }

        if (filter.AvgValueMin.HasValue)
        {
            query = query.Where(r => r.AverageValue >= filter.AvgValueMin.Value);
        }
        if (filter.AvgValueMax.HasValue)
        {
            query = query.Where(r => r.AverageValue <= filter.AvgValueMax.Value);
        }

        if (filter.AvgExecTimeMin.HasValue)
        {
            query = query.Where(r => r.AverageExecutionTime >= filter.AvgExecTimeMin.Value);
        }
        if (filter.AvgExecTimeMax.HasValue)
        {
            query = query.Where(r => r.AverageExecutionTime <= filter.AvgExecTimeMax.Value);
        }

        return await query
            .ToListAsync();
    }

    public async Task<IEnumerable<Value>?> GetValuesByFilenameAsync(string filename, int count = 10)
    {
        try
        {
            return await dbContext.Values
                .Where(x => x.File!.Filename == filename)
                .OrderByDescending(x => x.Date)
                .Take(10)
                .ToListAsync();
        }
        catch
        {
            return null;
        }
    }
}
