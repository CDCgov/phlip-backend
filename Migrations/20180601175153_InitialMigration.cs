using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace esquirebackend.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jurisdictions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GnisFipsConcatenation = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    GnisCode = table.Column<string>(nullable: true),
                    FipsCode = table.Column<string>(nullable: true),
                    Tag = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jurisdictions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schemes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuestionNumbering = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schemes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchemeQuestions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(nullable: true),
                    Hint = table.Column<string>(nullable: true),
                    IncludeComment = table.Column<bool>(nullable: false),
                    QuestionType = table.Column<int>(nullable: false),
                    IsCategoryQuestion = table.Column<bool>(nullable: false),
                    SchemeId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemeQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemeQuestions_Schemes_SchemeId",
                        column: x => x.SchemeId,
                        principalTable: "Schemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    SchemeId = table.Column<long>(nullable: true),
                    DateLastEdited = table.Column<DateTime>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastEditedById = table.Column<long>(nullable: true),
                    CreatedById = table.Column<long>(nullable: true),
                    UserId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_Users_LastEditedById",
                        column: x => x.LastEditedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_Schemes_SchemeId",
                        column: x => x.SchemeId,
                        principalTable: "Schemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SchemeAnswers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    SchemeQuestionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemeAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemeAnswers_SchemeQuestions_SchemeQuestionId",
                        column: x => x.SchemeQuestionId,
                        principalTable: "SchemeQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectJurisdictions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<long>(nullable: false),
                    JurisdictionId = table.Column<long>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectJurisdictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectJurisdictions_Jurisdictions_JurisdictionId",
                        column: x => x.JurisdictionId,
                        principalTable: "Jurisdictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectJurisdictions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Protocols",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<long>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    LastEditedById = table.Column<long>(nullable: true),
                    DateLastEdited = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Protocols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Protocols_Users_LastEditedById",
                        column: x => x.LastEditedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Protocols_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CodedQuestionBases",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SchemeQuestionId = table.Column<long>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    CategoryId = table.Column<long>(nullable: true),
                    ProjectJurisdictionId = table.Column<long>(nullable: true),
                    CodedById = table.Column<long>(nullable: true),
                    CodedQuestion_ProjectJurisdictionId = table.Column<long>(nullable: true),
                    CodedQuestion_CodedById = table.Column<long>(nullable: true),
                    ValidatedById = table.Column<long>(nullable: true),
                    ValidatedCategoryQuestion_CategoryId = table.Column<long>(nullable: true),
                    ValidatedCategoryQuestion_ProjectJurisdictionId = table.Column<long>(nullable: true),
                    ValidatedQuestion_ProjectJurisdictionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodedQuestionBases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodedQuestionBases_SchemeAnswers_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "SchemeAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CodedQuestionBases_Users_CodedById",
                        column: x => x.CodedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CodedQuestionBases_ProjectJurisdictions_ProjectJurisdictionId",
                        column: x => x.ProjectJurisdictionId,
                        principalTable: "ProjectJurisdictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CodedQuestionBases_Users_CodedQuestion_CodedById",
                        column: x => x.CodedQuestion_CodedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CodedQuestionBases_ProjectJurisdictions_CodedQuestion_ProjectJurisdictionId",
                        column: x => x.CodedQuestion_ProjectJurisdictionId,
                        principalTable: "ProjectJurisdictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CodedQuestionBases_SchemeQuestions_SchemeQuestionId",
                        column: x => x.SchemeQuestionId,
                        principalTable: "SchemeQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CodedQuestionBases_SchemeAnswers_ValidatedCategoryQuestion_CategoryId",
                        column: x => x.ValidatedCategoryQuestion_CategoryId,
                        principalTable: "SchemeAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CodedQuestionBases_ProjectJurisdictions_ValidatedCategoryQuestion_ProjectJurisdictionId",
                        column: x => x.ValidatedCategoryQuestion_ProjectJurisdictionId,
                        principalTable: "ProjectJurisdictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CodedQuestionBases_ProjectJurisdictions_ValidatedQuestion_ProjectJurisdictionId",
                        column: x => x.ValidatedQuestion_ProjectJurisdictionId,
                        principalTable: "ProjectJurisdictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CodedQuestionBases_Users_ValidatedById",
                        column: x => x.ValidatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CodedAnswers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SchemeAnswerId = table.Column<long>(nullable: false),
                    Pincite = table.Column<string>(nullable: true),
                    TextAnswer = table.Column<string>(nullable: true),
                    CodedQuestionBaseId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodedAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodedAnswers_CodedQuestionBases_CodedQuestionBaseId",
                        column: x => x.CodedQuestionBaseId,
                        principalTable: "CodedQuestionBases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CodedAnswers_SchemeAnswers_SchemeAnswerId",
                        column: x => x.SchemeAnswerId,
                        principalTable: "SchemeAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlagBases",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RaisedAt = table.Column<DateTime>(nullable: false),
                    RaisedById = table.Column<long>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    CodedQuestionId = table.Column<long>(nullable: true),
                    SchemeQuestionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlagBases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlagBases_CodedQuestionBases_CodedQuestionId",
                        column: x => x.CodedQuestionId,
                        principalTable: "CodedQuestionBases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlagBases_Users_RaisedById",
                        column: x => x.RaisedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlagBases_SchemeQuestions_SchemeQuestionId",
                        column: x => x.SchemeQuestionId,
                        principalTable: "SchemeQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CodedAnswers_SchemeAnswerId",
                table: "CodedAnswers",
                column: "SchemeAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_CodedAnswers_CodedQuestionBaseId_SchemeAnswerId",
                table: "CodedAnswers",
                columns: new[] { "CodedQuestionBaseId", "SchemeAnswerId" },
                unique: true,
                filter: "[CodedQuestionBaseId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CodedQuestionBases_CodedById",
                table: "CodedQuestionBases",
                column: "CodedById");

            migrationBuilder.CreateIndex(
                name: "IX_CodedQuestionBases_ProjectJurisdictionId",
                table: "CodedQuestionBases",
                column: "ProjectJurisdictionId");

            // CodedCategoryQuestion index
            migrationBuilder.CreateIndex(
                name: "IX_CodedQuestionBases_CategoryId_CodedById_ProjectJurisdictionId_SchemeQuestionId",
                table: "CodedQuestionBases",
                columns: new[] { "CategoryId", "CodedById", "ProjectJurisdictionId", "SchemeQuestionId" },
                unique: true,
                filter: "[CategoryId] IS NOT NULL AND [CodedById] IS NOT NULL AND [ProjectJurisdictionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CodedQuestionBases_CodedQuestion_ProjectJurisdictionId",
                table: "CodedQuestionBases",
                column: "CodedQuestion_ProjectJurisdictionId");

            // CodedQuestionIndex
            migrationBuilder.CreateIndex(
                name: "IX_CodedQuestionBases_CodedQuestion_CodedById_CodedQuestion_ProjectJurisdictionId_SchemeQuestionId",
                table: "CodedQuestionBases",
                columns: new[] { "CodedQuestion_CodedById", "CodedQuestion_ProjectJurisdictionId", "SchemeQuestionId" },
                unique: true,
                filter: "[CodedQuestion_CodedById] IS NOT NULL AND [CodedQuestion_ProjectJurisdictionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CodedQuestionBases_SchemeQuestionId",
                table: "CodedQuestionBases",
                column: "SchemeQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_CodedQuestionBases_ValidatedCategoryQuestion_ProjectJurisdictionId",
                table: "CodedQuestionBases",
                column: "ValidatedCategoryQuestion_ProjectJurisdictionId");

            // ValidatedCategoryQuestion
            migrationBuilder.CreateIndex(
                name: "IX_CodedQuestionBases_ValidatedCategoryQuestion_CategoryId_ValidatedCategoryQuestion_ProjectJurisdictionId_SchemeQuestionId",
                table: "CodedQuestionBases",
                columns: new[] { "ValidatedCategoryQuestion_CategoryId", "ValidatedCategoryQuestion_ProjectJurisdictionId", "SchemeQuestionId" },
                unique: true,
                filter: "[ValidatedCategoryQuestion_CategoryId] IS NOT NULL AND [ValidatedCategoryQuestion_ProjectJurisdictionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CodedQuestionBases_ValidatedQuestion_ProjectJurisdictionId_SchemeQuestionId",
                table: "CodedQuestionBases",
                columns: new[] { "ValidatedQuestion_ProjectJurisdictionId", "SchemeQuestionId" },
                unique: true,
                filter: "[ValidatedQuestion_ProjectJurisdictionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CodedQuestionBases_ValidatedById",
                table: "CodedQuestionBases",
                column: "ValidatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FlagBases_CodedQuestionId",
                table: "FlagBases",
                column: "CodedQuestionId",
                unique: true,
                filter: "[CodedQuestionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FlagBases_RaisedById",
                table: "FlagBases",
                column: "RaisedById");

            migrationBuilder.CreateIndex(
                name: "IX_FlagBases_SchemeQuestionId",
                table: "FlagBases",
                column: "SchemeQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Jurisdictions_GnisFipsConcatenation",
                table: "Jurisdictions",
                column: "GnisFipsConcatenation",
                unique: true,
                filter: "[GnisFipsConcatenation] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectJurisdictions_JurisdictionId",
                table: "ProjectJurisdictions",
                column: "JurisdictionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectJurisdictions_ProjectId",
                table: "ProjectJurisdictions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedById",
                table: "Projects",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LastEditedById",
                table: "Projects",
                column: "LastEditedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Name",
                table: "Projects",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_SchemeId",
                table: "Projects",
                column: "SchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId",
                table: "Projects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_LastEditedById",
                table: "Protocols",
                column: "LastEditedById");

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_ProjectId",
                table: "Protocols",
                column: "ProjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchemeAnswers_SchemeQuestionId",
                table: "SchemeAnswers",
                column: "SchemeQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeQuestions_SchemeId",
                table: "SchemeQuestions",
                column: "SchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CodedAnswers");

            migrationBuilder.DropTable(
                name: "FlagBases");

            migrationBuilder.DropTable(
                name: "Protocols");

            migrationBuilder.DropTable(
                name: "CodedQuestionBases");

            migrationBuilder.DropTable(
                name: "SchemeAnswers");

            migrationBuilder.DropTable(
                name: "ProjectJurisdictions");

            migrationBuilder.DropTable(
                name: "SchemeQuestions");

            migrationBuilder.DropTable(
                name: "Jurisdictions");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Schemes");
        }
    }
}
