using Microsoft.AspNetCore.Mvc;
using CsvProcessor.Services;
using CsvProcessor.Models.DbModels;
using CsvProcessor.Models.Filters;
using AutoMapper;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CsvProcessorController(
    CsvProcessorService csvInputService,
    DbService dbService,
    IMapper mapper) : ControllerBase
{
    [HttpPost("/")]
    public async Task<ActionResult<int>> PostCsv(IFormFile file, [FromQuery] char separator = ';')
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        using var streamReader = new StreamReader(memoryStream);

        return await csvInputService.ProcessInputFile(streamReader, file.FileName, separator);
    }

    [HttpGet("/ResultsFiltred")]
    public async Task<ActionResult<IEnumerable<ResultDto>?>> GetResults([FromQuery] ResultFilter filter)
    {
        var results = await dbService.GetResultsFiltredAsync(filter) ?? [];
        return Ok(mapper.ProjectTo<ResultDto>(results.AsQueryable()));
    }

    [HttpGet("/Last10ByFilename")]
    public async Task<ActionResult<IEnumerable<ValueDto>?>> GetLast10ByFilename([FromQuery] string filename)
    {
        var values = await dbService.GetValuesByFilenameAsync(filename) ?? [];
        return Ok(mapper.ProjectTo<ValueDto>(values.AsQueryable()));
    }
}
