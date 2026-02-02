using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Metrics.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMetricIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ExecutionMetrics_ActivityType_ActivityName",
                table: "ExecutionMetrics",
                columns: new[] { "ActivityType", "ActivityName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExecutionMetrics_ActivityType_ActivityName",
                table: "ExecutionMetrics");
        }
    }
}
