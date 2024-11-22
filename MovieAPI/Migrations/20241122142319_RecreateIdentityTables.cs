using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class RecreateIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if AspNetUsers table exists before creating it
            migrationBuilder.Sql(@"
        IF NOT EXISTS (
            SELECT * 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_NAME = 'AspNetUsers'
        )
        BEGIN
            CREATE TABLE [AspNetUsers] (
                [Id] NVARCHAR(450) NOT NULL PRIMARY KEY,
                [UserName] NVARCHAR(256) NULL,
                [NormalizedUserName] NVARCHAR(256) NULL,
                [Email] NVARCHAR(256) NULL,
                [NormalizedEmail] NVARCHAR(256) NULL,
                [EmailConfirmed] BIT NOT NULL,
                [PasswordHash] NVARCHAR(MAX) NULL,
                [SecurityStamp] NVARCHAR(MAX) NULL,
                [ConcurrencyStamp] NVARCHAR(MAX) NULL,
                [PhoneNumber] NVARCHAR(MAX) NULL,
                [PhoneNumberConfirmed] BIT NOT NULL,
                [TwoFactorEnabled] BIT NOT NULL,
                [LockoutEnd] DATETIMEOFFSET NULL,
                [LockoutEnabled] BIT NOT NULL,
                [AccessFailedCount] INT NOT NULL
            );
        END
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
