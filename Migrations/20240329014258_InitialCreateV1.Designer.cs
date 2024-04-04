﻿// <auto-generated />
using AccountManagermnet.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AccountManagermnet.Migrations
{
    [DbContext(typeof(AccountDbContext))]
    [Migration("20240329014258_InitialCreateV1")]
    partial class InitialCreateV1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AccountManagermnet.DTO.AccountCategory", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BankAccount")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("BankName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ParentId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("AccountCategorys");
                });

            modelBuilder.Entity("AccountManagermnet.Domain.GoodsReceivedNote", b =>
                {
                    b.Property<string>("GRNId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Detail")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("DocumentDay")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("DocumentNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PersonID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("GRNId");

                    b.HasIndex("PersonID")
                        .IsUnique();

                    b.ToTable("GoodsReceivedNotes");
                });

            modelBuilder.Entity("AccountManagermnet.Domain.GoodsReceivedNoteDetail", b =>
                {
                    b.Property<string>("GRNDId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("CreditAccount")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("DebitAccount")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("GRN_Id")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("UnitPirce")
                        .HasColumnType("int");

                    b.Property<string>("WarehousId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("GRNDId");

                    b.HasIndex("GRN_Id");

                    b.HasIndex("ProductId");

                    b.ToTable("GoodsReceivedNoteDetails");
                });

            modelBuilder.Entity("AccountManagermnet.Domain.Person", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("TaxCode")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Persons");
                });

            modelBuilder.Entity("AccountManagermnet.Domain.ProductCategory", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("GOGSAcc")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("GoodsAcc")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("RevenueAcc")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("AccountManagermnet.Domain.GoodsReceivedNote", b =>
                {
                    b.HasOne("AccountManagermnet.Domain.Person", "Person")
                        .WithOne("GoodsReceivedNote")
                        .HasForeignKey("AccountManagermnet.Domain.GoodsReceivedNote", "PersonID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Person");
                });

            modelBuilder.Entity("AccountManagermnet.Domain.GoodsReceivedNoteDetail", b =>
                {
                    b.HasOne("AccountManagermnet.Domain.GoodsReceivedNote", "GoodsReceivedNote")
                        .WithMany("GoodsReceivedNoteDetails")
                        .HasForeignKey("GRN_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AccountManagermnet.Domain.ProductCategory", "ProductCategorys")
                        .WithMany("GoodsReceivedNoteDetails")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GoodsReceivedNote");

                    b.Navigation("ProductCategorys");
                });

            modelBuilder.Entity("AccountManagermnet.Domain.GoodsReceivedNote", b =>
                {
                    b.Navigation("GoodsReceivedNoteDetails");
                });

            modelBuilder.Entity("AccountManagermnet.Domain.Person", b =>
                {
                    b.Navigation("GoodsReceivedNote")
                        .IsRequired();
                });

            modelBuilder.Entity("AccountManagermnet.Domain.ProductCategory", b =>
                {
                    b.Navigation("GoodsReceivedNoteDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
