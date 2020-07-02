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
    `FoodSafeExpiry` datetime(6) NOT NULL,
    `FirstAid` tinyint(1) NOT NULL,
    `FirstAidExpiry` datetime(6) NOT NULL,
    `Cpr` tinyint(1) NOT NULL,
    `CprExpiry` datetime(6) NOT NULL,
    `OtherCertificates` longtext CHARACTER SET utf8mb4 NULL,
    `EducationTraining` longtext CHARACTER SET utf8mb4 NOT NULL,
    `SkillsInterestsHobbies` longtext CHARACTER SET utf8mb4 NOT NULL,
    `VolunteerExperience` longtext CHARACTER SET utf8mb4 NOT NULL,
    `OtherBoards` longtext CHARACTER SET utf8mb4 NOT NULL,
    `VolunteerConfidentiality` tinyint(1) NOT NULL,
    `VolunteerEthics` tinyint(1) NOT NULL,
    `CriminalRecordCheck` tinyint(1) NOT NULL,
    `DrivingAbstract` tinyint(1) NOT NULL,
    `ConfirmationOfProfessionalDesignation` tinyint(1) NOT NULL,
    `ChildWelfareCheck` tinyint(1) NOT NULL,
    `OfficiallyApproved` tinyint(1) NOT NULL,
    `UserID` int NOT NULL,
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
    `VolunteerProfileId` int NULL,
    CONSTRAINT `PK_ClockedTime` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ClockedTime_Positions_PositionId` FOREIGN KEY (`PositionId`) REFERENCES `Positions` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ClockedTime_VolunteerProfiles_VolunteerProfileId` FOREIGN KEY (`VolunteerProfileId`) REFERENCES `VolunteerProfiles` (`Id`) ON DELETE RESTRICT
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
    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Phone` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Relationship` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Occupation` longtext CHARACTER SET utf8mb4 NOT NULL,
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
    `EmployerName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `EmployerAddress` longtext CHARACTER SET utf8mb4 NOT NULL,
    `StartDate` datetime(6) NOT NULL,
    `EndDate` datetime(6) NOT NULL,
    `EmployerPhone` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ContactPerson` longtext CHARACTER SET utf8mb4 NOT NULL,
    `PositionWorked` longtext CHARACTER SET utf8mb4 NOT NULL,
    `CurrentJob` tinyint(1) NOT NULL,
    CONSTRAINT `PK_WorkExperiences` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_WorkExperiences_VolunteerProfiles_VolunteerId` FOREIGN KEY (`VolunteerId`) REFERENCES `VolunteerProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Alerts` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `VolunteerId` int NULL,
    `Date` datetime(6) NOT NULL,
    `HasBeenRead` tinyint(1) NOT NULL,
    `Discriminator` longtext CHARACTER SET utf8mb4 NOT NULL,
    `OldShiftId` int NULL,
    `NewShiftId` int NULL,
    `Reason` longtext CHARACTER SET utf8mb4 NULL,
    `Status` int NULL,
    `DismissedByAdmin` tinyint(1) NULL,
    `DismissedByVolunteer` tinyint(1) NULL,
    `AddressedBy` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Alerts` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Alerts_VolunteerProfiles_VolunteerId` FOREIGN KEY (`VolunteerId`) REFERENCES `VolunteerProfiles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Alerts_Shifts_NewShiftId` FOREIGN KEY (`NewShiftId`) REFERENCES `Shifts` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Alerts_Shifts_OldShiftId` FOREIGN KEY (`OldShiftId`) REFERENCES `Shifts` (`Id`) ON DELETE RESTRICT
);

CREATE INDEX `IX_Alerts_VolunteerId` ON `Alerts` (`VolunteerId`);

CREATE INDEX `IX_Alerts_NewShiftId` ON `Alerts` (`NewShiftId`);

CREATE INDEX `IX_Alerts_OldShiftId` ON `Alerts` (`OldShiftId`);

CREATE INDEX `IX_AspNetRoleClaims_RoleId` ON `AspNetRoleClaims` (`RoleId`);

CREATE UNIQUE INDEX `RoleNameIndex` ON `AspNetRoles` (`NormalizedName`);

CREATE INDEX `IX_AspNetUserClaims_UserId` ON `AspNetUserClaims` (`UserId`);

CREATE INDEX `IX_AspNetUserLogins_UserId` ON `AspNetUserLogins` (`UserId`);

CREATE INDEX `IX_AspNetUserRoles_RoleId` ON `AspNetUserRoles` (`RoleId`);

CREATE INDEX `EmailIndex` ON `AspNetUsers` (`NormalizedEmail`);

CREATE UNIQUE INDEX `UserNameIndex` ON `AspNetUsers` (`NormalizedUserName`);

CREATE INDEX `IX_Availabilities_VolunteerId` ON `Availabilities` (`VolunteerId`);

CREATE INDEX `IX_ClockedTime_PositionId` ON `ClockedTime` (`PositionId`);

CREATE INDEX `IX_ClockedTime_VolunteerProfileId` ON `ClockedTime` (`VolunteerProfileId`);

CREATE INDEX `IX_PositionVolunteers_PositionId` ON `PositionVolunteers` (`PositionId`);

CREATE INDEX `IX_PositionVolunteers_VolunteerId` ON `PositionVolunteers` (`VolunteerId`);

CREATE INDEX `IX_References_VolunteerId` ON `References` (`VolunteerId`);

CREATE INDEX `IX_Shifts_ParentRecurringShiftId` ON `Shifts` (`ParentRecurringShiftId`);

CREATE INDEX `IX_Shifts_PositionWorkedId` ON `Shifts` (`PositionWorkedId`);

CREATE INDEX `IX_Shifts_VolunteerId` ON `Shifts` (`VolunteerId`);

CREATE UNIQUE INDEX `IX_VolunteerProfiles_UserID` ON `VolunteerProfiles` (`UserID`);

CREATE INDEX `IX_WorkExperiences_VolunteerId` ON `WorkExperiences` (`VolunteerId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200702163146_current', '3.1.3');

