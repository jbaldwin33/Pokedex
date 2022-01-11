﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pokedex.PkdxDatabase.Context;

namespace PkdxDatabase.Migrations
{
    [DbContext(typeof(PokedexDBContext))]
    [Migration("20220111151547_newid")]
    partial class newid
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5");

            modelBuilder.Entity("Pokedex.PkdxDatabase.Models.PokedexClass", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Ability1")
                        .HasColumnType("TEXT");

                    b.Property<string>("Ability2")
                        .HasColumnType("TEXT");

                    b.Property<string>("EggGroup1")
                        .HasColumnType("TEXT");

                    b.Property<string>("EggGroup2")
                        .HasColumnType("TEXT");

                    b.Property<int>("EvolveMethod")
                        .HasColumnType("INTEGER");

                    b.Property<string>("HiddenAbility")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<float>("Num")
                        .HasColumnType("REAL");

                    b.Property<string>("Type1")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type2")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("PokedexEntries");
                });
#pragma warning restore 612, 618
        }
    }
}