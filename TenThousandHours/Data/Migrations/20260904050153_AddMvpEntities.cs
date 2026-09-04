using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenThousandHours.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMvpEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WhatHappened = table.Column<string>(type: "TEXT", nullable: false),
                    GratefulFor = table.Column<string>(type: "TEXT", nullable: false),
                    WhatILearned = table.Column<string>(type: "TEXT", nullable: false),
                    WhatMadeTodayMeaningful = table.Column<string>(type: "TEXT", nullable: false),
                    WorkNotes = table.Column<string>(type: "TEXT", nullable: false),
                    LearningTopic = table.Column<string>(type: "TEXT", nullable: false),
                    IncomeSource = table.Column<string>(type: "TEXT", nullable: false),
                    IncomeAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    ContextWhere = table.Column<string>(type: "TEXT", nullable: false),
                    ContextWith = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DailyEntryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Hours = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityRecords_DailyEntries_DailyEntryId",
                        column: x => x.DailyEntryId,
                        principalTable: "DailyEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DailyEntryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    StorageProvider = table.Column<string>(type: "TEXT", nullable: false),
                    StorageKey = table.Column<string>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    YouTubeVideoId = table.Column<string>(type: "TEXT", nullable: true),
                    Caption = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaItems_DailyEntries_DailyEntryId",
                        column: x => x.DailyEntryId,
                        principalTable: "DailyEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityRecords_DailyEntryId",
                table: "ActivityRecords",
                column: "DailyEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyEntries_UserId_EntryDate",
                table: "DailyEntries",
                columns: new[] { "UserId", "EntryDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Goals_UserId_Title",
                table: "Goals",
                columns: new[] { "UserId", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_MediaItems_DailyEntryId",
                table: "MediaItems",
                column: "DailyEntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityRecords");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "MediaItems");

            migrationBuilder.DropTable(
                name: "DailyEntries");
        }
    }
}
