using System;
using System.Collections.Generic;

namespace CsvProcessor.Models.DbModels;

public partial class Value
{
    public int Id { get; set; }
    
    public DateTime Date { get; set; }

    public double ExecutionTime { get; set; }

    public double PointerValue { get; set; }

    public int? FileId { get; set; }

    public virtual File? File { get; set; }
}
