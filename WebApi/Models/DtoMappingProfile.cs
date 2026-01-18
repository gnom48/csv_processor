using AutoMapper;
using CsvProcessor.Models.DbModels;

namespace WebApi.Models;

public class DtoMappingProfile : Profile
{
    public DtoMappingProfile()
    {
        CreateMap<Value, ValueDto>();
        CreateMap<Result, ResultDto>();
    }
}
