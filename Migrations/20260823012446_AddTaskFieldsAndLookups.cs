using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rmp.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskFieldsAndLookups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "Tasks",
                newName: "StartDate");

            migrationBuilder.AddColumn<int>(
                name: "AppNameId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ComponentId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Tasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VersionId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Lookups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lookups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AppNameId",
                table: "Tasks",
                column: "AppNameId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ComponentId",
                table: "Tasks",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TypeId",
                table: "Tasks",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_VersionId",
                table: "Tasks",
                column: "VersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Lookups_Category_Value",
                table: "Lookups",
                columns: new[] { "Category", "Value" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Lookups_AppNameId",
                table: "Tasks",
                column: "AppNameId",
                principalTable: "Lookups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Lookups_ComponentId",
                table: "Tasks",
                column: "ComponentId",
                principalTable: "Lookups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Lookups_TypeId",
                table: "Tasks",
                column: "TypeId",
                principalTable: "Lookups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Lookups_VersionId",
                table: "Tasks",
                column: "VersionId",
                principalTable: "Lookups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Lookups_AppNameId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Lookups_ComponentId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Lookups_TypeId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Lookups_VersionId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "Lookups");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AppNameId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_ComponentId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TypeId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_VersionId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AppNameId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ComponentId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "VersionId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Tasks",
                newName: "DueDate");
        }
    }
}
