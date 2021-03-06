﻿// <auto-generated />
using System;
using Esquire.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace esquirebackend.Migrations
{
    [DbContext(typeof(ProjectContext))]
    [Migration("20190702161255_ProjectStatus")]
    partial class ProjectStatus
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Esquire.Models.CodedAnswer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Annotations")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("[]");

                    b.Property<long?>("CodedQuestionBaseId");

                    b.Property<string>("Pincite");

                    b.Property<long>("SchemeAnswerId");

                    b.Property<string>("TextAnswer");

                    b.HasKey("Id");

                    b.HasIndex("SchemeAnswerId");

                    b.HasIndex("CodedQuestionBaseId", "SchemeAnswerId")
                        .IsUnique()
                        .HasFilter("[CodedQuestionBaseId] IS NOT NULL");

                    b.ToTable("CodedAnswers");
                });

            modelBuilder.Entity("Esquire.Models.CodedQuestionBase", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<long>("SchemeQuestionId");

                    b.HasKey("Id");

                    b.HasIndex("SchemeQuestionId");

                    b.ToTable("CodedQuestionBases");

                    b.HasDiscriminator<string>("Discriminator").HasValue("CodedQuestionBase");
                });

            modelBuilder.Entity("Esquire.Models.FlagBase", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Notes");

                    b.Property<DateTime>("RaisedAt");

                    b.Property<long?>("RaisedById");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("RaisedById");

                    b.ToTable("FlagBases");

                    b.HasDiscriminator<string>("Discriminator").HasValue("FlagBase");
                });

            modelBuilder.Entity("Esquire.Models.Jurisdiction", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FipsCode");

                    b.Property<string>("GnisCode");

                    b.Property<string>("GnisFipsConcatenation");

                    b.Property<string>("Name")
                        .HasMaxLength(450);

                    b.Property<string>("Tag");

                    b.HasKey("Id");

                    b.HasIndex("GnisFipsConcatenation")
                        .IsUnique()
                        .HasFilter("[GnisFipsConcatenation] IS NOT NULL");

                    b.HasIndex("Name")
                        .HasName("IX_JurisdictionName");

                    b.ToTable("Jurisdictions");
                });

            modelBuilder.Entity("Esquire.Models.Project", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CreatedById");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateLastEdited");

                    b.Property<long?>("LastEditedById");

                    b.Property<string>("Name");

                    b.Property<long?>("SchemeId");

                    b.Property<byte>("Status")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue((byte)1);

                    b.Property<int>("Type");

                    b.Property<long?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("LastEditedById");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.HasIndex("SchemeId");

                    b.HasIndex("UserId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Esquire.Models.ProjectJurisdiction", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("EndDate");

                    b.Property<long>("JurisdictionId");

                    b.Property<long>("ProjectId");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.HasIndex("JurisdictionId");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectJurisdictions");
                });

            modelBuilder.Entity("Esquire.Models.ProjectUser", b =>
                {
                    b.Property<long>("RowId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<long?>("ProjectId");

                    b.Property<string>("Role");

                    b.Property<long>("UserId");

                    b.HasKey("RowId");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectUsers");
                });

            modelBuilder.Entity("Esquire.Models.Protocol", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateLastEdited");

                    b.Property<long?>("LastEditedById");

                    b.Property<long>("ProjectId");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("LastEditedById");

                    b.HasIndex("ProjectId")
                        .IsUnique();

                    b.ToTable("Protocols");
                });

            modelBuilder.Entity("Esquire.Models.Scheme", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("QuestionNumbering");

                    b.HasKey("Id");

                    b.ToTable("Schemes");
                });

            modelBuilder.Entity("Esquire.Models.SchemeAnswer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Order");

                    b.Property<long?>("SchemeQuestionId");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("SchemeQuestionId");

                    b.ToTable("SchemeAnswers");
                });

            modelBuilder.Entity("Esquire.Models.SchemeQuestion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Hint");

                    b.Property<bool>("IncludeComment");

                    b.Property<bool>("IsCategoryQuestion");

                    b.Property<int>("QuestionType");

                    b.Property<long?>("SchemeId");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("SchemeId");

                    b.ToTable("SchemeQuestions");
                });

            modelBuilder.Entity("Esquire.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Avatar");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<bool>("IsActive");

                    b.Property<string>("LastName");

                    b.Property<string>("Role");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasFilter("[Email] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Esquire.Models.CodedCategoryQuestion", b =>
                {
                    b.HasBaseType("Esquire.Models.CodedQuestionBase");

                    b.Property<long?>("CategoryId");

                    b.Property<long?>("CodedById");

                    b.Property<long?>("ProjectJurisdictionId");

                    b.HasIndex("CodedById");

                    b.HasIndex("ProjectJurisdictionId");

                    b.HasIndex("CategoryId", "CodedById", "ProjectJurisdictionId", "SchemeQuestionId")
                        .IsUnique()
                        .HasFilter("[CategoryId] IS NOT NULL AND [CodedById] IS NOT NULL AND [ProjectJurisdictionId] IS NOT NULL");

                    b.ToTable("CodedCategoryQuestion");

                    b.HasDiscriminator().HasValue("CodedCategoryQuestion");
                });

            modelBuilder.Entity("Esquire.Models.CodedQuestion", b =>
                {
                    b.HasBaseType("Esquire.Models.CodedQuestionBase");

                    b.Property<long?>("CodedById")
                        .HasColumnName("CodedQuestion_CodedById");

                    b.Property<long?>("ProjectJurisdictionId")
                        .HasColumnName("CodedQuestion_ProjectJurisdictionId");

                    b.HasIndex("ProjectJurisdictionId");

                    b.HasIndex("CodedById", "ProjectJurisdictionId", "SchemeQuestionId")
                        .IsUnique()
                        .HasFilter("[CodedQuestion_CodedById] IS NOT NULL AND [CodedQuestion_ProjectJurisdictionId] IS NOT NULL");

                    b.ToTable("CodedQuestion");

                    b.HasDiscriminator().HasValue("CodedQuestion");
                });

            modelBuilder.Entity("Esquire.Models.ValidatedQuestionBase", b =>
                {
                    b.HasBaseType("Esquire.Models.CodedQuestionBase");

                    b.Property<long?>("ValidatedById");

                    b.HasIndex("ValidatedById");

                    b.ToTable("ValidatedQuestionBase");

                    b.HasDiscriminator().HasValue("ValidatedQuestionBase");
                });

            modelBuilder.Entity("Esquire.Models.CodedQuestionFlag", b =>
                {
                    b.HasBaseType("Esquire.Models.FlagBase");

                    b.Property<long?>("CodedQuestionId");

                    b.HasIndex("CodedQuestionId")
                        .IsUnique()
                        .HasFilter("[CodedQuestionId] IS NOT NULL");

                    b.ToTable("CodedQuestionFlag");

                    b.HasDiscriminator().HasValue("CodedQuestionFlag");
                });

            modelBuilder.Entity("Esquire.Models.SchemeQuestionFlag", b =>
                {
                    b.HasBaseType("Esquire.Models.FlagBase");

                    b.Property<long>("SchemeQuestionId");

                    b.HasIndex("SchemeQuestionId");

                    b.ToTable("SchemeQuestionFlag");

                    b.HasDiscriminator().HasValue("SchemeQuestionFlag");
                });

            modelBuilder.Entity("Esquire.Models.ValidatedCategoryQuestion", b =>
                {
                    b.HasBaseType("Esquire.Models.ValidatedQuestionBase");

                    b.Property<long?>("CategoryId")
                        .HasColumnName("ValidatedCategoryQuestion_CategoryId");

                    b.Property<long?>("ProjectJurisdictionId")
                        .HasColumnName("ValidatedCategoryQuestion_ProjectJurisdictionId");

                    b.HasIndex("ProjectJurisdictionId");

                    b.HasIndex("CategoryId", "ProjectJurisdictionId", "SchemeQuestionId")
                        .IsUnique()
                        .HasFilter("[ValidatedCategoryQuestion_CategoryId] IS NOT NULL AND [ValidatedCategoryQuestion_ProjectJurisdictionId] IS NOT NULL");

                    b.ToTable("ValidatedCategoryQuestion");

                    b.HasDiscriminator().HasValue("ValidatedCategoryQuestion");
                });

            modelBuilder.Entity("Esquire.Models.ValidatedQuestion", b =>
                {
                    b.HasBaseType("Esquire.Models.ValidatedQuestionBase");

                    b.Property<long?>("ProjectJurisdictionId")
                        .HasColumnName("ValidatedQuestion_ProjectJurisdictionId");

                    b.HasIndex("ProjectJurisdictionId", "SchemeQuestionId")
                        .IsUnique()
                        .HasFilter("[ValidatedQuestion_ProjectJurisdictionId] IS NOT NULL");

                    b.ToTable("ValidatedQuestion");

                    b.HasDiscriminator().HasValue("ValidatedQuestion");
                });

            modelBuilder.Entity("Esquire.Models.CodedAnswer", b =>
                {
                    b.HasOne("Esquire.Models.CodedQuestionBase")
                        .WithMany("CodedAnswers")
                        .HasForeignKey("CodedQuestionBaseId");

                    b.HasOne("Esquire.Models.SchemeAnswer", "SchemeAnswer")
                        .WithMany()
                        .HasForeignKey("SchemeAnswerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Esquire.Models.CodedQuestionBase", b =>
                {
                    b.HasOne("Esquire.Models.SchemeQuestion", "SchemeQuestion")
                        .WithMany()
                        .HasForeignKey("SchemeQuestionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Esquire.Models.FlagBase", b =>
                {
                    b.HasOne("Esquire.Models.User", "RaisedBy")
                        .WithMany()
                        .HasForeignKey("RaisedById");
                });

            modelBuilder.Entity("Esquire.Models.Project", b =>
                {
                    b.HasOne("Esquire.Models.User", "CreatedBy")
                        .WithMany("Created")
                        .HasForeignKey("CreatedById");

                    b.HasOne("Esquire.Models.User", "LastEditedBy")
                        .WithMany("LastEdited")
                        .HasForeignKey("LastEditedById");

                    b.HasOne("Esquire.Models.Scheme", "Scheme")
                        .WithMany()
                        .HasForeignKey("SchemeId");

                    b.HasOne("Esquire.Models.User")
                        .WithMany("BookmarkedProjects")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Esquire.Models.ProjectJurisdiction", b =>
                {
                    b.HasOne("Esquire.Models.Jurisdiction", "Jurisdiction")
                        .WithMany("ProjectJurisdictions")
                        .HasForeignKey("JurisdictionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Esquire.Models.Project", "Project")
                        .WithMany("ProjectJurisdictions")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Esquire.Models.ProjectUser", b =>
                {
                    b.HasOne("Esquire.Models.Project")
                        .WithMany("ProjectUsers")
                        .HasForeignKey("ProjectId");
                });

            modelBuilder.Entity("Esquire.Models.Protocol", b =>
                {
                    b.HasOne("Esquire.Models.User", "LastEditedBy")
                        .WithMany()
                        .HasForeignKey("LastEditedById");

                    b.HasOne("Esquire.Models.Project")
                        .WithOne("Protocol")
                        .HasForeignKey("Esquire.Models.Protocol", "ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Esquire.Models.SchemeAnswer", b =>
                {
                    b.HasOne("Esquire.Models.SchemeQuestion")
                        .WithMany("PossibleAnswers")
                        .HasForeignKey("SchemeQuestionId");
                });

            modelBuilder.Entity("Esquire.Models.SchemeQuestion", b =>
                {
                    b.HasOne("Esquire.Models.Scheme")
                        .WithMany("Questions")
                        .HasForeignKey("SchemeId");
                });

            modelBuilder.Entity("Esquire.Models.CodedCategoryQuestion", b =>
                {
                    b.HasOne("Esquire.Models.SchemeAnswer", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("Esquire.Models.User", "CodedBy")
                        .WithMany()
                        .HasForeignKey("CodedById");

                    b.HasOne("Esquire.Models.ProjectJurisdiction", "ProjectJurisdiction")
                        .WithMany()
                        .HasForeignKey("ProjectJurisdictionId");
                });

            modelBuilder.Entity("Esquire.Models.CodedQuestion", b =>
                {
                    b.HasOne("Esquire.Models.User", "CodedBy")
                        .WithMany()
                        .HasForeignKey("CodedById");

                    b.HasOne("Esquire.Models.ProjectJurisdiction", "ProjectJurisdiction")
                        .WithMany()
                        .HasForeignKey("ProjectJurisdictionId");
                });

            modelBuilder.Entity("Esquire.Models.ValidatedQuestionBase", b =>
                {
                    b.HasOne("Esquire.Models.User", "ValidatedBy")
                        .WithMany()
                        .HasForeignKey("ValidatedById");
                });

            modelBuilder.Entity("Esquire.Models.CodedQuestionFlag", b =>
                {
                    b.HasOne("Esquire.Models.CodedQuestionBase", "CodedQuestion")
                        .WithOne("Flag")
                        .HasForeignKey("Esquire.Models.CodedQuestionFlag", "CodedQuestionId");
                });

            modelBuilder.Entity("Esquire.Models.SchemeQuestionFlag", b =>
                {
                    b.HasOne("Esquire.Models.SchemeQuestion")
                        .WithMany("Flags")
                        .HasForeignKey("SchemeQuestionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Esquire.Models.ValidatedCategoryQuestion", b =>
                {
                    b.HasOne("Esquire.Models.SchemeAnswer", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("Esquire.Models.ProjectJurisdiction", "ProjectJurisdiction")
                        .WithMany()
                        .HasForeignKey("ProjectJurisdictionId");
                });

            modelBuilder.Entity("Esquire.Models.ValidatedQuestion", b =>
                {
                    b.HasOne("Esquire.Models.ProjectJurisdiction", "ProjectJurisdiction")
                        .WithMany()
                        .HasForeignKey("ProjectJurisdictionId");
                });
#pragma warning restore 612, 618
        }
    }
}
