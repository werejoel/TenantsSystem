using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenantsManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class Roless : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 29, 3, 36, 1, 895, DateTimeKind.Utc).AddTicks(9567), new DateTime(2025, 8, 29, 3, 36, 1, 895, DateTimeKind.Utc).AddTicks(9567) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 29, 3, 36, 1, 895, DateTimeKind.Utc).AddTicks(9573), new DateTime(2025, 8, 29, 3, 36, 1, 895, DateTimeKind.Utc).AddTicks(9574) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 29, 3, 36, 1, 895, DateTimeKind.Utc).AddTicks(9580), new DateTime(2025, 8, 29, 3, 36, 1, 895, DateTimeKind.Utc).AddTicks(9580) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 29, 3, 36, 1, 895, DateTimeKind.Utc).AddTicks(9586), new DateTime(2025, 8, 29, 3, 36, 1, 895, DateTimeKind.Utc).AddTicks(9587) });

            migrationBuilder.UpdateData(
                table: "Landlords",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 29, 3, 36, 1, 895, DateTimeKind.Utc).AddTicks(9338), new DateTime(2025, 8, 29, 3, 36, 1, 895, DateTimeKind.Utc).AddTicks(9339) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"),
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "ModifiedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1572d93b-0704-458c-9581-42da40619588", new DateTime(2025, 8, 29, 3, 36, 1, 895, DateTimeKind.Utc).AddTicks(8555), new DateTime(2025, 8, 29, 3, 36, 1, 895, DateTimeKind.Utc).AddTicks(8560), "AQAAAAIAAYagAAAAEIegQfelYlPFFeD2nTd+8WJb47uHp9Erj7HQD3s/QAmmxUXc9JNp4XamhqTpfwT7XQ==", "67fcb427-9dc4-4d60-9244-df34a91cb49a" });
        }
    }
}
