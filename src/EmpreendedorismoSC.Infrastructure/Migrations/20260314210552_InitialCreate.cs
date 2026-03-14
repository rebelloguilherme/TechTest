using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmpreendedorismoSC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empreendimentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NomeEmpreendimento = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NomeEmpreendedor = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Municipio = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SegmentoAtuacao = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Contato = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empreendimentos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Empreendimentos",
                columns: new[] { "Id", "Ativo", "Contato", "DataAtualizacao", "DataCriacao", "Municipio", "NomeEmpreendedor", "NomeEmpreendimento", "SegmentoAtuacao" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), true, "marina@techfloripahub.com.br", null, new DateTime(2024, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Florianópolis", "Marina Costa", "Tech Floripa Hub", "Tecnologia" },
                    { new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"), true, "roberto@comerciodovale.com.br", null, new DateTime(2024, 2, 20, 14, 30, 0, 0, DateTimeKind.Utc), "Blumenau", "Roberto Schneider", "Comércio do Vale", "Comercio" },
                    { new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), true, "ana@joinvilletextil.com.br", null, new DateTime(2024, 3, 10, 9, 0, 0, 0, DateTimeKind.Utc), "Joinville", "Ana Müller", "Indústria Joinville Têxtil", "Industria" },
                    { new Guid("d4e5f6a7-b8c9-0123-defa-234567890123"), false, "pedro@consultoriaserra.com.br", null, new DateTime(2024, 4, 5, 16, 0, 0, 0, DateTimeKind.Utc), "Lages", "Pedro Souza", "Consultoria Serra Catarinense", "Servicos" },
                    { new Guid("e5f6a7b8-c9d0-1234-efab-345678901234"), true, "carla@agrooestesc.com.br", null, new DateTime(2024, 5, 12, 8, 0, 0, 0, DateTimeKind.Utc), "Chapecó", "Carla Fernandes", "Agro Oeste SC", "Agronegocio" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empreendimentos_Ativo",
                table: "Empreendimentos",
                column: "Ativo");

            migrationBuilder.CreateIndex(
                name: "IX_Empreendimentos_Municipio",
                table: "Empreendimentos",
                column: "Municipio");

            migrationBuilder.CreateIndex(
                name: "IX_Empreendimentos_SegmentoAtuacao",
                table: "Empreendimentos",
                column: "SegmentoAtuacao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Empreendimentos");
        }
    }
}
