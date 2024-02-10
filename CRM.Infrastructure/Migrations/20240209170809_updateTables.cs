using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowUpTime",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MessageTime",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Replay",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "FollowUpTime",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "MeetingTime",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "DealTime",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "CallTime",
                table: "Calls");

            migrationBuilder.DropColumn(
                name: "FollowUpTime",
                table: "Calls");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MessageDate",
                table: "Messages",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FollowUpDate",
                table: "Messages",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MeetingDate",
                table: "Meetings",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FollowUpDate",
                table: "Meetings",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DealDate",
                table: "Deals",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<int>(
                name: "InterestID",
                table: "Deals",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FollowUpDate",
                table: "Calls",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CallDate",
                table: "Calls",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<int>(
                name: "CallStatus",
                table: "Calls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Deals_InterestID",
                table: "Deals",
                column: "InterestID");

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_Interests_InterestID",
                table: "Deals",
                column: "InterestID",
                principalTable: "Interests",
                principalColumn: "InterestID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deals_Interests_InterestID",
                table: "Deals");

            migrationBuilder.DropIndex(
                name: "IX_Deals_InterestID",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "InterestID",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "CallStatus",
                table: "Calls");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MessageDate",
                table: "Messages",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "FollowUpDate",
                table: "Messages",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "FollowUpTime",
                table: "Messages",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "MessageTime",
                table: "Messages",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "Replay",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MeetingDate",
                table: "Meetings",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "FollowUpDate",
                table: "Meetings",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "FollowUpTime",
                table: "Meetings",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "MeetingTime",
                table: "Meetings",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DealDate",
                table: "Deals",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DealTime",
                table: "Deals",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AlterColumn<DateOnly>(
                name: "FollowUpDate",
                table: "Calls",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "CallDate",
                table: "Calls",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "CallTime",
                table: "Calls",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "FollowUpTime",
                table: "Calls",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
