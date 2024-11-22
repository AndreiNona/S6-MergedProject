using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedTopListRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{
    // Drop Foreign Key if it exists
    migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT 1
            FROM sys.foreign_keys
            WHERE name = 'FK_MovieListings_AspNetUsers_UserId'
        )
        BEGIN
            ALTER TABLE MovieListings DROP CONSTRAINT FK_MovieListings_AspNetUsers_UserId;
        END
    ");

    // Drop Primary Key if it exists
    migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT 1
            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
            WHERE CONSTRAINT_NAME = 'PK_MovieListings'
        )
        BEGIN
            ALTER TABLE MovieListings DROP CONSTRAINT PK_MovieListings;
        END
    ");

    // Rename Table only if it exists and hasn't been renamed
    migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT 1
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_NAME = 'MovieListings'
        )
        BEGIN
            EXEC sp_rename 'MovieListings', 'MovieListing';
        END
    ");

    // Rename Index if it exists
    migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT 1
            FROM sys.indexes
            WHERE name = 'IX_MovieListings_UserId'
        )
        BEGIN
            EXEC sp_rename 'IX_MovieListings_UserId', 'IX_MovieListing_UserId', 'INDEX';
        END
    ");

    // Add Primary Key to renamed table
    migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT 1
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_NAME = 'MovieListing'
        )
        AND NOT EXISTS (
            SELECT 1
            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
            WHERE CONSTRAINT_NAME = 'PK_MovieListing'
        )
        BEGIN
            ALTER TABLE MovieListing ADD CONSTRAINT PK_MovieListing PRIMARY KEY (Id);
        END
    ");

    // Create TopLists table if it doesn't exist
    migrationBuilder.Sql(@"
        IF NOT EXISTS (
            SELECT 1
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_NAME = 'TopLists'
        )
        BEGIN
            CREATE TABLE TopLists (
                Id INT NOT NULL IDENTITY PRIMARY KEY,
                Name NVARCHAR(MAX) NOT NULL,
                UserId NVARCHAR(MAX) NOT NULL,
                MovieIds NVARCHAR(MAX) NOT NULL
            );
        END
    ");

    // Add Foreign Key to MovieListing table if it doesn't exist
    migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT 1
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_NAME = 'MovieListing'
        )
        AND NOT EXISTS (
            SELECT 1
            FROM sys.foreign_keys
            WHERE name = 'FK_MovieListing_AspNetUsers_UserId'
        )
        BEGIN
            ALTER TABLE MovieListing ADD CONSTRAINT FK_MovieListing_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE;
        END
    ");
}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieListing_AspNetUsers_UserId",
                table: "MovieListing");

            migrationBuilder.DropTable(
                name: "TopLists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieListing",
                table: "MovieListing");

            migrationBuilder.RenameTable(
                name: "MovieListing",
                newName: "MovieListings");

            migrationBuilder.RenameIndex(
                name: "IX_MovieListing_UserId",
                table: "MovieListings",
                newName: "IX_MovieListings_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieListings",
                table: "MovieListings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieListings_AspNetUsers_UserId",
                table: "MovieListings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
