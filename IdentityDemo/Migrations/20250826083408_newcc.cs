using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenantsManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class newcc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 10, 27, 57, 512, DateTimeKind.Utc).AddTicks(8418), new DateTime(2025, 8, 23, 10, 27, 57, 512, DateTimeKind.Utc).AddTicks(8419) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 10, 27, 57, 512, DateTimeKind.Utc).AddTicks(8424), new DateTime(2025, 8, 23, 10, 27, 57, 512, DateTimeKind.Utc).AddTicks(8424) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 10, 27, 57, 512, DateTimeKind.Utc).AddTicks(8428), new DateTime(2025, 8, 23, 10, 27, 57, 512, DateTimeKind.Utc).AddTicks(8429) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 10, 27, 57, 512, DateTimeKind.Utc).AddTicks(8433), new DateTime(2025, 8, 23, 10, 27, 57, 512, DateTimeKind.Utc).AddTicks(8433) });

            migrationBuilder.UpdateData(
                table: "Landlords",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 10, 27, 57, 512, DateTimeKind.Utc).AddTicks(8330), new DateTime(2025, 8, 23, 10, 27, 57, 512, DateTimeKind.Utc).AddTicks(8330) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"),
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "ModifiedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f28e42ec-e894-4a4f-8672-9532a3446e19", new DateTime(2025, 8, 23, 10, 27, 57, 512, DateTimeKind.Utc).AddTicks(7926), new DateTime(2025, 8, 23, 10, 27, 57, 512, DateTimeKind.Utc).AddTicks(7931), "AQAAAAIAAYagAAAAEGFNF5l+9F9SiveJOM01ZplL/eD0PGDCDUPfiQKbSCkKMymZTwWZwKboTLuValxRhg==", "948c2cad-8eff-4ffe-b7f4-d59a5e830db4" });
        }
    }
}
