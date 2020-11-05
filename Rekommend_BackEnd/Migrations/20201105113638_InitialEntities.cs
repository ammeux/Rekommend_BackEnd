using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rekommend_BackEnd.Migrations
{
    public partial class InitialEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RegistrationDate = table.Column<DateTimeOffset>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    HqCity = table.Column<string>(maxLength: 50, nullable: false),
                    CompanyDescription = table.Column<string>(maxLength: 1500, nullable: false),
                    Category = table.Column<int>(maxLength: 50, nullable: false),
                    logoFileName = table.Column<string>(maxLength: 50, nullable: false),
                    Website = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rekommenders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateOfBirth = table.Column<DateTimeOffset>(nullable: false),
                    RegistrationDate = table.Column<DateTimeOffset>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    Position = table.Column<int>(nullable: false),
                    Seniority = table.Column<int>(nullable: false),
                    Company = table.Column<string>(maxLength: 50, nullable: false),
                    City = table.Column<string>(maxLength: 50, nullable: false),
                    Email = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rekommenders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recruiters",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RegistrationDate = table.Column<DateTimeOffset>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    DateOfBirth = table.Column<DateTimeOffset>(nullable: false),
                    City = table.Column<string>(maxLength: 50, nullable: false),
                    Country = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recruiters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recruiters_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rekommendations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false),
                    StatusChangeDate = table.Column<DateTimeOffset>(nullable: false),
                    RekommenderId = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    Position = table.Column<int>(nullable: false),
                    Seniority = table.Column<int>(nullable: false),
                    Company = table.Column<string>(maxLength: 50, nullable: false),
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    Comment = table.Column<string>(maxLength: 1500, nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rekommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rekommendations_Rekommenders_RekommenderId",
                        column: x => x.RekommenderId,
                        principalTable: "Rekommenders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechJobOpenings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false),
                    ClosingDate = table.Column<DateTimeOffset>(nullable: false),
                    StartingDate = table.Column<DateTimeOffset>(nullable: false),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    RecruiterId = table.Column<Guid>(nullable: false),
                    JobTechLanguage = table.Column<int>(nullable: false),
                    JobPosition = table.Column<int>(nullable: false),
                    Seniority = table.Column<int>(nullable: false),
                    ContractType = table.Column<int>(nullable: false),
                    RemoteWorkAccepted = table.Column<bool>(nullable: false),
                    MissionDescription = table.Column<string>(maxLength: 1500, nullable: false),
                    City = table.Column<string>(maxLength: 50, nullable: false),
                    Country = table.Column<string>(maxLength: 50, nullable: false),
                    Reward1 = table.Column<string>(maxLength: 50, nullable: true),
                    Reward2 = table.Column<string>(maxLength: 50, nullable: true),
                    Reward3 = table.Column<string>(maxLength: 50, nullable: true),
                    LikesNb = table.Column<int>(nullable: false),
                    RekommendationsNb = table.Column<int>(nullable: false),
                    ViewNb = table.Column<int>(nullable: false),
                    MinimumSalary = table.Column<int>(nullable: false),
                    MaximumSalary = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    pictureFileName = table.Column<string>(maxLength: 50, nullable: true),
                    RseDescription = table.Column<string>(maxLength: 1500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechJobOpenings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechJobOpenings_Recruiters_RecruiterId",
                        column: x => x.RecruiterId,
                        principalTable: "Recruiters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RekoHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RekommenderId = table.Column<Guid>(nullable: false),
                    RekommendationId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTimeOffset>(nullable: false),
                    RekommendationStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RekoHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RekoHistories_Rekommendations_RekommendationId",
                        column: x => x.RekommendationId,
                        principalTable: "Rekommendations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RekoHistories_Rekommenders_RekommenderId",
                        column: x => x.RekommenderId,
                        principalTable: "Rekommenders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recruiters_CompanyId",
                table: "Recruiters",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RekoHistories_RekommendationId",
                table: "RekoHistories",
                column: "RekommendationId");

            migrationBuilder.CreateIndex(
                name: "IX_RekoHistories_RekommenderId",
                table: "RekoHistories",
                column: "RekommenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Rekommendations_RekommenderId",
                table: "Rekommendations",
                column: "RekommenderId");

            migrationBuilder.CreateIndex(
                name: "IX_TechJobOpenings_RecruiterId",
                table: "TechJobOpenings",
                column: "RecruiterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RekoHistories");

            migrationBuilder.DropTable(
                name: "TechJobOpenings");

            migrationBuilder.DropTable(
                name: "Rekommendations");

            migrationBuilder.DropTable(
                name: "Recruiters");

            migrationBuilder.DropTable(
                name: "Rekommenders");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
