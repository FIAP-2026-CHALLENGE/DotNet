using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNet.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_T_CP_PETS_TUTOR_ID",
                table: "T_CP_PETS",
                column: "TUTOR_ID");

            migrationBuilder.CreateIndex(
                name: "IX_T_CP_CARE_EVENTS_PET_ID",
                table: "T_CP_CARE_EVENTS",
                column: "PET_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_T_CP_CARE_EVENTS_T_CP_PETS_PET_ID",
                table: "T_CP_CARE_EVENTS",
                column: "PET_ID",
                principalTable: "T_CP_PETS",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_CP_PETS_T_CP_TUTORS_TUTOR_ID",
                table: "T_CP_PETS",
                column: "TUTOR_ID",
                principalTable: "T_CP_TUTORS",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_CP_CARE_EVENTS_T_CP_PETS_PET_ID",
                table: "T_CP_CARE_EVENTS");

            migrationBuilder.DropForeignKey(
                name: "FK_T_CP_PETS_T_CP_TUTORS_TUTOR_ID",
                table: "T_CP_PETS");

            migrationBuilder.DropIndex(
                name: "IX_T_CP_PETS_TUTOR_ID",
                table: "T_CP_PETS");

            migrationBuilder.DropIndex(
                name: "IX_T_CP_CARE_EVENTS_PET_ID",
                table: "T_CP_CARE_EVENTS");
        }
    }
}
