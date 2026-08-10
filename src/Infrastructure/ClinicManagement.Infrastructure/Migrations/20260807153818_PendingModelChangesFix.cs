using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PendingModelChangesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PatientDocuments_PatientId",
                table: "PatientDocuments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDocuments_PatientId_CreatedAt",
                table: "PatientDocuments",
                columns: new[] { "PatientId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId_ScheduledStart",
                table: "Appointments",
                columns: new[] { "PatientId", "ScheduledStart" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PatientDocuments_PatientId_CreatedAt",
                table: "PatientDocuments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PatientId_ScheduledStart",
                table: "Appointments");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDocuments_PatientId",
                table: "PatientDocuments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");
        }
    }
}
