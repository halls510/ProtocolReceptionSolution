using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtocolConsumer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogErros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroProtocolo = table.Column<string>(type: "TEXT", nullable: false),
                    MotivoErro = table.Column<string>(type: "TEXT", nullable: false),
                    DataHora = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogErros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Protocolos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroProtocolo = table.Column<string>(type: "TEXT", nullable: false),
                    NumeroVia = table.Column<int>(type: "INTEGER", nullable: false),
                    Cpf = table.Column<string>(type: "TEXT", nullable: false),
                    Rg = table.Column<string>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    NomeMae = table.Column<string>(type: "TEXT", nullable: false),
                    NomePai = table.Column<string>(type: "TEXT", nullable: false),
                    Foto = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Protocolos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_cpf_numero_via",
                table: "Protocolos",
                columns: new[] { "Cpf", "NumeroVia" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_numero_protocolo",
                table: "Protocolos",
                column: "NumeroProtocolo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_rg_numero_via",
                table: "Protocolos",
                columns: new[] { "Rg", "NumeroVia" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogErros");

            migrationBuilder.DropTable(
                name: "Protocolos");
        }
    }
}
