using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByteBattlesServer.Microservices.TaskServices.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ShortTitle = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    FileExtension = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CompilerCommand = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ExecutionCommand = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SupportsCompilation = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PatternFunction = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PatternMain = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Difficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Author = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FunctionName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PatternFunction = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PatternMain = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Parameters = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ReturnType = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TotalAttempts = table.Column<int>(type: "integer", nullable: false),
                    SuccessfulAttempts = table.Column<int>(type: "integer", nullable: false),
                    AverageExecutionTime = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Libraries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameLibrary = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Version = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LanguageId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libraries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Libraries_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskLanguages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdTask = table.Column<Guid>(type: "uuid", nullable: false),
                    IdLanguage = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskLanguages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskLanguages_Languages_IdLanguage",
                        column: x => x.IdLanguage,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskLanguages_Tasks_IdTask",
                        column: x => x.IdTask,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestsTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Input = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ExpectedOutput = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    IsExample = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestsTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestsTasks_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskLibraries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdTask = table.Column<Guid>(type: "uuid", nullable: false),
                    IdLibrary = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskLibraries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskLibraries_Libraries_IdLibrary",
                        column: x => x.IdLibrary,
                        principalTable: "Libraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskLibraries_Tasks_IdTask",
                        column: x => x.IdTask,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CreatedAd",
                table: "Languages",
                column: "CreatedAd");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_ShortTitle",
                table: "Languages",
                column: "ShortTitle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Title",
                table: "Languages",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Libraries_CreatedAd",
                table: "Libraries",
                column: "CreatedAd");

            migrationBuilder.CreateIndex(
                name: "IX_Libraries_LanguageId",
                table: "Libraries",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskLanguages_IdLanguage",
                table: "TaskLanguages",
                column: "IdLanguage");

            migrationBuilder.CreateIndex(
                name: "IX_TaskLanguages_IdTask_IdLanguage",
                table: "TaskLanguages",
                columns: new[] { "IdTask", "IdLanguage" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskLibraries_IdLibrary",
                table: "TaskLibraries",
                column: "IdLibrary");

            migrationBuilder.CreateIndex(
                name: "IX_TaskLibraries_IdTask",
                table: "TaskLibraries",
                column: "IdTask");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Author",
                table: "Tasks",
                column: "Author");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedAt",
                table: "Tasks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Difficulty",
                table: "Tasks",
                column: "Difficulty");

            migrationBuilder.CreateIndex(
                name: "IX_TestsTasks_TaskId",
                table: "TestsTasks",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskLanguages");

            migrationBuilder.DropTable(
                name: "TaskLibraries");

            migrationBuilder.DropTable(
                name: "TestsTasks");

            migrationBuilder.DropTable(
                name: "Libraries");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}
