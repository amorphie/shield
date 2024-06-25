using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amorphie.shield.Migrations
{
    /// <inheritdoc />
    public partial class cert_origin_col_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Origin",
                table: "Certificates",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Origin",
                table: "Certificates");
        }
    }
}
