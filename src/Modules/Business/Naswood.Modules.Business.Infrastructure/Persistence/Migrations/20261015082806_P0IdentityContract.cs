using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Naswood.Modules.Business.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class P0IdentityContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_business_inventory_package_Barcode",
                schema: "business",
                table: "business_inventory_package");

            migrationBuilder.DropIndex(
                name: "IX_business_inventory_package_PackageNumber",
                schema: "business",
                table: "business_inventory_package");

            migrationBuilder.DropIndex(
                name: "UX_package_barcode_alive",
                schema: "business",
                table: "business_inventory_package");

            migrationBuilder.DropIndex(
                name: "UX_package_number_alive",
                schema: "business",
                table: "business_inventory_package");

            migrationBuilder.DropIndex(
                name: "UX_batch_production_lot_alive",
                schema: "business",
                table: "business_inventory_batch");

            migrationBuilder.CreateSequence(
                name: "identity_buc_auto",
                schema: "business");

            migrationBuilder.CreateSequence(
                name: "identity_buc_lam",
                schema: "business");

            migrationBuilder.CreateSequence(
                name: "identity_buc_lot",
                schema: "business");

            migrationBuilder.CreateSequence(
                name: "identity_buc_pkg",
                schema: "business");

            migrationBuilder.CreateSequence(
                name: "identity_buc_rest",
                schema: "business");

            migrationBuilder.CreateSequence(
                name: "identity_buc_trf",
                schema: "business");

            migrationBuilder.AlterColumn<Guid>(
                name: "MaterialId",
                schema: "business",
                table: "business_inventory_package",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BatchId",
                schema: "business",
                table: "business_inventory_package",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "UX_package_barcode",
                schema: "business",
                table: "business_inventory_package",
                column: "Barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_package_number",
                schema: "business",
                table: "business_inventory_package",
                column: "PackageNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_batch_number",
                schema: "business",
                table: "business_inventory_batch",
                column: "BatchNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_package_batch",
                schema: "business",
                table: "business_inventory_package",
                column: "BatchId",
                principalSchema: "business",
                principalTable: "business_inventory_batch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_package_material",
                schema: "business",
                table: "business_inventory_package",
                column: "MaterialId",
                principalSchema: "business",
                principalTable: "business_inventory_material",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_package_batch",
                schema: "business",
                table: "business_inventory_package");

            migrationBuilder.DropForeignKey(
                name: "FK_package_material",
                schema: "business",
                table: "business_inventory_package");

            migrationBuilder.DropIndex(
                name: "UX_package_barcode",
                schema: "business",
                table: "business_inventory_package");

            migrationBuilder.DropIndex(
                name: "UX_package_number",
                schema: "business",
                table: "business_inventory_package");

            migrationBuilder.DropIndex(
                name: "UX_batch_number",
                schema: "business",
                table: "business_inventory_batch");

            migrationBuilder.DropSequence(
                name: "identity_buc_auto",
                schema: "business");

            migrationBuilder.DropSequence(
                name: "identity_buc_lam",
                schema: "business");

            migrationBuilder.DropSequence(
                name: "identity_buc_lot",
                schema: "business");

            migrationBuilder.DropSequence(
                name: "identity_buc_pkg",
                schema: "business");

            migrationBuilder.DropSequence(
                name: "identity_buc_rest",
                schema: "business");

            migrationBuilder.DropSequence(
                name: "identity_buc_trf",
                schema: "business");

            migrationBuilder.AlterColumn<Guid>(
                name: "MaterialId",
                schema: "business",
                table: "business_inventory_package",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "BatchId",
                schema: "business",
                table: "business_inventory_package",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_package_Barcode",
                schema: "business",
                table: "business_inventory_package",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_package_PackageNumber",
                schema: "business",
                table: "business_inventory_package",
                column: "PackageNumber");

            migrationBuilder.CreateIndex(
                name: "UX_package_barcode_alive",
                schema: "business",
                table: "business_inventory_package",
                column: "Barcode",
                unique: true,
                filter: "\"IsDeleted\" = false AND \"Barcode\" <> ''");

            migrationBuilder.CreateIndex(
                name: "UX_package_number_alive",
                schema: "business",
                table: "business_inventory_package",
                column: "PackageNumber",
                unique: true,
                filter: "\"IsDeleted\" = false AND \"PackageNumber\" <> ''");

            migrationBuilder.CreateIndex(
                name: "UX_batch_production_lot_alive",
                schema: "business",
                table: "business_inventory_batch",
                column: "BatchNumber",
                unique: true,
                filter: "\"IsDeleted\" = false AND \"SourceType\" = 'PRODUCTION' AND \"BatchNumber\" <> ''");
        }
    }
}
