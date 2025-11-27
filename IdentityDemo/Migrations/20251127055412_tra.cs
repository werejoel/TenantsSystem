using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenantsManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class tra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 25, 8, 45, 31, 130, DateTimeKind.Utc).AddTicks(8873), new DateTime(2025, 11, 25, 8, 45, 31, 130, DateTimeKind.Utc).AddTicks(8874) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 25, 8, 45, 31, 130, DateTimeKind.Utc).AddTicks(8879), new DateTime(2025, 11, 25, 8, 45, 31, 130, DateTimeKind.Utc).AddTicks(8880) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 25, 8, 45, 31, 130, DateTimeKind.Utc).AddTicks(8885), new DateTime(2025, 11, 25, 8, 45, 31, 130, DateTimeKind.Utc).AddTicks(8886) });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 25, 8, 45, 31, 130, DateTimeKind.Utc).AddTicks(8890), new DateTime(2025, 11, 25, 8, 45, 31, 130, DateTimeKind.Utc).AddTicks(8891) });

            migrationBuilder.UpdateData(
                table: "Landlords",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 25, 8, 45, 31, 130, DateTimeKind.Utc).AddTicks(8631), new DateTime(2025, 11, 25, 8, 45, 31, 130, DateTimeKind.Utc).AddTicks(8631) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"),
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "ModifiedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "85be9f84-91df-4b53-8f7a-893654f38b2d", new DateTime(2025, 11, 25, 8, 45, 31, 130, DateTimeKind.Utc).AddTicks(7866), new DateTime(2025, 11, 25, 8, 45, 31, 130, DateTimeKind.Utc).AddTicks(7872), "AQAAAAIAAYagAAAAENX6YYaCpzb/Vh0+uY7N4KlwBLTmavUOFH1M9XtpfmCL4KMJapbl05A1DqSlhlDLcQ==", "dba292ba-2a64-408e-baf6-7b312d0829c6" });
        }
    }
}
