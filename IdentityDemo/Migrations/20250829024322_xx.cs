using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenantsManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class xx : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 29, 2, 43, 21, 294, DateTimeKind.Utc).AddTicks(9856), new DateTime(2025, 8, 29, 2, 43, 21, 294, DateTimeKind.Utc).AddTicks(9856) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 29, 2, 43, 21, 294, DateTimeKind.Utc).AddTicks(9860), new DateTime(2025, 8, 29, 2, 43, 21, 294, DateTimeKind.Utc).AddTicks(9861) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 29, 2, 43, 21, 294, DateTimeKind.Utc).AddTicks(9865), new DateTime(2025, 8, 29, 2, 43, 21, 294, DateTimeKind.Utc).AddTicks(9865) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 29, 2, 43, 21, 294, DateTimeKind.Utc).AddTicks(9869), new DateTime(2025, 8, 29, 2, 43, 21, 294, DateTimeKind.Utc).AddTicks(9869) });

            migrationBuilder.UpdateData(
                table: "Landlords",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 29, 2, 43, 21, 294, DateTimeKind.Utc).AddTicks(9800), new DateTime(2025, 8, 29, 2, 43, 21, 294, DateTimeKind.Utc).AddTicks(9801) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"),
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "ModifiedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bd40d0ae-67a0-4050-b996-154eb9215850", new DateTime(2025, 8, 29, 2, 43, 21, 294, DateTimeKind.Utc).AddTicks(9491), new DateTime(2025, 8, 29, 2, 43, 21, 294, DateTimeKind.Utc).AddTicks(9497), "AQAAAAIAAYagAAAAEOmaSn5/HfleCuskWKqgtRm/2IHMHpwBiEU64Xdw2jIhAu30LEO3VEWdCh197EY/Eg==", "30d4a011-8834-48a9-ae4e-bd8f2960d488" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 8, 34, 5, 624, DateTimeKind.Utc).AddTicks(5468), new DateTime(2025, 8, 26, 8, 34, 5, 624, DateTimeKind.Utc).AddTicks(5469) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 8, 34, 5, 624, DateTimeKind.Utc).AddTicks(5473), new DateTime(2025, 8, 26, 8, 34, 5, 624, DateTimeKind.Utc).AddTicks(5474) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 8, 34, 5, 624, DateTimeKind.Utc).AddTicks(5478), new DateTime(2025, 8, 26, 8, 34, 5, 624, DateTimeKind.Utc).AddTicks(5478) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 8, 34, 5, 624, DateTimeKind.Utc).AddTicks(5482), new DateTime(2025, 8, 26, 8, 34, 5, 624, DateTimeKind.Utc).AddTicks(5483) });

            migrationBuilder.UpdateData(
                table: "Landlords",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 8, 34, 5, 624, DateTimeKind.Utc).AddTicks(5415), new DateTime(2025, 8, 26, 8, 34, 5, 624, DateTimeKind.Utc).AddTicks(5416) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"),
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "ModifiedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "23940f87-1447-4a4e-9ab4-fd652643cb99", new DateTime(2025, 8, 26, 8, 34, 5, 624, DateTimeKind.Utc).AddTicks(5113), new DateTime(2025, 8, 26, 8, 34, 5, 624, DateTimeKind.Utc).AddTicks(5118), "AQAAAAIAAYagAAAAEH3P5q+8I9LkI97F825gFw/xorN9ugfW26sNZQx0Qn8iFpUA5cH8Nzdf+xzxXoYauw==", "53c11b71-050e-445f-86b5-0ddd01d1a0e6" });
        }
    }
}
