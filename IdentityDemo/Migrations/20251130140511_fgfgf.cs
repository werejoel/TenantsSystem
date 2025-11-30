using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenantsManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class fgfgf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 5, 10, 293, DateTimeKind.Utc).AddTicks(8864), new DateTime(2025, 11, 30, 14, 5, 10, 293, DateTimeKind.Utc).AddTicks(8865) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 5, 10, 293, DateTimeKind.Utc).AddTicks(8873), new DateTime(2025, 11, 30, 14, 5, 10, 293, DateTimeKind.Utc).AddTicks(8874) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 5, 10, 293, DateTimeKind.Utc).AddTicks(8881), new DateTime(2025, 11, 30, 14, 5, 10, 293, DateTimeKind.Utc).AddTicks(8882) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 5, 10, 293, DateTimeKind.Utc).AddTicks(8889), new DateTime(2025, 11, 30, 14, 5, 10, 293, DateTimeKind.Utc).AddTicks(8889) });

            migrationBuilder.UpdateData(
                table: "Landlords",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 5, 10, 293, DateTimeKind.Utc).AddTicks(8621), new DateTime(2025, 11, 30, 14, 5, 10, 293, DateTimeKind.Utc).AddTicks(8622) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"),
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "ModifiedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4c5afa38-0015-4630-a39c-74ea764ef6ee", new DateTime(2025, 11, 30, 14, 5, 10, 293, DateTimeKind.Utc).AddTicks(7490), new DateTime(2025, 11, 30, 14, 5, 10, 293, DateTimeKind.Utc).AddTicks(7496), "AQAAAAIAAYagAAAAENADUEJYJLhxqdgFhyeIGJxJbJ61KqY3XIhAm8SCNB0imGsBYDFunrKStBUMBknupQ==", "a6af7238-f47e-4600-9db9-179c9c5e4d39" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
