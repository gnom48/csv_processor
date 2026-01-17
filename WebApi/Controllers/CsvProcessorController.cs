using Microsoft.AspNetCore.Mvc;
using CsvProcessor.Services;
using CsvProcessor.Models.DbModels;
using CsvProcessor.Models.Filters;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CsvProcessorController(
    CsvProcessorService csvInputService,
    DbService dbService) : ControllerBase
{
    [HttpPost("/PostCsv")]
    public async Task<ActionResult<int>> PostCsv(IFormFile file, [FromQuery] char separator = ';')
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        using var streamReader = new StreamReader(memoryStream);
        
        return await csvInputService.ProcessInputFile(streamReader, file.FileName, separator);
    }

    [HttpGet("/GetResultsFiltred")]
    public async Task<ActionResult<IEnumerable<Value>>> GetResults([FromQuery] ResultFilter filter)
        => Ok(await dbService.GetResultsFiltredAsync(filter));

    [HttpGet("/GetLast10ByFilename")]
    public async Task<ActionResult<IEnumerable<Value>?>> GetLast10ByFilename([FromQuery] string filename)
        => Ok(await dbService.GetValuesByFilenameAsync(filename));
}
