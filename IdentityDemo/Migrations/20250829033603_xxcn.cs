using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenantsManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class xxcn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

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
    }
}
