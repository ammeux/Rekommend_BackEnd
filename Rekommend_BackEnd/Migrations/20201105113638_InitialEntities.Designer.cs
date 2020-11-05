﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Rekommend_BackEnd.DbContexts;

namespace Rekommend_BackEnd.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20201105113638_InitialEntities")]
    partial class InitialEntities
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Rekommend_BackEnd.Entities.Company", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Category")
                        .HasColumnType("integer")
                        .HasMaxLength(50);

                    b.Property<string>("CompanyDescription")
                        .IsRequired()
                        .HasColumnType("character varying(1500)")
                        .HasMaxLength(1500);

                    b.Property<string>("HqCity")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<DateTimeOffset>("RegistrationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Website")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<string>("logoFileName")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("Rekommend_BackEnd.Entities.Recruiter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<Guid>("CompanyId")
                        .HasColumnType("uuid");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<DateTimeOffset>("DateOfBirth")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<int>("Position")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("RegistrationDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Recruiters");
                });

            modelBuilder.Entity("Rekommend_BackEnd.Entities.RekoHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("RekommendationId")
                        .HasColumnType("uuid");

                    b.Property<int>("RekommendationStatus")
                        .HasColumnType("integer");

                    b.Property<Guid>("RekommenderId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RekommendationId");

                    b.HasIndex("RekommenderId");

                    b.ToTable("RekoHistories");
                });

            modelBuilder.Entity("Rekommend_BackEnd.Entities.Rekommendation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("character varying(1500)")
                        .HasMaxLength(1500);

                    b.Property<string>("Company")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<DateTimeOffset>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<int>("Position")
                        .HasColumnType("integer");

                    b.Property<Guid>("RekommenderId")
                        .HasColumnType("uuid");

                    b.Property<int>("Seniority")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("StatusChangeDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("RekommenderId");

                    b.ToTable("Rekommendations");
                });

            modelBuilder.Entity("Rekommend_BackEnd.Entities.Rekommender", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Company")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<DateTimeOffset>("DateOfBirth")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<int>("Position")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("RegistrationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Seniority")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Rekommenders");
                });

            modelBuilder.Entity("Rekommend_BackEnd.Entities.TechJobOpening", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<DateTimeOffset>("ClosingDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ContractType")
                        .HasColumnType("integer");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<DateTimeOffset>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("JobPosition")
                        .HasColumnType("integer");

                    b.Property<int>("JobTechLanguage")
                        .HasColumnType("integer");

                    b.Property<int>("LikesNb")
                        .HasColumnType("integer");

                    b.Property<int>("MaximumSalary")
                        .HasColumnType("integer");

                    b.Property<int>("MinimumSalary")
                        .HasColumnType("integer");

                    b.Property<string>("MissionDescription")
                        .IsRequired()
                        .HasColumnType("character varying(1500)")
                        .HasMaxLength(1500);

                    b.Property<Guid>("RecruiterId")
                        .HasColumnType("uuid");

                    b.Property<int>("RekommendationsNb")
                        .HasColumnType("integer");

                    b.Property<bool>("RemoteWorkAccepted")
                        .HasColumnType("boolean");

                    b.Property<string>("Reward1")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Reward2")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Reward3")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<string>("RseDescription")
                        .HasColumnType("character varying(1500)")
                        .HasMaxLength(1500);

                    b.Property<int>("Seniority")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("StartingDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<int>("ViewNb")
                        .HasColumnType("integer");

                    b.Property<string>("pictureFileName")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("RecruiterId");

                    b.ToTable("TechJobOpenings");
                });

            modelBuilder.Entity("Rekommend_BackEnd.Entities.Recruiter", b =>
                {
                    b.HasOne("Rekommend_BackEnd.Entities.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Rekommend_BackEnd.Entities.RekoHistory", b =>
                {
                    b.HasOne("Rekommend_BackEnd.Entities.Rekommendation", "Rekommendation")
                        .WithMany()
                        .HasForeignKey("RekommendationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Rekommend_BackEnd.Entities.Rekommender", "Rekommender")
                        .WithMany()
                        .HasForeignKey("RekommenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Rekommend_BackEnd.Entities.Rekommendation", b =>
                {
                    b.HasOne("Rekommend_BackEnd.Entities.Rekommender", "Rekommender")
                        .WithMany()
                        .HasForeignKey("RekommenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Rekommend_BackEnd.Entities.TechJobOpening", b =>
                {
                    b.HasOne("Rekommend_BackEnd.Entities.Recruiter", "Recruiter")
                        .WithMany()
                        .HasForeignKey("RecruiterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
