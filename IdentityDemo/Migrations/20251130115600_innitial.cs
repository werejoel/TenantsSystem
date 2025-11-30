using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenantsManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class innitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 30, 11, 55, 59, 155, DateTimeKind.Utc).AddTicks(2549), new DateTime(2025, 11, 30, 11, 55, 59, 155, DateTimeKind.Utc).AddTicks(2550) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 30, 11, 55, 59, 155, DateTimeKind.Utc).AddTicks(2557), new DateTime(2025, 11, 30, 11, 55, 59, 155, DateTimeKind.Utc).AddTicks(2558) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 30, 11, 55, 59, 155, DateTimeKind.Utc).AddTicks(2563), new DateTime(2025, 11, 30, 11, 55, 59, 155, DateTimeKind.Utc).AddTicks(2564) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 30, 11, 55, 59, 155, DateTimeKind.Utc).AddTicks(2570), new DateTime(2025, 11, 30, 11, 55, 59, 155, DateTimeKind.Utc).AddTicks(2571) });

            migrationBuilder.UpdateData(
                table: "Landlords",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 30, 11, 55, 59, 155, DateTimeKind.Utc).AddTicks(2273), new DateTime(2025, 11, 30, 11, 55, 59, 155, DateTimeKind.Utc).AddTicks(2274) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"),
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "ModifiedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1d858377-75b0-4e56-bdf8-8b6da49b562b", new DateTime(2025, 11, 30, 11, 55, 59, 155, DateTimeKind.Utc).AddTicks(1318), new DateTime(2025, 11, 30, 11, 55, 59, 155, DateTimeKind.Utc).AddTicks(1330), "AQAAAAIAAYagAAAAEFI/vBNfbyUlwI05cYnSAgAEY4/Lpc2VPbstF5Jmlt7sRqAY2OtN7KTYGOuday5zdQ==", "5dc5b204-b25d-4e66-b275-9d4d08bf1390" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 27, 5, 54, 10, 374, DateTimeKind.Utc).AddTicks(616), new DateTime(2025, 11, 27, 5, 54, 10, 374, DateTimeKind.Utc).AddTicks(617) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 27, 5, 54, 10, 374, DateTimeKind.Utc).AddTicks(623), new DateTime(2025, 11, 27, 5, 54, 10, 374, DateTimeKind.Utc).AddTicks(623) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 27, 5, 54, 10, 374, DateTimeKind.Utc).AddTicks(628), new DateTime(2025, 11, 27, 5, 54, 10, 374, DateTimeKind.Utc).AddTicks(628) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 27, 5, 54, 10, 374, DateTimeKind.Utc).AddTicks(632), new DateTime(2025, 11, 27, 5, 54, 10, 374, DateTimeKind.Utc).AddTicks(633) });

            migrationBuilder.UpdateData(
                table: "Landlords",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 27, 5, 54, 10, 374, DateTimeKind.Utc).AddTicks(536), new DateTime(2025, 11, 27, 5, 54, 10, 374, DateTimeKind.Utc).AddTicks(537) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"),
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "ModifiedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4f762d64-b084-464d-8e4a-6f974f2d3937", new DateTime(2025, 11, 27, 5, 54, 10, 373, DateTimeKind.Utc).AddTicks(9849), new DateTime(2025, 11, 27, 5, 54, 10, 373, DateTimeKind.Utc).AddTicks(9857), "AQAAAAIAAYagAAAAEObc7cgG6/1RZuiju3RPoz8zp8mdStyRjHt5EEykdyMsJTyEBLIh2ERt75lZy4lI1Q==", "5cb1a490-cf85-4651-9fbe-e6ec194db3dc" });
        }
    }
}
