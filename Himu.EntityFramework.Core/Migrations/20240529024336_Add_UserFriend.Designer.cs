﻿// <auto-generated />
using System;
using Himu.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Himu.EntityFramework.Core.Migrations
{
    [DbContext(typeof(HimuMySqlContext))]
    [Migration("20240529024336_Add_UserFriend")]
    partial class Add_UserFriend
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.ContestCreator", b =>
                {
                    b.Property<long>("ContestId")
                        .HasColumnType("bigint");

                    b.Property<long>("CreatorId")
                        .HasColumnType("bigint");

                    b.HasKey("ContestId", "CreatorId");

                    b.HasIndex("CreatorId");

                    b.ToTable("ContestCreators");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.HimuArticle", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("AuthorId")
                        .HasColumnType("bigint");

                    b.Property<string>("Brief")
                        .HasColumnType("longtext");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.HimuCommit", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateOnly>("CommitDate")
                        .HasColumnType("date");

                    b.Property<long>("ProblemId")
                        .HasColumnType("bigint");

                    b.Property<string>("SourceUri")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ProblemId");

                    b.HasIndex("Status");

                    b.HasIndex("UserId");

                    b.ToTable("UserCommits");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.HimuContest", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<DateOnly>("CreateDate")
                        .HasColumnType("date");

                    b.Property<long>("DistributorId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("DistributorId");

                    b.ToTable("Contests");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.HimuHomeRole", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.HimuHomeUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("Background")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateOnly>("LastLoginDate")
                        .HasColumnType("date");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateOnly>("RegisterDate")
                        .HasColumnType("date");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.HimuProblem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("ContestId")
                        .HasColumnType("bigint");

                    b.Property<long>("DistributorId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ContestId");

                    b.HasIndex("DistributorId");

                    b.ToTable("ProblemSet");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.HimuTestPoint", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("CaseName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)");

                    b.Property<string>("Expected")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("Input")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<long>("ProblemId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ProblemId");

                    b.ToTable("TestPoints");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.PermissionRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Operation")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("varchar(16)");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("PermissionRecords");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.TestPointResult", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("CommitId")
                        .HasColumnType("bigint");

                    b.Property<long>("TestPointId")
                        .HasColumnType("bigint");

                    b.Property<string>("TestStatus")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("varchar(32)");

                    b.HasKey("Id");

                    b.HasIndex("CommitId");

                    b.HasIndex("TestPointId");

                    b.ToTable("PointResults");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.UserFriend", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long>("FriendId")
                        .HasColumnType("bigint");

                    b.HasKey("UserId", "FriendId");

                    b.HasIndex("FriendId");

                    b.ToTable("UserFriend");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<long>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<long>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<long>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<long>", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<long>", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.ContestCreator", b =>
                {
                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuContest", "Contest")
                        .WithMany()
                        .HasForeignKey("ContestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuHomeUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contest");

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.HimuArticle", b =>
                {
                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuHomeUser", "Author")
                        .WithMany("Articles")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.HimuCommit", b =>
                {
                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuProblem", "Problem")
                        .WithMany("UserCommits")
                        .HasForeignKey("ProblemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuHomeUser", "User")
                        .WithMany("MyCommits")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Himu.EntityFramework.Core.Entity.Components.CompilerInfo", "CompilerInformation", b1 =>
                        {
                            b1.Property<long>("HimuCommitId")
                                .HasColumnType("bigint");

                            b1.Property<string>("CompilerName")
                                .IsRequired()
                                .HasMaxLength(128)
                                .HasColumnType("varchar(128)");

                            b1.Property<string>("MessageFromCompiler")
                                .HasMaxLength(4096)
                                .HasColumnType("varchar(4096)");

                            b1.HasKey("HimuCommitId");

                            b1.HasIndex("CompilerName");

                            b1.ToTable("UserCommits");

                            b1.WithOwner()
                                .HasForeignKey("HimuCommitId");
                        });

                    b.Navigation("CompilerInformation")
                        .IsRequired();

                    b.Navigation("Problem");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.HimuContest", b =>
                {
                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuHomeUser", "Distributor")
                        .WithMany("Contests")
                        .HasForeignKey("DistributorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Himu.EntityFramework.Core.Entity.Components.HimuContestInformation", "Information", b1 =>
                        {
                            b1.Property<long>("HimuContestId")
                                .HasColumnType("bigint");

                            b1.Property<string>("Code")
                                .IsRequired()
                                .HasMaxLength(128)
                                .HasColumnType("varchar(128)")
                                .HasColumnName("ContestCode");

                            b1.Property<string>("Description")
                                .IsRequired()
                                .HasMaxLength(100000)
                                .HasColumnType("longtext");

                            b1.Property<string>("Introduction")
                                .IsRequired()
                                .HasMaxLength(1024)
                                .HasColumnType("varchar(1024)");

                            b1.Property<bool>("LaunchTaskAtOnce")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("tinyint(1)")
                                .HasDefaultValue(false);

                            b1.Property<string>("Title")
                                .IsRequired()
                                .HasMaxLength(128)
                                .HasColumnType("varchar(128)");

                            b1.HasKey("HimuContestId");

                            b1.HasIndex("Code")
                                .IsUnique();

                            b1.HasIndex("Title")
                                .HasAnnotation("MySql:IndexPrefixLength", new[] { 50 });

                            b1.ToTable("Contests");

                            b1.WithOwner()
                                .HasForeignKey("HimuContestId");
                        });

                    b.Navigation("Distributor");

                    b.Navigation("Information")
                        .IsRequired();
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.HimuProblem", b =>
                {
                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuContest", "Contest")
                        .WithMany("Problems")
                        .HasForeignKey("ContestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuHomeUser", "Distributor")
                        .WithMany("Problems")
                        .HasForeignKey("DistributorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Himu.EntityFramework.Core.Entity.HimuProblemDetail", "Detail", b1 =>
                        {
                            b1.Property<long>("HimuProblemId")
                                .HasColumnType("bigint");

                            b1.Property<bool>("AllowDownloadAnswer")
                                .HasColumnType("tinyint(1)");

                            b1.Property<bool>("AllowDownloadInput")
                                .HasColumnType("tinyint(1)");

                            b1.Property<string>("Code")
                                .IsRequired()
                                .HasMaxLength(64)
                                .HasColumnType("varchar(64)");

                            b1.Property<string>("Content")
                                .IsRequired()
                                .HasMaxLength(1000000)
                                .HasColumnType("longtext");

                            b1.Property<long>("MaxExecuteTimeLimit")
                                .HasColumnType("bigint");

                            b1.Property<long>("MaxMemoryLimitByte")
                                .HasColumnType("bigint");

                            b1.Property<string>("Title")
                                .IsRequired()
                                .HasMaxLength(512)
                                .HasColumnType("varchar(512)");

                            b1.HasKey("HimuProblemId");

                            b1.HasIndex("Code");

                            b1.HasIndex("Title")
                                .HasAnnotation("MySql:IndexPrefixLength", new[] { 10 });

                            b1.ToTable("ProblemSet");

                            b1.WithOwner()
                                .HasForeignKey("HimuProblemId");
                        });

                    b.Navigation("Contest");

                    b.Navigation("Detail")
                        .IsRequired();

                    b.Navigation("Distributor");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.HimuTestPoint", b =>
                {
                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuProblem", "Problem")
                        .WithMany("TestPoints")
                        .HasForeignKey("ProblemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Problem");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.PermissionRecord", b =>
                {
                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuHomeRole", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuHomeUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.TestPointResult", b =>
                {
                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuCommit", "Commit")
                        .WithMany("TestPointResults")
                        .HasForeignKey("CommitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuTestPoint", "TestPoint")
                        .WithMany()
                        .HasForeignKey("TestPointId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Himu.EntityFramework.Core.Entity.Components.ExitCodeWithMessage", "RunResult", b1 =>
                        {
                            b1.Property<long>("TestPointResultId")
                                .HasColumnType("bigint");

                            b1.Property<int>("ExitCode")
                                .HasColumnType("int");

                            b1.Property<string>("Message")
                                .IsRequired()
                                .HasColumnType("longtext");

                            b1.HasKey("TestPointResultId");

                            b1.ToTable("PointResults");

                            b1.WithOwner()
                                .HasForeignKey("TestPointResultId");
                        });

                    b.OwnsOne("Himu.EntityFramework.Core.Entity.Components.OutputDifference", "Difference", b1 =>
                        {
                            b1.Property<long>("TestPointResultId")
                                .HasColumnType("bigint");

                            b1.Property<string>("Actual")
                                .IsRequired()
                                .HasColumnType("longtext");

                            b1.Property<string>("Expected")
                                .IsRequired()
                                .HasColumnType("longtext");

                            b1.Property<int>("Position")
                                .HasColumnType("int");

                            b1.HasKey("TestPointResultId");

                            b1.ToTable("PointResults");

                            b1.WithOwner()
                                .HasForeignKey("TestPointResultId");
                        });

                    b.OwnsOne("Himu.EntityFramework.Core.Entity.Components.ResourceUsage", "Usage", b1 =>
                        {
                            b1.Property<long>("TestPointResultId")
                                .HasColumnType("bigint");

                            b1.Property<long>("MemoryByteUsed")
                                .HasColumnType("bigint");

                            b1.Property<long>("TimeUsed")
                                .HasColumnType("bigint");

                            b1.HasKey("TestPointResultId");

                            b1.ToTable("PointResults");

                            b1.WithOwner()
                                .HasForeignKey("TestPointResultId");
                        });

                    b.Navigation("Commit");

                    b.Navigation("Difference");

                    b.Navigation("RunResult");

                    b.Navigation("TestPoint");

                    b.Navigation("Usage");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.UserFriend", b =>
                {
                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuHomeUser", "Friend")
                        .WithMany()
                        .HasForeignKey("FriendId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuHomeUser", "User")
                        .WithMany("Friends")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Friend");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<long>", b =>
                {
                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuHomeRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<long>", b =>
                {
                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuHomeUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<long>", b =>
                {
                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuHomeUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<long>", b =>
                {
                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuHomeRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuHomeUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<long>", b =>
                {
                    b.HasOne("Himu.EntityFramework.Core.Entity.HimuHomeUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.HimuCommit", b =>
                {
                    b.Navigation("TestPointResults");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.HimuContest", b =>
                {
                    b.Navigation("Problems");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.HimuHomeUser", b =>
                {
                    b.Navigation("Articles");

                    b.Navigation("Contests");

                    b.Navigation("Friends");

                    b.Navigation("MyCommits");

                    b.Navigation("Problems");
                });

            modelBuilder.Entity("Himu.EntityFramework.Core.Entity.HimuProblem", b =>
                {
                    b.Navigation("TestPoints");

                    b.Navigation("UserCommits");
                });
#pragma warning restore 612, 618
        }
    }
}