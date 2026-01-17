using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CsvProcessor.Migrations
{
    /// <inheritdoc />
    public partial class UpdateResultsFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_results_file_name",
                table: "results");

            migrationBuilder.DropIndex(
                name: "results_file_name_key",
                table: "results");

            migrationBuilder.DropColumn(
                name: "file_name",
                table: "results");

            migrationBuilder.AddColumn<int>(
                name: "file_id",
                table: "results",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_results_file_id",
                table: "results",
                column: "file_id");

            migrationBuilder.AddForeignKey(
                name: "values_file_id_fkey",
                table: "results",
                column: "file_id",
                principalTable: "files",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "values_file_id_fkey",
                table: "results");

            migrationBuilder.DropIndex(
                name: "IX_results_file_id",
                table: "results");

            migrationBuilder.DropColumn(
                name: "file_id",
                table: "results");

            migrationBuilder.AddColumn<string>(
                name: "file_name",
                table: "results",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "idx_results_file_name",
                table: "results",
                column: "file_name");

            migrationBuilder.CreateIndex(
                name: "results_file_name_key",
                table: "results",
                column: "file_name",
                unique: true);
        }
    }
}
