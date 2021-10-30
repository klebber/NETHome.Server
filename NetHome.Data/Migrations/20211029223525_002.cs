using Microsoft.EntityFrameworkCore.Migrations;

namespace NetHome.Data.Migrations
{
    public partial class _002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Percentage",
                table: "Device",
                newName: "FavPos4");

            migrationBuilder.AddColumn<int>(
                name: "CurrentPercentage",
                table: "Device",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FavPos1",
                table: "Device",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FavPos2",
                table: "Device",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FavPos3",
                table: "Device",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Humidity",
                table: "Device",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Placement",
                table: "Device",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "THSensor_Temperature",
                table: "Device",
                type: "REAL",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPercentage",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "FavPos1",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "FavPos2",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "FavPos3",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "Placement",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "THSensor_Temperature",
                table: "Device");

            migrationBuilder.RenameColumn(
                name: "FavPos4",
                table: "Device",
                newName: "Percentage");
        }
    }
}
