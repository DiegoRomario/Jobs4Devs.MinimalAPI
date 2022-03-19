﻿// <auto-generated />
using System;
using Jobs4Devs.MinimalAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Jobs4Devs.MinimalAPI.Migrations
{
    [DbContext(typeof(APIDBContext))]
    partial class APIDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Jobs4Devs.MinimalAPI.Models.Vacancy", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Company")
                        .IsRequired()
                        .HasColumnType("varchar(120)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValue(new DateTime(2022, 3, 19, 10, 8, 44, 719, DateTimeKind.Local).AddTicks(5200));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(1024)");

                    b.Property<bool>("IsOpen")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<double>("MaxSalary")
                        .HasColumnType("float");

                    b.Property<double>("MinSalary")
                        .HasColumnType("float");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("varchar(240)");

                    b.HasKey("Id");

                    b.ToTable("Vacancies", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
