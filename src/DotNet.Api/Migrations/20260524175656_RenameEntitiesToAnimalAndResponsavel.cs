using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNet.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameEntitiesToAnimalAndResponsavel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_CP_CARE_EVENTS_T_CP_PETS_PET_ID",
                table: "T_CP_CARE_EVENTS");

            migrationBuilder.DropTable(
                name: "T_CP_PETS");

            migrationBuilder.DropTable(
                name: "T_CP_TUTORS");

            migrationBuilder.CreateTable(
                name: "T_CP_RESPONSAVEIS",
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
                    table.PrimaryKey("PK_T_CP_RESPONSAVEIS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "T_CP_ANIMAIS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    RESPONSAVEL_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
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
                    table.PrimaryKey("PK_T_CP_ANIMAIS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_T_CP_ANIMAIS_T_CP_RESPONSAVEIS_RESPONSAVEL_ID",
                        column: x => x.RESPONSAVEL_ID,
                        principalTable: "T_CP_RESPONSAVEIS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_CP_ANIMAIS_RESPONSAVEL_ID",
                table: "T_CP_ANIMAIS",
                column: "RESPONSAVEL_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_T_CP_CARE_EVENTS_T_CP_ANIMAIS_PET_ID",
                table: "T_CP_CARE_EVENTS",
                column: "PET_ID",
                principalTable: "T_CP_ANIMAIS",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_CP_CARE_EVENTS_T_CP_ANIMAIS_PET_ID",
                table: "T_CP_CARE_EVENTS");

            migrationBuilder.DropTable(
                name: "T_CP_ANIMAIS");

            migrationBuilder.DropTable(
                name: "T_CP_RESPONSAVEIS");

            migrationBuilder.CreateTable(
                name: "T_CP_TUTORS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CPF = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(160)", maxLength: 160, nullable: false),
                    IS_ACTIVE = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    PHONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_CP_TUTORS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "T_CP_PETS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    BIRTH_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    BREED = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    IS_ACTIVE = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    NICKNAME = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    RGA = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    SEX = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    SPECIES = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    TUTOR_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    WEIGHT = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_CP_PETS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_T_CP_PETS_T_CP_TUTORS_TUTOR_ID",
                        column: x => x.TUTOR_ID,
                        principalTable: "T_CP_TUTORS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_CP_PETS_TUTOR_ID",
                table: "T_CP_PETS",
                column: "TUTOR_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_T_CP_CARE_EVENTS_T_CP_PETS_PET_ID",
                table: "T_CP_CARE_EVENTS",
                column: "PET_ID",
                principalTable: "T_CP_PETS",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
