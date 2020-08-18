-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               10.4.12-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win64
-- HeidiSQL Version:             10.2.0.5599
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- Dumping database structure for foodbankhangfire
CREATE DATABASE IF NOT EXISTS `foodbankhangfire` /*!40100 DEFAULT CHARACTER SET latin1 */;

CREATE DATABASE IF NOT EXISTS foodbankdb /*!40100 DEFAULT CHARACTER SET utf8 */;
USE foodbankdb;

CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);

CREATE TABLE `AspNetRoles` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetRoles` PRIMARY KEY (`Id`)
);

CREATE TABLE `AspNetUsers` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `Email` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 NULL,
    `EmailConfirmed` tinyint(1) NOT NULL,
    `PasswordHash` longtext CHARACTER SET utf8mb4 NULL,
    `SecurityStamp` longtext CHARACTER SET utf8mb4 NULL,
    `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
    `TwoFactorEnabled` tinyint(1) NOT NULL,
    CONSTRAINT `PK_AspNetUsers` PRIMARY KEY (`Id`)
);

CREATE TABLE `Positions` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Positions` PRIMARY KEY (`Id`)
);

CREATE TABLE `Reminders` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ShiftId` int NOT NULL,
    `ShiftDate` datetime(6) NOT NULL,
    `HangfireJobId` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Reminders` PRIMARY KEY (`Id`)
);

CREATE TABLE `AspNetRoleClaims` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `RoleId` int NOT NULL,
    `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
    `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetRoleClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserClaims` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` int NOT NULL,
    `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
    `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetUserClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserLogins` (
    `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProviderKey` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProviderDisplayName` longtext CHARACTER SET utf8mb4 NULL,
    `UserId` int NOT NULL,
    CONSTRAINT `PK_AspNetUserLogins` PRIMARY KEY (`LoginProvider`, `ProviderKey`),
    CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserRoles` (
    `UserId` int NOT NULL,
    `RoleId` int NOT NULL,
    CONSTRAINT `PK_AspNetUserRoles` PRIMARY KEY (`UserId`, `RoleId`),
    CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserTokens` (
    `UserId` int NOT NULL,
    `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Value` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetUserTokens` PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
    CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `VolunteerProfiles` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `FirstName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `LastName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Address` longtext CHARACTER SET utf8mb4 NOT NULL,
    `City` longtext CHARACTER SET utf8mb4 NOT NULL,
    `PostalCode` longtext CHARACTER SET utf8mb4 NOT NULL,
    `MainPhone` longtext CHARACTER SET utf8mb4 NOT NULL,
    `AlternatePhone1` longtext CHARACTER SET utf8mb4 NULL,
    `AlternatePhone2` longtext CHARACTER SET utf8mb4 NULL,
    `Birthdate` datetime(6) NOT NULL,
    `EmergencyFullName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `EmergencyPhone1` longtext CHARACTER SET utf8mb4 NOT NULL,
    `EmergencyPhone2` longtext CHARACTER SET utf8mb4 NULL,
    `EmergencyRelationship` longtext CHARACTER SET utf8mb4 NOT NULL,
    `FoodSafe` tinyint(1) NOT NULL,
    `FoodSafeExpiry` datetime(6) NULL,
    `FirstAidCprLevel` longtext CHARACTER SET utf8mb4 NULL,
    `FirstAidCpr` tinyint(1) NOT NULL,
    `FirstAidCprExpiry` datetime(6) NULL,
    `OtherCertificates` longtext CHARACTER SET utf8mb4 NULL,
    `EducationTraining` longtext CHARACTER SET utf8mb4 NULL,
    `SkillsInterestsHobbies` longtext CHARACTER SET utf8mb4 NULL,
    `VolunteerExperience` longtext CHARACTER SET utf8mb4 NULL,
    `OtherBoards` longtext CHARACTER SET utf8mb4 NULL,
    `VolunteerConfidentiality` tinyint(1) NOT NULL,
    `VolunteerEthics` tinyint(1) NOT NULL,
    `CriminalRecordCheck` tinyint(1) NOT NULL,
    `DrivingAbstract` tinyint(1) NOT NULL,
    `ConfirmationOfProfessionalDesignation` tinyint(1) NOT NULL,
    `ChildWelfareCheck` tinyint(1) NOT NULL,
    `ApprovalStatus` int NOT NULL,
    `UserID` int NOT NULL,
    `Deleted` tinyint(1) NOT NULL,
    CONSTRAINT `PK_VolunteerProfiles` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_VolunteerProfiles_AspNetUsers_UserID` FOREIGN KEY (`UserID`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Availabilities` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `VolunteerId` int NULL,
    `StartTime` time(6) NOT NULL,
    `EndTime` time(6) NOT NULL,
    `AvailableDay` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Availabilities` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Availabilities_VolunteerProfiles_VolunteerId` FOREIGN KEY (`VolunteerId`) REFERENCES `VolunteerProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ClockedTime` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `StartTime` datetime(6) NOT NULL,
    `EndTime` datetime(6) NULL,
    `PositionId` int NULL,
    `VolunteerId` int NULL,
    CONSTRAINT `PK_ClockedTime` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ClockedTime_Positions_PositionId` FOREIGN KEY (`PositionId`) REFERENCES `Positions` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ClockedTime_VolunteerProfiles_VolunteerId` FOREIGN KEY (`VolunteerId`) REFERENCES `VolunteerProfiles` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `PositionVolunteers` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `VolunteerId` int NULL,
    `PositionId` int NULL,
    `Association` int NOT NULL,
    CONSTRAINT `PK_PositionVolunteers` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_PositionVolunteers_Positions_PositionId` FOREIGN KEY (`PositionId`) REFERENCES `Positions` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_PositionVolunteers_VolunteerProfiles_VolunteerId` FOREIGN KEY (`VolunteerId`) REFERENCES `VolunteerProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `References` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `VolunteerId` int NULL,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    `Phone` longtext CHARACTER SET utf8mb4 NULL,
    `Relationship` longtext CHARACTER SET utf8mb4 NULL,
    `Occupation` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_References` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_References_VolunteerProfiles_VolunteerId` FOREIGN KEY (`VolunteerId`) REFERENCES `VolunteerProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Shifts` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `StartDate` datetime(6) NOT NULL,
    `StartTime` time(6) NOT NULL,
    `EndTime` time(6) NOT NULL,
    `PositionWorkedId` int NULL,
    `VolunteerId` int NULL,
    `ParentRecurringShiftId` int NULL,
    `Description` longtext CHARACTER SET utf8mb4 NULL,
    `Hidden` tinyint(1) NOT NULL,
    `Discriminator` longtext CHARACTER SET utf8mb4 NOT NULL,
    `EndDate` datetime(6) NULL,
    `Weekdays` longtext CHARACTER SET utf8mb4 NULL,
    `RecurrenceRule` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Shifts` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Shifts_Shifts_ParentRecurringShiftId` FOREIGN KEY (`ParentRecurringShiftId`) REFERENCES `Shifts` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Shifts_Positions_PositionWorkedId` FOREIGN KEY (`PositionWorkedId`) REFERENCES `Positions` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Shifts_VolunteerProfiles_VolunteerId` FOREIGN KEY (`VolunteerId`) REFERENCES `VolunteerProfiles` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `WorkExperiences` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `VolunteerId` int NULL,
    `EmployerName` longtext CHARACTER SET utf8mb4 NULL,
    `EmployerAddress` longtext CHARACTER SET utf8mb4 NULL,
    `StartDate` datetime(6) NOT NULL,
    `EndDate` datetime(6) NOT NULL,
    `EmployerPhone` longtext CHARACTER SET utf8mb4 NULL,
    `ContactPerson` longtext CHARACTER SET utf8mb4 NULL,
    `PositionWorked` longtext CHARACTER SET utf8mb4 NULL,
    `CurrentJob` tinyint(1) NOT NULL,
    CONSTRAINT `PK_WorkExperiences` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_WorkExperiences_VolunteerProfiles_VolunteerId` FOREIGN KEY (`VolunteerId`) REFERENCES `VolunteerProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Alerts` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `VolunteerId` int NULL,
    `Date` datetime(6) NOT NULL,
    `Read` tinyint(1) NOT NULL,
    `Deleted` tinyint(1) NOT NULL,
    `AlertType` longtext CHARACTER SET utf8mb4 NULL,
    `Discriminator` longtext CHARACTER SET utf8mb4 NOT NULL,
    `OriginalShiftId` int NULL,
    `RequestedShiftId` int NULL,
    `Reason` longtext CHARACTER SET utf8mb4 NULL,
    `Status` int NULL,
    `DismissedByAdmin` tinyint(1) NULL,
    `DismissedByVolunteer` tinyint(1) NULL,
    `AddressedBy` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Alerts` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Alerts_VolunteerProfiles_VolunteerId` FOREIGN KEY (`VolunteerId`) REFERENCES `VolunteerProfiles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Alerts_Shifts_OriginalShiftId` FOREIGN KEY (`OriginalShiftId`) REFERENCES `Shifts` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Alerts_Shifts_RequestedShiftId` FOREIGN KEY (`RequestedShiftId`) REFERENCES `Shifts` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `ShiftLinks` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ParentSetId` int NULL,
    `OriginalShiftId` int NULL,
    `NewShiftId` int NULL,
    CONSTRAINT `PK_ShiftLinks` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ShiftLinks_Shifts_NewShiftId` FOREIGN KEY (`NewShiftId`) REFERENCES `Shifts` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ShiftLinks_Shifts_OriginalShiftId` FOREIGN KEY (`OriginalShiftId`) REFERENCES `Shifts` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ShiftLinks_Shifts_ParentSetId` FOREIGN KEY (`ParentSetId`) REFERENCES `Shifts` (`Id`) ON DELETE RESTRICT
);

CREATE INDEX `IX_Alerts_VolunteerId` ON `Alerts` (`VolunteerId`);

CREATE INDEX `IX_Alerts_OriginalShiftId` ON `Alerts` (`OriginalShiftId`);

CREATE INDEX `IX_Alerts_RequestedShiftId` ON `Alerts` (`RequestedShiftId`);

CREATE INDEX `IX_AspNetRoleClaims_RoleId` ON `AspNetRoleClaims` (`RoleId`);

CREATE UNIQUE INDEX `RoleNameIndex` ON `AspNetRoles` (`NormalizedName`);

CREATE INDEX `IX_AspNetUserClaims_UserId` ON `AspNetUserClaims` (`UserId`);

CREATE INDEX `IX_AspNetUserLogins_UserId` ON `AspNetUserLogins` (`UserId`);

CREATE INDEX `IX_AspNetUserRoles_RoleId` ON `AspNetUserRoles` (`RoleId`);

CREATE INDEX `EmailIndex` ON `AspNetUsers` (`NormalizedEmail`);

CREATE UNIQUE INDEX `UserNameIndex` ON `AspNetUsers` (`NormalizedUserName`);

CREATE INDEX `IX_Availabilities_VolunteerId` ON `Availabilities` (`VolunteerId`);

CREATE INDEX `IX_ClockedTime_PositionId` ON `ClockedTime` (`PositionId`);

CREATE INDEX `IX_ClockedTime_VolunteerId` ON `ClockedTime` (`VolunteerId`);

CREATE INDEX `IX_PositionVolunteers_PositionId` ON `PositionVolunteers` (`PositionId`);

CREATE INDEX `IX_PositionVolunteers_VolunteerId` ON `PositionVolunteers` (`VolunteerId`);

CREATE INDEX `IX_References_VolunteerId` ON `References` (`VolunteerId`);

CREATE INDEX `IX_ShiftLinks_NewShiftId` ON `ShiftLinks` (`NewShiftId`);

CREATE INDEX `IX_ShiftLinks_OriginalShiftId` ON `ShiftLinks` (`OriginalShiftId`);

CREATE INDEX `IX_ShiftLinks_ParentSetId` ON `ShiftLinks` (`ParentSetId`);

CREATE INDEX `IX_Shifts_ParentRecurringShiftId` ON `Shifts` (`ParentRecurringShiftId`);

CREATE INDEX `IX_Shifts_PositionWorkedId` ON `Shifts` (`PositionWorkedId`);

CREATE INDEX `IX_Shifts_VolunteerId` ON `Shifts` (`VolunteerId`);

CREATE UNIQUE INDEX `IX_VolunteerProfiles_UserID` ON `VolunteerProfiles` (`UserID`);

CREATE INDEX `IX_WorkExperiences_VolunteerId` ON `WorkExperiences` (`VolunteerId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200817152729_init', '3.1.3');

