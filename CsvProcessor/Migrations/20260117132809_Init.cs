using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CsvProcessor.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "_timescaledb_catalog");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:timescaledb", ",,");

            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    filename = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    upload_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    processing_status = table.Column<byte>(type: "smallint", nullable: true, defaultValue: (byte)2)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "results",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_id = table.Column<int>(type: "integer", nullable: true),
                    delta_seconds = table.Column<TimeSpan>(type: "interval", nullable: true),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    average_execution_time = table.Column<double>(type: "double precision", nullable: true),
                    average_value = table.Column<double>(type: "double precision", nullable: true),
                    median_value = table.Column<double>(type: "double precision", nullable: true),
                    max_value = table.Column<double>(type: "double precision", nullable: true),
                    min_value = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("results_pkey", x => x.id);
                    table.ForeignKey(
                        name: "values_file_id_fkey",
                        column: x => x.file_id,
                        principalTable: "files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "values",
                columns: table => new
                {
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    execution_time = table.Column<double>(type: "double precision", nullable: false),
                    value = table.Column<double>(type: "double precision", nullable: false),
                    file_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "values_file_id_fkey",
                        column: x => x.file_id,
                        principalTable: "files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "files_filename_key",
                table: "files",
                column: "filename",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_results_file_id",
                table: "results",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "idx_values_date",
                table: "values",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "IX_values_file_id",
                table: "values",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "values_date_idx",
                table: "values",
                column: "date",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "results");

            migrationBuilder.DropTable(
                name: "values");

            migrationBuilder.DropTable(
                name: "files");
        }
    }
}
