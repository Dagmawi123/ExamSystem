using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamSystem.Migrations
{
    /// <inheritdoc />
    public partial class ResultsModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Score",
                table: "Results",
                newName: "RowScore");

            migrationBuilder.AddColumn<float>(
                name: "outOf100",
                table: "Results",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "outOf100",
                table: "Results");

            migrationBuilder.RenameColumn(
                name: "RowScore",
                table: "Results",
                newName: "Score");
        }
    }
}
