using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "achievements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    icon_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    required_value = table.Column<int>(type: "integer", nullable: false),
                    reward_experience = table.Column<int>(type: "integer", nullable: false),
                    is_secret = table.Column<bool>(type: "boolean", nullable: false),
                    category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    rarity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    unlock_message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_achievements", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    avatar_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    bio = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    github_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    linkedin_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "battle_results",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    battle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    opponent_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    result = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    experience_gained = table.Column<int>(type: "integer", nullable: false),
                    problems_solved = table.Column<Guid>(type: "uuid", nullable: false),
                    completion_time = table.Column<long>(type: "bigint", nullable: false),
                    battle_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_battle_results", x => x.id);
                    table.ForeignKey(
                        name: "FK_battle_results_user_profiles_user_profile_id",
                        column: x => x.user_profile_id,
                        principalTable: "user_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recent_activities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    experience_gained = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recent_activities", x => x.id);
                    table.ForeignKey(
                        name: "FK_recent_activities_user_profiles_user_profile_id",
                        column: x => x.user_profile_id,
                        principalTable: "user_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recent_problems",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    problem_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    difficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    solved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    language = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recent_problems", x => x.id);
                    table.ForeignKey(
                        name: "FK_recent_problems_user_profiles_user_profile_id",
                        column: x => x.user_profile_id,
                        principalTable: "user_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "teacher_stats",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_tasks = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    active_students = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    average_rating = table.Column<double>(type: "double precision", precision: 3, scale: 2, nullable: false, defaultValue: 0.0),
                    total_submissions = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teacher_stats", x => x.id);
                    table.ForeignKey(
                        name: "FK_teacher_stats_user_profiles_user_profile_id",
                        column: x => x.user_profile_id,
                        principalTable: "user_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_achievements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    achievement_id = table.Column<Guid>(type: "uuid", nullable: false),
                    unlocked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    progress = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    is_unlocked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_achievements", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_achievements_achievements",
                        column: x => x.achievement_id,
                        principalTable: "achievements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_achievements_user_profiles",
                        column: x => x.user_profile_id,
                        principalTable: "user_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_settings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email_notifications = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    battle_invitations = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    achievement_notifications = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    theme = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "light"),
                    code_editor_theme = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "vs-light"),
                    preferred_language = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "csharp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_settings", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_settings_user_profiles_user_profile_id",
                        column: x => x.user_profile_id,
                        principalTable: "user_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_stats",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_problems_solved = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    total_battles = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    wins = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    losses = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    draws = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    current_streak = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    max_streak = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    total_experience = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    easy_problems_solved = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    medium_problems_solved = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    hard_problems_solved = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    total_submissions = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    successful_submissions = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    total_execution_time = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    SolvedTaskIds = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_stats", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_stats_user_profiles_user_profile_id",
                        column: x => x.user_profile_id,
                        principalTable: "user_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "achievements",
                columns: new[] { "id", "category", "description", "icon_url", "is_secret", "name", "rarity", "required_value", "reward_experience", "type", "unlock_message" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Problems", "Решите свою первую задачу", "/achievements/first-blood.png", false, "Первая кровь", "Common", 1, 100, "TotalProblemsSolved", "Отличный старт! Первая задача решена!" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Problems", "Решите 10 задач", "/achievements/solver.png", false, "Решатель", "Common", 10, 250, "TotalProblemsSolved", null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Battles", "Выиграйте первый баттл", "/achievements/first-victory.png", false, "Первая победа", "Common", 1, 200, "Wins", "Поздравляем с первой победой в баттле!" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Streaks", "Выиграйте 10 баттлов подряд", "/achievements/invincible.png", false, "Непобедимый", "Epic", 10, 1500, "CurrentStreak", null },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Problems", "Решите 100 задач", "/achievements/algorithm-master.png", false, "Мастер алгоритмов", "Rare", 100, 1000, "TotalProblemsSolved", null },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "Special", "Решите задачу за 10 секунд", "/achievements/code-ninja.png", true, "Ниндзя кода", "Legendary", 10, 2000, "FastestSubmission", "Невероятная скорость! Вы настоящий ниндзя кода!" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_achievements_category",
                table: "achievements",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "ix_achievements_rarity",
                table: "achievements",
                column: "rarity");

            migrationBuilder.CreateIndex(
                name: "ix_achievements_required_value",
                table: "achievements",
                column: "required_value");

            migrationBuilder.CreateIndex(
                name: "ix_achievements_type",
                table: "achievements",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_battle_results_battle_date",
                table: "battle_results",
                column: "battle_date");

            migrationBuilder.CreateIndex(
                name: "IX_battle_results_battle_id",
                table: "battle_results",
                column: "battle_id");

            migrationBuilder.CreateIndex(
                name: "IX_battle_results_user_profile_id",
                table: "battle_results",
                column: "user_profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_recent_activities_created_at",
                table: "recent_activities",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_recent_activities_type",
                table: "recent_activities",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_recent_activities_user_profile_id",
                table: "recent_activities",
                column: "user_profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_recent_problems_difficulty",
                table: "recent_problems",
                column: "difficulty");

            migrationBuilder.CreateIndex(
                name: "IX_recent_problems_problem_id",
                table: "recent_problems",
                column: "problem_id");

            migrationBuilder.CreateIndex(
                name: "IX_recent_problems_solved_at",
                table: "recent_problems",
                column: "solved_at");

            migrationBuilder.CreateIndex(
                name: "IX_recent_problems_user_profile_id",
                table: "recent_problems",
                column: "user_profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_teacher_stats_user_profile_id",
                table: "teacher_stats",
                column: "user_profile_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_achievements_achievement",
                table: "user_achievements",
                column: "achievement_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_achievements_is_unlocked",
                table: "user_achievements",
                column: "is_unlocked");

            migrationBuilder.CreateIndex(
                name: "ix_user_achievements_unlocked_at",
                table: "user_achievements",
                column: "unlocked_at",
                filter: "unlocked_at IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_user_achievements_user_profile",
                table: "user_achievements",
                column: "user_profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_achievements_user_profile_achievement",
                table: "user_achievements",
                columns: new[] { "user_profile_id", "achievement_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_created_at",
                table: "user_profiles",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_email",
                table: "user_profiles",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_is_public",
                table: "user_profiles",
                column: "is_public");

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_level",
                table: "user_profiles",
                column: "level");

            migrationBuilder.CreateIndex(
                name: "ix_user_profiles_role_teacher",
                table: "user_profiles",
                column: "Role",
                filter: "\"Role\" = 'Teacher'");

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_updated_at",
                table: "user_profiles",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_user_id",
                table: "user_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_user_name",
                table: "user_profiles",
                column: "user_name");

            migrationBuilder.CreateIndex(
                name: "IX_user_settings_user_profile_id",
                table: "user_settings",
                column: "user_profile_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_stats_user_profile_id",
                table: "user_stats",
                column: "user_profile_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "battle_results");

            migrationBuilder.DropTable(
                name: "recent_activities");

            migrationBuilder.DropTable(
                name: "recent_problems");

            migrationBuilder.DropTable(
                name: "teacher_stats");

            migrationBuilder.DropTable(
                name: "user_achievements");

            migrationBuilder.DropTable(
                name: "user_settings");

            migrationBuilder.DropTable(
                name: "user_stats");

            migrationBuilder.DropTable(
                name: "achievements");

            migrationBuilder.DropTable(
                name: "user_profiles");
        }
    }
}
