CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);

CREATE TABLE `AspNetRoles` (
    `Id` char(36) NOT NULL,
    `Name` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetRoles` PRIMARY KEY (`Id`)
);

CREATE TABLE `AspNetUsers` (
    `Id` char(36) NOT NULL,
    `UserName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `Email` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 NULL,
    `EmailConfirmed` tinyint(1) NOT NULL,
    `PasswordHash` longtext CHARACTER SET utf8mb4 NULL,
    `SecurityStamp` longtext CHARACTER SET utf8mb4 NULL,
    `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
    `PhoneNumber` longtext CHARACTER SET utf8mb4 NULL,
    `PhoneNumberConfirmed` tinyint(1) NOT NULL,
    `TwoFactorEnabled` tinyint(1) NOT NULL,
    `LockoutEnd` datetime(6) NULL,
    `LockoutEnabled` tinyint(1) NOT NULL,
    `AccessFailedCount` int NOT NULL,
    `FirstName` longtext CHARACTER SET utf8mb4 NULL,
    `LastName` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetUsers` PRIMARY KEY (`Id`)
);

CREATE TABLE `Position` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Position` PRIMARY KEY (`Id`)
);

CREATE TABLE `AspNetRoleClaims` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `RoleId` char(36) NOT NULL,
    `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
    `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetRoleClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserClaims` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` char(36) NOT NULL,
    `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
    `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetUserClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserLogins` (
    `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProviderKey` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProviderDisplayName` longtext CHARACTER SET utf8mb4 NULL,
    `UserId` char(36) NOT NULL,
    CONSTRAINT `PK_AspNetUserLogins` PRIMARY KEY (`LoginProvider`, `ProviderKey`),
    CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserRoles` (
    `UserId` char(36) NOT NULL,
    `RoleId` char(36) NOT NULL,
    CONSTRAINT `PK_AspNetUserRoles` PRIMARY KEY (`UserId`, `RoleId`),
    CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserTokens` (
    `UserId` char(36) NOT NULL,
    `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Value` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetUserTokens` PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
    CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Availabilities` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `VolunteerId` char(36) NULL,
    `StartTime` time(6) NOT NULL,
    `EndTime` time(6) NOT NULL,
    `AvailableDay` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Availabilities` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Availabilities_AspNetUsers_VolunteerId` FOREIGN KEY (`VolunteerId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `References` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` char(36) NULL,
    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Phone` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Relationship` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Occupation` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_References` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_References_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `WorkExperiences` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` char(36) NULL,
    `EmployerName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `EmployerAddress` longtext CHARACTER SET utf8mb4 NOT NULL,
    `StartDate` datetime(6) NOT NULL,
    `EndDate` datetime(6) NOT NULL,
    `EmployerPhone` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ContactPerson` longtext CHARACTER SET utf8mb4 NOT NULL,
    `PositionWorked` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_WorkExperiences` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_WorkExperiences_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `AccountPositions` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` char(36) NULL,
    `PositionId` int NULL,
    `Association` int NOT NULL,
    CONSTRAINT `PK_AccountPositions` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AccountPositions_Position_PositionId` FOREIGN KEY (`PositionId`) REFERENCES `Position` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_AccountPositions_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Shifts` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `StartDate` datetime(6) NOT NULL,
    `StartTime` time(6) NOT NULL,
    `EndTime` time(6) NOT NULL,
    `PositionWorkedId` int NULL,
    `VolunteerId` char(36) NULL,
    `ParentRecurringShiftId` int NULL,
    `Description` longtext CHARACTER SET utf8mb4 NULL,
    `Hidden` tinyint(1) NOT NULL,
    `Discriminator` longtext CHARACTER SET utf8mb4 NOT NULL,
    `EndDate` datetime(6) NULL,
    `Weekdays` longtext CHARACTER SET utf8mb4 NULL,
    `RecurrenceRule` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Shifts` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Shifts_Shifts_ParentRecurringShiftId` FOREIGN KEY (`ParentRecurringShiftId`) REFERENCES `Shifts` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Shifts_Position_PositionWorkedId` FOREIGN KEY (`PositionWorkedId`) REFERENCES `Position` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Shifts_AspNetUsers_VolunteerId` FOREIGN KEY (`VolunteerId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Alerts` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` char(36) NULL,
    `Date` datetime(6) NOT NULL,
    `HasBeenRead` tinyint(1) NOT NULL,
    `Discriminator` longtext CHARACTER SET utf8mb4 NOT NULL,
    `OldShiftId` int NULL,
    `NewShiftId` int NULL,
    `Reason` longtext CHARACTER SET utf8mb4 NULL,
    `Status` int NULL,
    `DismissedByAdmin` tinyint(1) NULL,
    `DismissedByVolunteer` tinyint(1) NULL,
    CONSTRAINT `PK_Alerts` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Alerts_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Alerts_Shifts_NewShiftId` FOREIGN KEY (`NewShiftId`) REFERENCES `Shifts` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Alerts_Shifts_OldShiftId` FOREIGN KEY (`OldShiftId`) REFERENCES `Shifts` (`Id`) ON DELETE RESTRICT
);

INSERT INTO `AspNetRoles` (`Id`, `ConcurrencyStamp`, `Name`, `NormalizedName`)
VALUES ('3a9b9375-4db9-4597-adad-3eda2f1cd29b', 'e03caaf6-3f08-48de-b840-c2056188f5e9', 'Member', 'Member');
INSERT INTO `AspNetRoles` (`Id`, `ConcurrencyStamp`, `Name`, `NormalizedName`)
VALUES ('206f3af5-2fa0-40ff-b95a-303a7c24ddc6', '5e6452e9-b40b-4d40-9302-d3c589cb4637', 'Staff', 'STAFF');
INSERT INTO `AspNetRoles` (`Id`, `ConcurrencyStamp`, `Name`, `NormalizedName`)
VALUES ('222f0447-e59b-4cd5-8027-e97110df82bf', 'df38e007-b485-4c86-b8e4-eb33427808ed', 'Admin', 'ADMIN');

INSERT INTO `Position` (`Id`, `Name`)
VALUES (6, 'Front Stock');
INSERT INTO `Position` (`Id`, `Name`)
VALUES (1, 'Warehouse');
INSERT INTO `Position` (`Id`, `Name`)
VALUES (2, 'Janitorial');
INSERT INTO `Position` (`Id`, `Name`)
VALUES (3, 'General Maintenance');
INSERT INTO `Position` (`Id`, `Name`)
VALUES (4, 'Special Events');
INSERT INTO `Position` (`Id`, `Name`)
VALUES (5, 'Community Relations');

CREATE INDEX `IX_AccountPositions_PositionId` ON `AccountPositions` (`PositionId`);

CREATE INDEX `IX_AccountPositions_UserId` ON `AccountPositions` (`UserId`);

CREATE INDEX `IX_Alerts_UserId` ON `Alerts` (`UserId`);

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

CREATE INDEX `IX_References_UserId` ON `References` (`UserId`);

CREATE INDEX `IX_Shifts_ParentRecurringShiftId` ON `Shifts` (`ParentRecurringShiftId`);

CREATE INDEX `IX_Shifts_PositionWorkedId` ON `Shifts` (`PositionWorkedId`);

CREATE INDEX `IX_Shifts_VolunteerId` ON `Shifts` (`VolunteerId`);

CREATE INDEX `IX_WorkExperiences_UserId` ON `WorkExperiences` (`UserId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200701004128_current', '3.1.5');

