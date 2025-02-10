﻿// <auto-generated />
using System;
using ButceYonet.Application.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ButceYonet.Application.Migrations
{
    [DbContext(typeof(ButceYonetDbContext))]
    partial class ButceYonetDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.Bank", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Bid")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreateUser")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ModifyUser")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Banks", (string)null);
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.BankAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BankId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreateUser")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ModifyUser")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BankId");

                    b.ToTable("BankAccounts", (string)null);
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreateUser")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsSymbolRight")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ModifyUser")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Currencies", (string)null);
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.DefaultLabel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ColorCode")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("varchar(16)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreateUser")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ModifyUser")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("DefaultLabels", (string)null);
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.Notebook", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreateUser")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ModifyUser")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Notebooks", (string)null);
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.NotebookLabel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ColorCode")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("varchar(16)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreateUser")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ModifyUser")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<int>("NotebookId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("NotebookId");

                    b.ToTable("NotebookLabels", (string)null);
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.NotebookUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreateUser")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ModifyUser")
                        .HasColumnType("longtext");

                    b.Property<int>("NotebookId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("NotebookId");

                    b.ToTable("NotebookUsers");
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.Plan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreateUser")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ModifyUser")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Plans", (string)null);
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.PlanFeature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreateUser")
                        .HasColumnType("longtext");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Feature")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ModifyUser")
                        .HasColumnType("longtext");

                    b.Property<int>("PlanId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("PlanId");

                    b.ToTable("PlanFeatures");
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("BankAccountId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreateUser")
                        .HasColumnType("longtext");

                    b.Property<int>("CurrencyId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsMatched")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsProceed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ModifyUser")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int?>("NotebookId")
                        .HasColumnType("int");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("BankAccountId");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("NotebookId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.TransactionLabel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreateUser")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ModifyUser")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<int>("NotebookLabelId")
                        .HasColumnType("int");

                    b.Property<int>("TransactionId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("NotebookLabelId");

                    b.HasIndex("TransactionId");

                    b.ToTable("TransactionLabels", (string)null);
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.UserPlan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreateUser")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("ExpirationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ModifyUser")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<int>("PlanId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PlanId");

                    b.ToTable("UserPlans", (string)null);
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.BankAccount", b =>
                {
                    b.HasOne("ButceYonet.Application.Domain.Entities.Bank", "Bank")
                        .WithMany("BankAccounts")
                        .HasForeignKey("BankId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Bank");
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.NotebookLabel", b =>
                {
                    b.HasOne("ButceYonet.Application.Domain.Entities.Notebook", "Notebook")
                        .WithMany("NotebookLabels")
                        .HasForeignKey("NotebookId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Notebook");
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.NotebookUser", b =>
                {
                    b.HasOne("ButceYonet.Application.Domain.Entities.Notebook", "Notebook")
                        .WithMany("NotebookUsers")
                        .HasForeignKey("NotebookId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Notebook");
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.PlanFeature", b =>
                {
                    b.HasOne("ButceYonet.Application.Domain.Entities.Plan", "Plan")
                        .WithMany("PlanFeatures")
                        .HasForeignKey("PlanId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Plan");
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.Transaction", b =>
                {
                    b.HasOne("ButceYonet.Application.Domain.Entities.BankAccount", "BankAccount")
                        .WithMany("Transactions")
                        .HasForeignKey("BankAccountId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("ButceYonet.Application.Domain.Entities.Currency", "Currency")
                        .WithMany("Transactions")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("ButceYonet.Application.Domain.Entities.Notebook", "Notebook")
                        .WithMany("Transactions")
                        .HasForeignKey("NotebookId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("BankAccount");

                    b.Navigation("Currency");

                    b.Navigation("Notebook");
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.TransactionLabel", b =>
                {
                    b.HasOne("ButceYonet.Application.Domain.Entities.NotebookLabel", "NotebookLabel")
                        .WithMany("TransactionLabels")
                        .HasForeignKey("NotebookLabelId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("ButceYonet.Application.Domain.Entities.Transaction", "Transaction")
                        .WithMany("TransactionLabels")
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("NotebookLabel");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.UserPlan", b =>
                {
                    b.HasOne("ButceYonet.Application.Domain.Entities.Plan", "Plan")
                        .WithMany("UserPlans")
                        .HasForeignKey("PlanId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Plan");
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.Bank", b =>
                {
                    b.Navigation("BankAccounts");
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.BankAccount", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.Currency", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.Notebook", b =>
                {
                    b.Navigation("NotebookLabels");

                    b.Navigation("NotebookUsers");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.NotebookLabel", b =>
                {
                    b.Navigation("TransactionLabels");
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.Plan", b =>
                {
                    b.Navigation("PlanFeatures");

                    b.Navigation("UserPlans");
                });

            modelBuilder.Entity("ButceYonet.Application.Domain.Entities.Transaction", b =>
                {
                    b.Navigation("TransactionLabels");
                });
#pragma warning restore 612, 618
        }
    }
}
