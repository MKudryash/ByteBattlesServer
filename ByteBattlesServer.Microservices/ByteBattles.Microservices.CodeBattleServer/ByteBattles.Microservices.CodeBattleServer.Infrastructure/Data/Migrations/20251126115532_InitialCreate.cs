using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByteBattles.Microservices.CodeBattleServer.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "battle_rooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_battle_rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "code_submissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProblemId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Result_IsSuccess = table.Column<bool>(type: "boolean", nullable: true),
                    Result_Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Result_TestsPassed = table.Column<int>(type: "integer", nullable: true),
                    Result_TotalTests = table.Column<int>(type: "integer", nullable: true),
                    Result_ExecutionTime = table.Column<TimeSpan>(type: "interval", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_code_submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_code_submissions_battle_rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "battle_rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_participants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_room_participants_battle_rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "battle_rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_code_submissions_RoomId",
                table: "code_submissions",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_room_participants_RoomId",
                table: "room_participants",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "code_submissions");

            migrationBuilder.DropTable(
                name: "room_participants");

            migrationBuilder.DropTable(
                name: "battle_rooms");
        }
    }
}
