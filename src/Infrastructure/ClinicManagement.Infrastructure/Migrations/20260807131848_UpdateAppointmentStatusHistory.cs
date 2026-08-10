using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppointmentStatusHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentId1",
                table: "AppointmentStatusHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentStatusHistories_AppointmentId1",
                table: "AppointmentStatusHistories",
                column: "AppointmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentStatusHistories_Appointments_AppointmentId1",
                table: "AppointmentStatusHistories",
                column: "AppointmentId1",
                principalTable: "Appointments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentStatusHistories_Appointments_AppointmentId1",
                table: "AppointmentStatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentStatusHistories_AppointmentId1",
                table: "AppointmentStatusHistories");

            migrationBuilder.DropColumn(
                name: "AppointmentId1",
                table: "AppointmentStatusHistories");
        }
    }
}
