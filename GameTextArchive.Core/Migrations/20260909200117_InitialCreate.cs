using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace GameTextArchive.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lexemes",
                columns: table => new
                {
                    lexemeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    Frequency = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lexemes", x => x.lexemeId);
                });

            migrationBuilder.CreateTable(
                name: "TextRecords",
                columns: table => new
                {
                    recordId = table.Column<Guid>(type: "uuid", nullable: false),
                    EditorId = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    SpeakerId = table.Column<string>(type: "text", nullable: true),
                    Text = table.Column<string>(type: "text", nullable: true),
                    SourceFile = table.Column<string>(type: "text", nullable: false),
                    ImportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Metadata = table.Column<Dictionary<string, JsonElement>>(type: "jsonb", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: true)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "Text" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextRecords", x => x.recordId);
                });

            migrationBuilder.CreateTable(
                name: "TextRecordLexemes",
                columns: table => new
                {
                    recordId = table.Column<Guid>(type: "uuid", nullable: false),
                    lexemeId = table.Column<Guid>(type: "uuid", nullable: false),
                    frequency = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextRecordLexemes", x => new { x.recordId, x.lexemeId });
                    table.ForeignKey(
                        name: "FK_TextRecordLexemes_Lexemes_lexemeId",
                        column: x => x.lexemeId,
                        principalTable: "Lexemes",
                        principalColumn: "lexemeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TextRecordLexemes_TextRecords_recordId",
                        column: x => x.recordId,
                        principalTable: "TextRecords",
                        principalColumn: "recordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lexemes_Value",
                table: "Lexemes",
                column: "Value",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TextRecordLexemes_lexemeId",
                table: "TextRecordLexemes",
                column: "lexemeId");

            migrationBuilder.CreateIndex(
                name: "IX_TextRecords_SearchVector",
                table: "TextRecords",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TextRecordLexemes");

            migrationBuilder.DropTable(
                name: "Lexemes");

            migrationBuilder.DropTable(
                name: "TextRecords");
        }
    }
}
