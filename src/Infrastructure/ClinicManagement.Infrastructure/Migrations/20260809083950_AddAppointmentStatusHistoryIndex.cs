using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentStatusHistoryIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppointmentStatusHistories_AppointmentId",
                table: "AppointmentStatusHistories");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentStatusHistories_AppointmentId_ChangedAt",
                table: "AppointmentStatusHistories",
                columns: new[] { "AppointmentId", "ChangedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppointmentStatusHistories_AppointmentId_ChangedAt",
                table: "AppointmentStatusHistories");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentStatusHistories_AppointmentId",
                table: "AppointmentStatusHistories",
                column: "AppointmentId");
        }
    }
}
