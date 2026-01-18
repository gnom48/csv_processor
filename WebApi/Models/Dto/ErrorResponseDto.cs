using System;

namespace WebApi.Models.Dto;

public class ErrorResponseDto
{
    public string Msg { get; set; } = null!;
    public string? Detail { get; set; } = null;
}
