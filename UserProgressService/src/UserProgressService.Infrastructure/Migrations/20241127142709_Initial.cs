using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserProgressService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_issue_progress",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_activity_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_issue_progress", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_progress",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    experience = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_progress", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "issue_progress",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    module_id = table.Column<Guid>(type: "uuid", nullable: true),
                    issue_id = table.Column<Guid>(type: "uuid", nullable: false),
                    try_count = table.Column<int>(type: "integer", nullable: false),
                    started_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    execution_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    user_issue_progress_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issue_progress", x => x.id);
                    table.ForeignKey(
                        name: "fk_issue_progress_user_issue_progress_user_issue_progress_id",
                        column: x => x.user_issue_progress_id,
                        principalTable: "user_issue_progress",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "issue_achievement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_date = table.Column<DateOnly>(type: "date", nullable: false),
                    user_progress_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issue_achievement", x => x.id);
                    table.ForeignKey(
                        name: "fk_issue_achievement_user_progress_user_progress_id",
                        column: x => x.user_progress_id,
                        principalTable: "user_progress",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "issues_achievement_conditions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    attempts = table.Column<int>(type: "integer", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false),
                    time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    user_activity_date = table.Column<DateOnly>(type: "date", nullable: false),
                    difficulty = table.Column<string>(type: "text", nullable: false),
                    issue_achievement_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issues_achievement_conditions", x => x.id);
                    table.ForeignKey(
                        name: "fk_issues_achievement_conditions_issue_achievement_issue_achie",
                        column: x => x.issue_achievement_id,
                        principalTable: "issue_achievement",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_issue_achievement_user_progress_id",
                table: "issue_achievement",
                column: "user_progress_id");

            migrationBuilder.CreateIndex(
                name: "ix_issue_progress_user_issue_progress_id",
                table: "issue_progress",
                column: "user_issue_progress_id");

            migrationBuilder.CreateIndex(
                name: "ix_issues_achievement_conditions_issue_achievement_id",
                table: "issues_achievement_conditions",
                column: "issue_achievement_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "issue_progress");

            migrationBuilder.DropTable(
                name: "issues_achievement_conditions");

            migrationBuilder.DropTable(
                name: "user_issue_progress");

            migrationBuilder.DropTable(
                name: "issue_achievement");

            migrationBuilder.DropTable(
                name: "user_progress");
        }
    }
}
