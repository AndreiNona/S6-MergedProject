using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddMovieListingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
    migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetRoles')
        BEGIN
            CREATE TABLE [AspNetRoles] (
                [Id] nvarchar(450) NOT NULL,
                [Name] nvarchar(256) NULL,
                [NormalizedName] nvarchar(256) NULL,
                [ConcurrencyStamp] nvarchar(max) NULL,
                CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
            );
        END
    ");

    migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUsers')
        BEGIN
            CREATE TABLE [AspNetUsers] (
                [Id] nvarchar(450) NOT NULL,
                [UserName] nvarchar(256) NULL,
                [NormalizedUserName] nvarchar(256) NULL,
                [Email] nvarchar(256) NULL,
                [NormalizedEmail] nvarchar(256) NULL,
                [EmailConfirmed] bit NOT NULL,
                [PasswordHash] nvarchar(max) NULL,
                [SecurityStamp] nvarchar(max) NULL,
                [ConcurrencyStamp] nvarchar(max) NULL,
                [PhoneNumber] nvarchar(max) NULL,
                [PhoneNumberConfirmed] bit NOT NULL,
                [TwoFactorEnabled] bit NOT NULL,
                [LockoutEnd] datetimeoffset NULL,
                [LockoutEnabled] bit NOT NULL,
                [AccessFailedCount] int NOT NULL,
                CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
            );
        END
    ");

    // Comment out the following blocks since `Movies` and `People` tables already exist.
    /*
    migrationBuilder.CreateTable(
        name: "Movies",
        columns: table => new
        {
            Id = table.Column<int>(type: "int", nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"),
            Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Year = table.Column<int>(type: "int", nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Movies", x => x.Id);
        });

    migrationBuilder.CreateTable(
        name: "People",
        columns: table => new
        {
            Id = table.Column<int>(type: "int", nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Birth = table.Column<int>(type: "int", nullable: true)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_People", x => x.Id);
        });
    */

    migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Rating')
        BEGIN
            CREATE TABLE [Rating] (
                [Source] nvarchar(max) NOT NULL,
                [Value] nvarchar(max) NOT NULL
            );
        END
    ");

    // Continue to add checks for each additional table.
    migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MovieListings')
        BEGIN
            CREATE TABLE [MovieListings] (
                [Id] int NOT NULL IDENTITY,
                [Rank] int NOT NULL,
                [MovieId] int NOT NULL,
                [UserId] nvarchar(450) NOT NULL,
                CONSTRAINT [PK_MovieListings] PRIMARY KEY ([Id]),
                FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
            );
        END
    ");

    // Create AspNetRoles table if it does not exist
    migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetRoles')
        BEGIN
            CREATE TABLE [AspNetRoles] (
                [Id] nvarchar(450) NOT NULL,
                [Name] nvarchar(256) NULL,
                [NormalizedName] nvarchar(256) NULL,
                [ConcurrencyStamp] nvarchar(max) NULL,
                CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
            );
        END
    ");

    // Create AspNetUsers table if it does not exist
    migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUsers')
        BEGIN
            CREATE TABLE [AspNetUsers] (
                [Id] nvarchar(450) NOT NULL,
                [UserName] nvarchar(256) NULL,
                [NormalizedUserName] nvarchar(256) NULL,
                [Email] nvarchar(256) NULL,
                [NormalizedEmail] nvarchar(256) NULL,
                [EmailConfirmed] bit NOT NULL,
                [PasswordHash] nvarchar(max) NULL,
                [SecurityStamp] nvarchar(max) NULL,
                [ConcurrencyStamp] nvarchar(max) NULL,
                [PhoneNumber] nvarchar(max) NULL,
                [PhoneNumberConfirmed] bit NOT NULL,
                [TwoFactorEnabled] bit NOT NULL,
                [LockoutEnd] datetimeoffset NULL,
                [LockoutEnabled] bit NOT NULL,
                [AccessFailedCount] int NOT NULL,
                CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
            );
        END
    ");

    // Create Rating table if it does not exist
    migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Rating')
        BEGIN
            CREATE TABLE [Rating] (
                [Source] nvarchar(max) NOT NULL,
                [Value] nvarchar(max) NOT NULL
            );
        END
    ");

    // Create MovieListings table if it does not exist
    migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MovieListings')
        BEGIN
            CREATE TABLE [MovieListings] (
                [Id] int NOT NULL IDENTITY,
                [Rank] int NOT NULL,
                [MovieId] int NOT NULL,
                [UserId] nvarchar(450) NOT NULL,
                CONSTRAINT [PK_MovieListings] PRIMARY KEY ([Id]),
                FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
            );
        END
    ");

    // Create Directors table if it does not exist
    migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Directors')
        BEGIN
            CREATE TABLE [Directors] (
                [movie_id] int NOT NULL,
                [person_id] int NOT NULL,
                FOREIGN KEY ([movie_id]) REFERENCES [Movies]([Id]),
                FOREIGN KEY ([person_id]) REFERENCES [People]([Id])
            );
        END
    ");

    // Create Stars table if it does not exist
    migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Stars')
        BEGIN
            CREATE TABLE [Stars] (
                [movie_id] int NOT NULL,
                [person_id] int NOT NULL,
                FOREIGN KEY ([movie_id]) REFERENCES [Movies]([Id]),
                FOREIGN KEY ([person_id]) REFERENCES [People]([Id])
            );
        END
    ");

    // Create AspNetRoleClaims table if it does not exist
    migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetRoleClaims')
        BEGIN
            CREATE TABLE [AspNetRoleClaims] (
                [Id] int NOT NULL IDENTITY,
                [RoleId] nvarchar(450) NOT NULL,
                [ClaimType] nvarchar(max) NULL,
                [ClaimValue] nvarchar(max) NULL,
                CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
                FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles]([Id])
            );
        END
    ");

    // Create AspNetUserClaims table if it does not exist
    migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUserClaims')
        BEGIN
            CREATE TABLE [AspNetUserClaims] (
                [Id] int NOT NULL IDENTITY,
                [UserId] nvarchar(450) NOT NULL,
                [ClaimType] nvarchar(max) NULL,
                [ClaimValue] nvarchar(max) NULL,
                CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
                FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
            );
        END
    ");

    // Create AspNetUserLogins table if it does not exist
    migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUserLogins')
        BEGIN
            CREATE TABLE [AspNetUserLogins] (
                [LoginProvider] nvarchar(450) NOT NULL,
                [ProviderKey] nvarchar(450) NOT NULL,
                [ProviderDisplayName] nvarchar(max) NULL,
                [UserId] nvarchar(450) NOT NULL,
                CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
                FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
            );
        END
    ");

    // Create AspNetUserRoles table if it does not exist
    migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUserRoles')
        BEGIN
            CREATE TABLE [AspNetUserRoles] (
                [UserId] nvarchar(450) NOT NULL,
                [RoleId] nvarchar(450) NOT NULL,
                CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
                FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
                FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles]([Id])
            );
        END
    ");

    // Create AspNetUserTokens table if it does not exist
    migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUserTokens')
        BEGIN
            CREATE TABLE [AspNetUserTokens] (
                [UserId] nvarchar(450) NOT NULL,
                [LoginProvider] nvarchar(450) NOT NULL,
                [Name] nvarchar(450) NOT NULL,
                [Value] nvarchar(max) NULL,
                CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
                FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
            );
        END
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Directors");

            migrationBuilder.DropTable(
                name: "MovieListings");

            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropTable(
                name: "Stars");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            /*migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "People");*/
        }
    }
}
