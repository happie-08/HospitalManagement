using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOPDMultiSelect : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OPDs_Masters_DiagnosisId",
                table: "OPDs");

            migrationBuilder.DropForeignKey(
                name: "FK_OPDs_Masters_SymptomId",
                table: "OPDs");

            migrationBuilder.DropIndex(
                name: "IX_OPDs_DiagnosisId",
                table: "OPDs");

            migrationBuilder.DropIndex(
                name: "IX_OPDs_SymptomId",
                table: "OPDs");

            migrationBuilder.DropColumn(
                name: "DiagnosisId",
                table: "OPDs");

            migrationBuilder.DropColumn(
                name: "SymptomId",
                table: "OPDs");

            migrationBuilder.CreateTable(
                name: "OPDDiagnoses",
                columns: table => new
                {
                    OPDId = table.Column<int>(type: "int", nullable: false),
                    DiagnosisId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OPDDiagnoses", x => new { x.OPDId, x.DiagnosisId });
                    table.ForeignKey(
                        name: "FK_OPDDiagnoses_Masters_DiagnosisId",
                        column: x => x.DiagnosisId,
                        principalTable: "Masters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OPDDiagnoses_OPDs_OPDId",
                        column: x => x.OPDId,
                        principalTable: "OPDs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OPDSymptoms",
                columns: table => new
                {
                    OPDId = table.Column<int>(type: "int", nullable: false),
                    SymptomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OPDSymptoms", x => new { x.OPDId, x.SymptomId });
                    table.ForeignKey(
                        name: "FK_OPDSymptoms_Masters_SymptomId",
                        column: x => x.SymptomId,
                        principalTable: "Masters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OPDSymptoms_OPDs_OPDId",
                        column: x => x.OPDId,
                        principalTable: "OPDs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OPDDiagnoses_DiagnosisId",
                table: "OPDDiagnoses",
                column: "DiagnosisId");

            migrationBuilder.CreateIndex(
                name: "IX_OPDSymptoms_SymptomId",
                table: "OPDSymptoms",
                column: "SymptomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OPDDiagnoses");

            migrationBuilder.DropTable(
                name: "OPDSymptoms");

            migrationBuilder.AddColumn<int>(
                name: "DiagnosisId",
                table: "OPDs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SymptomId",
                table: "OPDs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OPDs_DiagnosisId",
                table: "OPDs",
                column: "DiagnosisId");

            migrationBuilder.CreateIndex(
                name: "IX_OPDs_SymptomId",
                table: "OPDs",
                column: "SymptomId");

            migrationBuilder.AddForeignKey(
                name: "FK_OPDs_Masters_DiagnosisId",
                table: "OPDs",
                column: "DiagnosisId",
                principalTable: "Masters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OPDs_Masters_SymptomId",
                table: "OPDs",
                column: "SymptomId",
                principalTable: "Masters",
                principalColumn: "Id");
        }
    }
}
