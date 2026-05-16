using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNet.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_CP_CARE_EVENTS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PET_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TYPE = table.Column<string>(type: "NVARCHAR2(40)", maxLength: 40, nullable: false),
                    TITLE = table.Column<string>(type: "NVARCHAR2(160)", maxLength: 160, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    SCHEDULED_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    COMPLETED_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    STATUS = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    PRIORITY = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    NOTES = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    IS_ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_CP_CARE_EVENTS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "T_CP_PETS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TUTOR_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    NICKNAME = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    SPECIES = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    BREED = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    BIRTH_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    WEIGHT = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    SEX = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    RGA = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    IS_ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_CP_PETS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "T_CP_TUTORS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAME = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(160)", maxLength: 160, nullable: false),
                    PHONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    CPF = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    IS_ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_CP_TUTORS", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_CP_CARE_EVENTS");

            migrationBuilder.DropTable(
                name: "T_CP_PETS");

            migrationBuilder.DropTable(
                name: "T_CP_TUTORS");
        }
    }
}
