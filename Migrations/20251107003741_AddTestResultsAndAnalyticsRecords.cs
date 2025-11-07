using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediScope.Migrations
{
    /// <inheritdoc />
    public partial class AddTestResultsAndAnalyticsRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Doctors_DoctorId",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Patients_PatientId",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_DoctorId",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_PatientId",
                table: "TestResults");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TestResults_DoctorId",
                table: "TestResults",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_PatientId",
                table: "TestResults",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Doctors_DoctorId",
                table: "TestResults",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Patients_PatientId",
                table: "TestResults",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
