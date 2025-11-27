using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TenantsManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class iij : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Notifications",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TxRef = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FlwRef = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    PaymentId = table.Column<int>(type: "int", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CustomerEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CustomerPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RawResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Verified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

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

            migrationBuilder.InsertData(
                table: "PaymentMethods",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "MTN", new DateTime(2025, 8, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "MTN Mobile Money (Uganda)", true, "MTN Mobile Money", new DateTime(2025, 8, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "AIRTEL", new DateTime(2025, 8, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Airtel Money (Uganda)", true, "Airtel Money", new DateTime(2025, 8, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "BANK", new DateTime(2025, 8, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Direct bank transfer", true, "Bank Transfer", new DateTime(2025, 8, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"),
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "ModifiedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "85be9f84-91df-4b53-8f7a-893654f38b2d", new DateTime(2025, 11, 25, 8, 45, 31, 130, DateTimeKind.Utc).AddTicks(7866), new DateTime(2025, 11, 25, 8, 45, 31, 130, DateTimeKind.Utc).AddTicks(7872), "AQAAAAIAAYagAAAAENX6YYaCpzb/Vh0+uY7N4KlwBLTmavUOFH1M9XtpfmCL4KMJapbl05A1DqSlhlDLcQ==", "dba292ba-2a64-408e-baf6-7b312d0829c6" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_Code",
                table: "PaymentMethods",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_FlwRef",
                table: "PaymentTransactions",
                column: "FlwRef");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_PaymentId",
                table: "PaymentTransactions",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_TenantId",
                table: "PaymentTransactions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_TxRef",
                table: "PaymentTransactions",
                column: "TxRef");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Notifications");

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
    }
}
