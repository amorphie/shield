﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using amorphie.shield;

#nullable disable

namespace amorphie.shield.Migrations
{
    [DbContext(typeof(ShieldDbContext))]
    partial class ShieldDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("amorphie.shield.Certificates.Certificate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Cn")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CreatedByBehalfOf")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("InstanceId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ModifiedByBehalfOf")
                        .HasColumnType("uuid");

                    b.Property<string>("PublicCert")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("RevocationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SerialNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("StatusReason")
                        .HasColumnType("text");

                    b.Property<string>("ThumbPrint")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Certificates", (string)null);
                });

            modelBuilder.Entity("amorphie.shield.Transactions.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CertificateId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CreatedByBehalfOf")
                        .HasColumnType("uuid");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("InstanceId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ModifiedByBehalfOf")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RequestId")
                        .HasColumnType("uuid");

                    b.Property<string>("SignSignature")
                        .HasColumnType("text");

                    b.Property<DateTime?>("SignedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CertificateId");

                    b.ToTable("Transactions", (string)null);
                });

            modelBuilder.Entity("amorphie.shield.Certificates.Certificate", b =>
                {
                    b.OwnsOne("amorphie.shield.Shared.Identity", "Identity", b1 =>
                        {
                            b1.Property<Guid>("CertificateId")
                                .HasColumnType("uuid");

                            b1.Property<Guid>("DeviceId")
                                .HasColumnType("uuid")
                                .HasColumnName("DeviceId");

                            b1.Property<Guid>("RequestId")
                                .HasColumnType("uuid")
                                .HasColumnName("RequestId");

                            b1.Property<Guid>("TokenId")
                                .HasColumnType("uuid")
                                .HasColumnName("TokenId");

                            b1.Property<string>("UserTCKN")
                                .IsRequired()
                                .HasMaxLength(11)
                                .HasColumnType("character varying(11)")
                                .HasColumnName("UserTCKN");

                            b1.HasKey("CertificateId");

                            b1.ToTable("Certificates");

                            b1.WithOwner()
                                .HasForeignKey("CertificateId");
                        });

                    b.Navigation("Identity")
                        .IsRequired();
                });

            modelBuilder.Entity("amorphie.shield.Transactions.Transaction", b =>
                {
                    b.HasOne("amorphie.shield.Certificates.Certificate", null)
                        .WithMany()
                        .HasForeignKey("CertificateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("amorphie.shield.Transactions.TransactionActivity", "Activities", b1 =>
                        {
                            b1.Property<Guid>("TransactionId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<string>("Data")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<Guid>("RequestId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Status")
                                .HasColumnType("integer");

                            b1.HasKey("TransactionId", "Id");

                            b1.ToTable("TransactionActivities", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("TransactionId");
                        });

                    b.Navigation("Activities");
                });
#pragma warning restore 612, 618
        }
    }
}
