using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenantsManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class ininit3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 6, 20, 3, 7, 709, DateTimeKind.Utc).AddTicks(682), new DateTime(2025, 10, 6, 20, 3, 7, 709, DateTimeKind.Utc).AddTicks(683) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 6, 20, 3, 7, 709, DateTimeKind.Utc).AddTicks(689), new DateTime(2025, 10, 6, 20, 3, 7, 709, DateTimeKind.Utc).AddTicks(689) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 6, 20, 3, 7, 709, DateTimeKind.Utc).AddTicks(693), new DateTime(2025, 10, 6, 20, 3, 7, 709, DateTimeKind.Utc).AddTicks(694) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 6, 20, 3, 7, 709, DateTimeKind.Utc).AddTicks(698), new DateTime(2025, 10, 6, 20, 3, 7, 709, DateTimeKind.Utc).AddTicks(699) });

            migrationBuilder.UpdateData(
                table: "Landlords",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 6, 20, 3, 7, 709, DateTimeKind.Utc).AddTicks(596), new DateTime(2025, 10, 6, 20, 3, 7, 709, DateTimeKind.Utc).AddTicks(597) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"),
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "ModifiedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4f382809-3e3a-414e-a9d0-23468b50b8f5", new DateTime(2025, 10, 6, 20, 3, 7, 708, DateTimeKind.Utc).AddTicks(9855), new DateTime(2025, 10, 6, 20, 3, 7, 708, DateTimeKind.Utc).AddTicks(9864), "AQAAAAIAAYagAAAAEJIyohBhVmjTRD1Qm/lN5WU25lL6bbAw+NKcs+ysyO/Qw2rRnsXb2IXpk3YpIToKQQ==", "a8994d7b-1064-42d4-a772-1b4b40adc8a7" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
