﻿// <auto-generated />
using System;
using MHFoodBank.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MHFoodBank.Web.Migrations
{
    [DbContext(typeof(FoodBankContext))]
    partial class FoodBankContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("MHFoodBank.Web.Data.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Email")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.Alert", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AlertType")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("Deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("Read")
                        .HasColumnType("tinyint(1)");

                    b.Property<int?>("VolunteerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VolunteerId");

                    b.ToTable("Alerts");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Alert");
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.Availability", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AvailableDay")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<TimeSpan>("EndTime")
                        .HasColumnType("time(6)");

                    b.Property<TimeSpan>("StartTime")
                        .HasColumnType("time(6)");

                    b.Property<int?>("VolunteerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VolunteerId");

                    b.ToTable("Availabilities");
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.Position", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.PositionVolunteer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Association")
                        .HasColumnType("int");

                    b.Property<int?>("PositionId")
                        .HasColumnType("int");

                    b.Property<int?>("VolunteerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PositionId");

                    b.HasIndex("VolunteerId");

                    b.ToTable("PositionVolunteers");
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.Reference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Occupation")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Phone")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Relationship")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("VolunteerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VolunteerId");

                    b.ToTable("References");
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.Reminder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("HangfireJobId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("ShiftDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ShiftId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Reminders");
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.Shift", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<TimeSpan>("EndTime")
                        .HasColumnType("time(6)");

                    b.Property<bool>("Hidden")
                        .HasColumnType("tinyint(1)");

                    b.Property<int?>("ParentRecurringShiftId")
                        .HasColumnType("int");

                    b.Property<int?>("PositionWorkedId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime(6)");

                    b.Property<TimeSpan>("StartTime")
                        .HasColumnType("time(6)");

                    b.Property<int?>("VolunteerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParentRecurringShiftId");

                    b.HasIndex("PositionWorkedId");

                    b.HasIndex("VolunteerId");

                    b.ToTable("Shifts");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Shift");
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.VolunteerProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("AlternatePhone1")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("AlternatePhone2")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("Birthdate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("ChildWelfareCheck")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("ConfirmationOfProfessionalDesignation")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Cpr")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("CprExpiry")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("CriminalRecordCheck")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("DrivingAbstract")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("EducationTraining")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("EmergencyFullName")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("EmergencyPhone1")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("EmergencyPhone2")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("EmergencyRelationship")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("FirstAid")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("FirstAidExpiry")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("FoodSafe")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("FoodSafeExpiry")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("MainPhone")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("OfficiallyApproved")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("OtherBoards")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("OtherCertificates")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("SkillsInterestsHobbies")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<bool>("VolunteerConfidentiality")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("VolunteerEthics")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("VolunteerExperience")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("UserID")
                        .IsUnique();

                    b.ToTable("VolunteerProfiles");
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.WorkExperience", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ContactPerson")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("CurrentJob")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("EmployerAddress")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("EmployerName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("EmployerPhone")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("PositionWorked")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("VolunteerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VolunteerId");

                    b.ToTable("WorkExperiences");
                });

            modelBuilder.Entity("MHFoodBank.Web.Models.ClockedTime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("PositionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("VolunteerProfileId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PositionId");

                    b.HasIndex("VolunteerProfileId");

                    b.ToTable("ClockedTime");
                });

            modelBuilder.Entity("MHFoodBank.Web.Models.RecurringChildLink", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("NewShiftId")
                        .HasColumnType("int");

                    b.Property<int?>("OriginalShiftId")
                        .HasColumnType("int");

                    b.Property<int?>("ParentSetId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("NewShiftId");

                    b.HasIndex("OriginalShiftId");

                    b.HasIndex("ParentSetId");

                    b.ToTable("ShiftLinks");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("Value")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.ApplicationAlert", b =>
                {
                    b.HasBaseType("MHFoodBank.Web.Data.Models.Alert");

                    b.HasDiscriminator().HasValue("ApplicationAlert");
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.ShiftRequestAlert", b =>
                {
                    b.HasBaseType("MHFoodBank.Web.Data.Models.Alert");

                    b.Property<string>("AddressedBy")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("DismissedByAdmin")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("DismissedByVolunteer")
                        .HasColumnType("tinyint(1)");

                    b.Property<int?>("OriginalShiftId")
                        .HasColumnType("int");

                    b.Property<string>("Reason")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("RequestedShiftId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasIndex("OriginalShiftId");

                    b.HasIndex("RequestedShiftId");

                    b.HasDiscriminator().HasValue("ShiftRequestAlert");
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.RecurringShift", b =>
                {
                    b.HasBaseType("MHFoodBank.Web.Data.Models.Shift");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("RecurrenceRule")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Weekdays")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasDiscriminator().HasValue("RecurringShift");
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.Alert", b =>
                {
                    b.HasOne("MHFoodBank.Web.Data.Models.VolunteerProfile", "Volunteer")
                        .WithMany("Alerts")
                        .HasForeignKey("VolunteerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.Availability", b =>
                {
                    b.HasOne("MHFoodBank.Web.Data.Models.VolunteerProfile", "Volunteer")
                        .WithMany("Availabilities")
                        .HasForeignKey("VolunteerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.PositionVolunteer", b =>
                {
                    b.HasOne("MHFoodBank.Web.Data.Models.Position", "Position")
                        .WithMany()
                        .HasForeignKey("PositionId");

                    b.HasOne("MHFoodBank.Web.Data.Models.VolunteerProfile", "Volunteer")
                        .WithMany("Positions")
                        .HasForeignKey("VolunteerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.Reference", b =>
                {
                    b.HasOne("MHFoodBank.Web.Data.Models.VolunteerProfile", "Volunteer")
                        .WithMany("References")
                        .HasForeignKey("VolunteerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.Shift", b =>
                {
                    b.HasOne("MHFoodBank.Web.Data.Models.RecurringShift", "ParentRecurringShift")
                        .WithMany("ExcludedShifts")
                        .HasForeignKey("ParentRecurringShiftId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("MHFoodBank.Web.Data.Models.Position", "PositionWorked")
                        .WithMany()
                        .HasForeignKey("PositionWorkedId");

                    b.HasOne("MHFoodBank.Web.Data.Models.VolunteerProfile", "Volunteer")
                        .WithMany("Shifts")
                        .HasForeignKey("VolunteerId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.VolunteerProfile", b =>
                {
                    b.HasOne("MHFoodBank.Web.Data.AppUser", "User")
                        .WithOne("VolunteerProfile")
                        .HasForeignKey("MHFoodBank.Web.Data.Models.VolunteerProfile", "UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.WorkExperience", b =>
                {
                    b.HasOne("MHFoodBank.Web.Data.Models.VolunteerProfile", "Volunteer")
                        .WithMany("WorkExperiences")
                        .HasForeignKey("VolunteerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MHFoodBank.Web.Models.ClockedTime", b =>
                {
                    b.HasOne("MHFoodBank.Web.Data.Models.Position", "Position")
                        .WithMany()
                        .HasForeignKey("PositionId");

                    b.HasOne("MHFoodBank.Web.Data.Models.VolunteerProfile", "VolunteerProfile")
                        .WithMany()
                        .HasForeignKey("VolunteerProfileId");
                });

            modelBuilder.Entity("MHFoodBank.Web.Models.RecurringChildLink", b =>
                {
                    b.HasOne("MHFoodBank.Web.Data.Models.Shift", "NewShift")
                        .WithMany()
                        .HasForeignKey("NewShiftId");

                    b.HasOne("MHFoodBank.Web.Data.Models.Shift", "OriginalShift")
                        .WithMany()
                        .HasForeignKey("OriginalShiftId");

                    b.HasOne("MHFoodBank.Web.Data.Models.RecurringShift", "ParentSet")
                        .WithMany()
                        .HasForeignKey("ParentSetId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("MHFoodBank.Web.Data.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("MHFoodBank.Web.Data.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MHFoodBank.Web.Data.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("MHFoodBank.Web.Data.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MHFoodBank.Web.Data.Models.ShiftRequestAlert", b =>
                {
                    b.HasOne("MHFoodBank.Web.Data.Models.Shift", "OriginalShift")
                        .WithMany()
                        .HasForeignKey("OriginalShiftId");

                    b.HasOne("MHFoodBank.Web.Data.Models.Shift", "RequestedShift")
                        .WithMany()
                        .HasForeignKey("RequestedShiftId");
                });
#pragma warning restore 612, 618
        }
    }
}