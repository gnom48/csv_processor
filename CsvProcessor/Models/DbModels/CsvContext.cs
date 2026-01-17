using Microsoft.EntityFrameworkCore;

namespace CsvProcessor.Models.DbModels;

public partial class CsvContext : DbContext
{
    public CsvContext()
    {
    }

    public CsvContext(DbContextOptions<CsvContext> options)
        : base(options)
    {
    }

    public virtual DbSet<File> Files { get; set; }

    public virtual DbSet<Result> Results { get; set; }

    public virtual DbSet<Value> Values { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("host=localhost;database=csv;username=postgres;password=tkPkU9RoqSN5hxtfK0;port=10020");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("timescaledb");

        modelBuilder.Entity<File>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("files_pkey");

            entity.ToTable("files");

            entity.HasIndex(e => e.Filename, "files_filename_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Filename)
                .HasMaxLength(255)
                .HasColumnName("filename");
            entity.Property(e => e.ProcessingStatus)
                .HasDefaultValue(ProcessingStatus.Processing)
                .HasColumnName("processing_status");
            entity.Property(e => e.UploadTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("upload_time");
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("results_pkey");

            entity.ToTable("results");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AverageExecutionTime).HasColumnName("average_execution_time");
            entity.Property(e => e.AverageValue).HasColumnName("average_value");
            entity.Property(e => e.DeltaSeconds).HasColumnName("delta_seconds");
            entity.Property(e => e.FileId).HasColumnName("file_id");

            entity.HasOne(d => d.File).WithMany()
                .HasForeignKey(d => d.FileId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("values_file_id_fkey");

            entity.Property(e => e.MaxValue).HasColumnName("max_value");
            entity.Property(e => e.MedianValue).HasColumnName("median_value");
            entity.Property(e => e.MinValue).HasColumnName("min_value");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
        });

        modelBuilder.Entity<Value>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("values");

            entity.HasIndex(e => e.Date, "idx_values_date");

            entity.HasIndex(e => e.Date, "values_date_idx").IsDescending();

            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.ExecutionTime).HasColumnName("execution_time");
            entity.Property(e => e.FileId).HasColumnName("file_id");
            entity.Property(e => e.PointerValue).HasColumnName("value");

            entity.HasOne(d => d.File).WithMany()
                .HasForeignKey(d => d.FileId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("values_file_id_fkey");
        });
        modelBuilder.HasSequence("chunk_constraint_name", "_timescaledb_catalog");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
