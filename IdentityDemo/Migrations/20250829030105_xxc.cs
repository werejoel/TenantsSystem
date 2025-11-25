using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenantsManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class xxc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "PeriodStart",
                table: "Payments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PeriodEnd",
                table: "Payments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 29, 3, 1, 3, 770, DateTimeKind.Utc).AddTicks(9003), new DateTime(2025, 8, 29, 3, 1, 3, 770, DateTimeKind.Utc).AddTicks(9003) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 29, 3, 1, 3, 770, DateTimeKind.Utc).AddTicks(9008), new DateTime(2025, 8, 29, 3, 1, 3, 770, DateTimeKind.Utc).AddTicks(9009) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 29, 3, 1, 3, 770, DateTimeKind.Utc).AddTicks(9013), new DateTime(2025, 8, 29, 3, 1, 3, 770, DateTimeKind.Utc).AddTicks(9013) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 29, 3, 1, 3, 770, DateTimeKind.Utc).AddTicks(9017), new DateTime(2025, 8, 29, 3, 1, 3, 770, DateTimeKind.Utc).AddTicks(9017) });

            migrationBuilder.UpdateData(
                table: "Landlords",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 29, 3, 1, 3, 770, DateTimeKind.Utc).AddTicks(8946), new DateTime(2025, 8, 29, 3, 1, 3, 770, DateTimeKind.Utc).AddTicks(8947) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"),
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "ModifiedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "05a86cb8-6a9c-4e5a-9458-42528e4248d6", new DateTime(2025, 8, 29, 3, 1, 3, 770, DateTimeKind.Utc).AddTicks(8622), new DateTime(2025, 8, 29, 3, 1, 3, 770, DateTimeKind.Utc).AddTicks(8626), "AQAAAAIAAYagAAAAEKj1f9qsEO6d18tR5+F6kyi385L6fWSyJnRU37xqhDlQukbtE75K1MJy2QNfyEJz1w==", "a4a6e88d-8b9f-4dd8-810a-607741bea40d" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "PeriodStart",
                table: "Payments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PeriodEnd",
                table: "Payments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

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
    }
}
