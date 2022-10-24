﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(SmartMeterContext))]
    partial class SmartMeterContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Data.Models.Installation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("LocationId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RabbitMQExchange")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("RabbitMQPassword")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("RabbitMQUsername")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("RabbitMQVHost")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasAlternateKey("LocationId");

                    b.HasAlternateKey("Name");

                    b.ToTable("Installations");
                });

            modelBuilder.Entity("Data.Models.InstallationAccess", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("InstallationId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "InstallationId");

                    b.ToTable("InstallationsAccesses");
                });

            modelBuilder.Entity("Data.Models.MeterData", b =>
                {
                    b.Property<int>("InstallationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<decimal>("GasReadout")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("KwhInT1")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("KwhInT2")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("KwhOutT1")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("KwhOutT2")
                        .HasColumnType("decimal(65,30)");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime(6)");

                    b.HasKey("InstallationId");

                    b.HasAlternateKey("Time");

                    b.ToTable("MeterData");
                });

            modelBuilder.Entity("Data.Models.Permission", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("CanModifyInstallations")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("CanModifySettings")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("CanModifyUsers")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("RoleId");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("Data.Models.ReaderApiToken", b =>
                {
                    b.Property<string>("Token")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("InstallationId")
                        .HasColumnType("int");

                    b.HasKey("Token");

                    b.HasAlternateKey("InstallationId");

                    b.ToTable("ReaderApiTokens");
                });

            modelBuilder.Entity("Data.Models.RefreshToken", b =>
                {
                    b.Property<string>("Token")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Token");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("Data.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasAlternateKey("Name");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Data.Models.Setting", b =>
                {
                    b.Property<int>("Key")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Key");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("Data.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("varchar(60)");

                    b.HasKey("Id");

                    b.HasAlternateKey("Username");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
