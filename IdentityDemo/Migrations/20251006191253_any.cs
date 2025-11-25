using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenantsManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class any : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 6, 19, 12, 46, 471, DateTimeKind.Utc).AddTicks(8232), new DateTime(2025, 10, 6, 19, 12, 46, 471, DateTimeKind.Utc).AddTicks(8233) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 6, 19, 12, 46, 471, DateTimeKind.Utc).AddTicks(8239), new DateTime(2025, 10, 6, 19, 12, 46, 471, DateTimeKind.Utc).AddTicks(8239) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 6, 19, 12, 46, 471, DateTimeKind.Utc).AddTicks(8244), new DateTime(2025, 10, 6, 19, 12, 46, 471, DateTimeKind.Utc).AddTicks(8244) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 6, 19, 12, 46, 471, DateTimeKind.Utc).AddTicks(8248), new DateTime(2025, 10, 6, 19, 12, 46, 471, DateTimeKind.Utc).AddTicks(8249) });

            migrationBuilder.UpdateData(
                table: "Landlords",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 6, 19, 12, 46, 471, DateTimeKind.Utc).AddTicks(8144), new DateTime(2025, 10, 6, 19, 12, 46, 471, DateTimeKind.Utc).AddTicks(8145) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"),
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "ModifiedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1a3e8045-f98a-4d13-87ff-d1300e065bb2", new DateTime(2025, 10, 6, 19, 12, 46, 471, DateTimeKind.Utc).AddTicks(7386), new DateTime(2025, 10, 6, 19, 12, 46, 471, DateTimeKind.Utc).AddTicks(7394), "AQAAAAIAAYagAAAAEOkCA6gHoP3WPvJRwiAZCRFcwZRNcymVPcIjyAuMZfz5VBrU4VBvdrveGsZstoFIdg==", "9d3b7ccb-4004-4197-9bb6-6b8182dfd821" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 21, 7, 36, 3, 379, DateTimeKind.Utc).AddTicks(2690), new DateTime(2025, 9, 21, 7, 36, 3, 379, DateTimeKind.Utc).AddTicks(2691) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 21, 7, 36, 3, 379, DateTimeKind.Utc).AddTicks(2696), new DateTime(2025, 9, 21, 7, 36, 3, 379, DateTimeKind.Utc).AddTicks(2697) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 21, 7, 36, 3, 379, DateTimeKind.Utc).AddTicks(2701), new DateTime(2025, 9, 21, 7, 36, 3, 379, DateTimeKind.Utc).AddTicks(2702) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 21, 7, 36, 3, 379, DateTimeKind.Utc).AddTicks(2706), new DateTime(2025, 9, 21, 7, 36, 3, 379, DateTimeKind.Utc).AddTicks(2707) });

            migrationBuilder.UpdateData(
                table: "Landlords",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 21, 7, 36, 3, 379, DateTimeKind.Utc).AddTicks(2530), new DateTime(2025, 9, 21, 7, 36, 3, 379, DateTimeKind.Utc).AddTicks(2531) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"),
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "ModifiedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "20baba2c-c22e-4925-ac4a-322b0484311e", new DateTime(2025, 9, 21, 7, 36, 3, 379, DateTimeKind.Utc).AddTicks(1733), new DateTime(2025, 9, 21, 7, 36, 3, 379, DateTimeKind.Utc).AddTicks(1739), "AQAAAAIAAYagAAAAEGgnH2wIXxfbzyKRs68XMTYcAXTmJAwdBZPRAiiJ5qCGn1wr0rHnAGlV5LIfRnzL+w==", "4ed12e7a-c0e4-493a-9ff6-2f9565736172" });
        }
    }
}
