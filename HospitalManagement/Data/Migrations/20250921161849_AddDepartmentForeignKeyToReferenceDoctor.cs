using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    public partial class AddDepartmentForeignKeyToReferenceDoctor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add DepartmentId column (nullable first to avoid breaking existing data)
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "ReferenceDoctors",
                type: "int",
                nullable: true); // nullable for now

            // Create index for FK
            migrationBuilder.CreateIndex(
                name: "IX_ReferenceDoctors_DepartmentId",
                table: "ReferenceDoctors",
                column: "DepartmentId");

            // Add the FK constraint
            migrationBuilder.AddForeignKey(
                name: "FK_ReferenceDoctors_Departments_DepartmentId",
                table: "ReferenceDoctors",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict); // Restrict to avoid accidental deletes
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop FK first
            migrationBuilder.DropForeignKey(
                name: "FK_ReferenceDoctors_Departments_DepartmentId",
                table: "ReferenceDoctors");

            // Drop index
            migrationBuilder.DropIndex(
                name: "IX_ReferenceDoctors_DepartmentId",
                table: "ReferenceDoctors");

            // Drop the column
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "ReferenceDoctors");
        }
    }
}
