﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tippy.Core.Data;

namespace Tippy.Core.Migrations
{
    [DbContext(typeof(TippyDbContext))]
    partial class TippyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Tippy.Core.Models.Contracts", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("chainid")
                        .HasColumnType("INTEGER");

                    b.Property<int>("filename")
                        .HasColumnType("INTEGER");

                    b.Property<int>("filepath")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Contracts");
                });

            modelBuilder.Entity("Tippy.Core.Models.DeniedTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("DenyType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TxHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("TxHash", "DenyType")
                        .IsUnique();

                    b.ToTable("DeniedTransactions");
                });

            modelBuilder.Entity("Tippy.Core.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Chain")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ExtraToml")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("IndexerRpcPort")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(false);

                    b.Property<string>("LockArg")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("NodeNetworkPort")
                        .HasColumnType("INTEGER");

                    b.Property<int>("NodeRpcPort")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Tippy.Core.Models.RecordedTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Error")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RawTransaction")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TxHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("RecordedTransactions");
                });

            modelBuilder.Entity("Tippy.Core.Models.Token", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Decimals")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TypeScriptArgs")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TypeScriptCodeHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TypeScriptHashType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("Tippy.Core.Models.Contracts", b =>
                {
                    b.HasOne("Tippy.Core.Models.Project", null)
                        .WithMany("contracts")
                        .HasForeignKey("ProjectId");
                });

            modelBuilder.Entity("Tippy.Core.Models.DeniedTransaction", b =>
                {
                    b.HasOne("Tippy.Core.Models.Project", "Project")
                        .WithMany("DeniedTransactions")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Tippy.Core.Models.RecordedTransaction", b =>
                {
                    b.HasOne("Tippy.Core.Models.Project", "Project")
                        .WithMany("RecordedTransactions")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Tippy.Core.Models.Token", b =>
                {
                    b.HasOne("Tippy.Core.Models.Project", "Project")
                        .WithMany("Tokens")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Tippy.Core.Models.Project", b =>
                {
                    b.Navigation("contracts");

                    b.Navigation("DeniedTransactions");

                    b.Navigation("RecordedTransactions");

                    b.Navigation("Tokens");
                });
#pragma warning restore 612, 618
        }
    }
}
