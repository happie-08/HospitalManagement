using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOPDTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OPDs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    DiagnosisId = table.Column<int>(type: "int", nullable: true),
                    SymptomId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InvestNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OPDs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OPDs_Masters_DiagnosisId",
                        column: x => x.DiagnosisId,
                        principalTable: "Masters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OPDs_Masters_SymptomId",
                        column: x => x.SymptomId,
                        principalTable: "Masters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OPDs_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OPDs_ReferenceDoctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "ReferenceDoctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OPDs_DiagnosisId",
                table: "OPDs",
                column: "DiagnosisId");

            migrationBuilder.CreateIndex(
                name: "IX_OPDs_DoctorId",
                table: "OPDs",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_OPDs_PatientId",
                table: "OPDs",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_OPDs_SymptomId",
                table: "OPDs",
                column: "SymptomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OPDs");
        }
    }
}
