using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

#nullable disable

namespace GameTextArchive.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TextRecords",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<int>(type: "integer", nullable: false),
                    speaker_id = table.Column<string>(type: "text", nullable: true),
                    text = table.Column<string>(type: "text", nullable: true),
                    SourceFile = table.Column<string>(type: "text", nullable: false),
                    ImportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Metadata = table.Column<Dictionary<string, JsonElement>>(type: "jsonb", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextRecords", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TextRecords");
        }
    }
}
