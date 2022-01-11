using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PkdxDatabase.Migrations
{
    public partial class newid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "PokedexEntries",
                newName: "Id");

            //migrationBuilder.AlterColumn<string>(
            //    name: "Type2",
            //    table: "PokedexEntries",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Type1",
            //    table: "PokedexEntries",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Name",
            //    table: "PokedexEntries",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "HiddenAbility",
            //    table: "PokedexEntries",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "EvolveMethod",
            //    table: "PokedexEntries",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<string>(
            //    name: "EggGroup2",
            //    table: "PokedexEntries",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "EggGroup1",
            //    table: "PokedexEntries",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Ability2",
            //    table: "PokedexEntries",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Ability1",
            //    table: "PokedexEntries",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "Id",
            //    table: "PokedexEntries",
            //    nullable: false,
            //    oldClrType: typeof(float),
            //    oldType: "real");

            migrationBuilder.AddColumn<float>(
                name: "Num",
                table: "PokedexEntries",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "Num",
            //    table: "PokedexEntries");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PokedexEntries",
                newName: "ID");

            //migrationBuilder.AlterColumn<string>(
            //    name: "Type2",
            //    table: "PokedexEntries",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Type1",
            //    table: "PokedexEntries",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Name",
            //    table: "PokedexEntries",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "HiddenAbility",
            //    table: "PokedexEntries",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "EvolveMethod",
            //    table: "PokedexEntries",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(int));

            //migrationBuilder.AlterColumn<string>(
            //    name: "EggGroup2",
            //    table: "PokedexEntries",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "EggGroup1",
            //    table: "PokedexEntries",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Ability2",
            //    table: "PokedexEntries",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Ability1",
            //    table: "PokedexEntries",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<float>(
            //    name: "ID",
            //    table: "PokedexEntries",
            //    type: "real",
            //    nullable: false,
            //    oldClrType: typeof(Guid));
        }
    }
}
