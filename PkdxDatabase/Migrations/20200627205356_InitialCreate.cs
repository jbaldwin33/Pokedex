using Microsoft.EntityFrameworkCore.Migrations;

namespace PkdxDatabase.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PokedexEntries",
                columns: table => new
                {
                    ID = table.Column<float>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type1 = table.Column<string>(nullable: true),
                    Type2 = table.Column<string>(nullable: true),
                    Ability1 = table.Column<string>(nullable: true),
                    Ability2 = table.Column<string>(nullable: true),
                    HiddenAbility = table.Column<string>(nullable: true),
                    EggGroup1 = table.Column<string>(nullable: true),
                    EggGroup2 = table.Column<string>(nullable: true),
                    EvolveMethod = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokedexEntries", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokedexEntries");
        }
    }
}
