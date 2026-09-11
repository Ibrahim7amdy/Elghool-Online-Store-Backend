using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchManagerRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_Branches_BranchId",
                table: "Admins");

            migrationBuilder.DropIndex(
                name: "IX_Admins_BranchId",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Admins");

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "Branches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuperAdmin",
                table: "Admins",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_ManagerId",
                table: "Branches",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Employees_ManagerId",
                table: "Branches",
                column: "ManagerId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Employees_ManagerId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_ManagerId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "IsSuperAdmin",
                table: "Admins");

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Admins",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admins_BranchId",
                table: "Admins",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_Branches_BranchId",
                table: "Admins",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
