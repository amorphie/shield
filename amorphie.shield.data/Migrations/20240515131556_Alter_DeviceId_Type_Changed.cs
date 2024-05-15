using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amorphie.shield.Migrations
{
    /// <inheritdoc />
    public partial class Alter_DeviceId_Type_Changed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "Certificates",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceId",
                table: "Certificates",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
