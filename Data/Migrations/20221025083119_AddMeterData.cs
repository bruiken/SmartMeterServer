﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class AddMeterData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeterData",
                columns: table => new
                {
                    InstallationId = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    KwhInT1 = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    KwhInT2 = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    KwhOutT1 = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    KwhOutT2 = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    GasReadout = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterData", x => new { x.InstallationId, x.Time });
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeterData");
        }
    }
}
