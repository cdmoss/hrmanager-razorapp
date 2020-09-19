CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);

ALTER TABLE `Shifts` DROP FOREIGN KEY `FK_Shifts_Shifts_ParentRecurringShiftId`;

DROP TABLE `ShiftLinks`;

ALTER TABLE `Shifts` DROP INDEX `IX_Shifts_ParentRecurringShiftId`;

ALTER TABLE `Shifts` DROP COLUMN `EndDate`;

ALTER TABLE `Shifts` DROP COLUMN `Weekdays`;

ALTER TABLE `Shifts` DROP COLUMN `Discriminator`;

ALTER TABLE `Shifts` DROP COLUMN `Hidden`;

ALTER TABLE `Shifts` DROP COLUMN `ParentRecurringShiftId`;

ALTER TABLE `Shifts` DROP COLUMN `StartDate`;

ALTER TABLE `Shifts` MODIFY COLUMN `StartTime` datetime(6) NOT NULL;

ALTER TABLE `Shifts` MODIFY COLUMN `EndTime` datetime(6) NOT NULL;

ALTER TABLE `Shifts` ADD `RecurrenceException` longtext CHARACTER SET utf8mb4 NULL;

ALTER TABLE `Shifts` ADD `RecurrenceID` int NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200918001115_change_to_scheduler', '3.1.3');

