SET GLOBAL log_bin_trust_function_creators = 1;
-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema IntelliTrackSolutionsDB
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema IntelliTrackSolutionsDB
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `IntelliTrackSolutionsDB` DEFAULT CHARACTER SET utf8 ;
SHOW WARNINGS;
USE `IntelliTrackSolutionsDB` ;

-- -----------------------------------------------------
-- Table `IntelliTrackSolutionsDB`.`InformationSystem`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `IntelliTrackSolutionsDB`.`InformationSystem` (
  `idInformationSystem` INT NOT NULL AUTO_INCREMENT,
  `Title` NVARCHAR(50) NOT NULL COMMENT 'Название информационной системы.',
  `Description` NVARCHAR(500) NOT NULL COMMENT 'Описание информационной системы.',
  `ApiKey` TEXT NOT NULL COMMENT 'Ключ апи для работы с данной информационной системой.',
  PRIMARY KEY (`idInformationSystem`))
ENGINE = InnoDB;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `IntelliTrackSolutionsDB`.`AccessLevel`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `IntelliTrackSolutionsDB`.`AccessLevel` (
  `idAccessLevel` INT NOT NULL AUTO_INCREMENT,
  `Permission` TINYINT NOT NULL COMMENT 'Уровень доступа в информационной системе.',
  `InformationSystemId` INT NOT NULL COMMENT 'Идентификатор информационной системы к которой принадлежит лист доступа.',
  PRIMARY KEY (`idAccessLevel`),
  INDEX `fk_AccessLevel_InformationSystem_idx` (`InformationSystemId` ASC) INVISIBLE,
  UNIQUE INDEX `uq_System_Permission` (`InformationSystemId` ASC, `Permission` ASC) INVISIBLE,
  CONSTRAINT `fk_AccessList_InformationSystem`
    FOREIGN KEY (`InformationSystemId`)
    REFERENCES `IntelliTrackSolutionsDB`.`InformationSystem` (`idInformationSystem`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `IntelliTrackSolutionsDB`.`User`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `IntelliTrackSolutionsDB`.`User` (
  `idUser` INT NOT NULL AUTO_INCREMENT,
  `Login` NVARCHAR(100) NOT NULL COMMENT 'Логин пользователя.',
  `Password` NVARCHAR(255) NOT NULL COMMENT 'Пароль пользователя.',
  `AccessLevelId` INT NULL COMMENT 'Идентификатор листа доступа пользователя, необходим для получения соответствующей уровню доступа информации.',
  PRIMARY KEY (`idUser`),
  UNIQUE INDEX `uq_login_password` (`Password` ASC, `Login` ASC) VISIBLE,
  INDEX `fk_User_AccessLevel_idx` (`AccessLevelId` ASC) VISIBLE,
  CONSTRAINT `fk_User_AccessList`
    FOREIGN KEY (`AccessLevelId`)
    REFERENCES `IntelliTrackSolutionsDB`.`AccessLevel` (`idAccessLevel`)
    ON DELETE SET NULL
    ON UPDATE CASCADE)
ENGINE = InnoDB;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `IntelliTrackSolutionsDB`.`TwoFactorAuthentification`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `IntelliTrackSolutionsDB`.`TwoFactorAuthentification` (
  `idTwoFactorAuthentification` INT NOT NULL AUTO_INCREMENT,
  `UserId` INT NOT NULL COMMENT 'Идентификатор связанного пользователя.',
  `Code` NVARCHAR(6) NOT NULL COMMENT 'Код двухфакторной авторизации.',
  `LastUpdateTime` DATETIME NOT NULL DEFAULT NOW() COMMENT 'Последнее обновление кода двухфакторной авторизации.',
  PRIMARY KEY (`idTwoFactorAuthentification`, `UserId`),
  INDEX `fk_TwoFactorAuthentification_User_idx` (`UserId` ASC) VISIBLE,
  UNIQUE INDEX `User_idUser_UNIQUE` (`UserId` ASC) VISIBLE,
  CONSTRAINT `fk_TwoFactorAuthentification_User`
    FOREIGN KEY (`UserId`)
    REFERENCES `IntelliTrackSolutionsDB`.`User` (`idUser`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `IntelliTrackSolutionsDB`.`Role`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `IntelliTrackSolutionsDB`.`Role` (
  `idRole` INT NOT NULL AUTO_INCREMENT,
  `AccessLevelId` INT NOT NULL COMMENT 'Привязанный к роли лист доступа.',
  `Title` NVARCHAR(50) NOT NULL COMMENT 'Название роли.',
  `Description` NVARCHAR(400) NOT NULL COMMENT 'Описание роли.',
  PRIMARY KEY (`idRole`, `AccessLevelId`),
  INDEX `fk_Role_AccessLevel_idx` (`AccessLevelId` ASC) VISIBLE,
  UNIQUE INDEX `AccessLevel_idAccessLevel_UNIQUE` (`AccessLevelId` ASC) VISIBLE,
  CONSTRAINT `fk_Role_AccessList`
    FOREIGN KEY (`AccessLevelId`)
    REFERENCES `IntelliTrackSolutionsDB`.`AccessLevel` (`idAccessLevel`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `IntelliTrackSolutionsDB`.`SystemObject`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `IntelliTrackSolutionsDB`.`SystemObject` (
  `idSystemObject` INT NOT NULL AUTO_INCREMENT,
  `Name` NVARCHAR(50) NOT NULL COMMENT 'Наименование системного объекта.',
  `Description` NVARCHAR(400) NOT NULL COMMENT 'Описание системного объекта.',
  `Condition` ENUM('Faulty', 'Serviceable', 'Disabled') NOT NULL DEFAULT 'Serviceable' COMMENT 'Состояние системного объекта.',
  `DataRegistration` DATETIME NOT NULL DEFAULT NOW() COMMENT 'Дата регистрации системного объекта.',
  `LastUpdate` DATETIME NOT NULL DEFAULT NOW() COMMENT 'Срок последнего обновления информации о системном объекте.',
  `InformationSystemId` INT NOT NULL COMMENT 'Идентификатор информационной системы к которой принадлежит системный объект.',
  PRIMARY KEY (`idSystemObject`),
  UNIQUE INDEX `uq_Name_idSystemObject` (`Name` ASC, `idSystemObject` ASC) VISIBLE,
  INDEX `fk_SystemObject_InformationSystem_idx` (`InformationSystemId` ASC) VISIBLE,
  CONSTRAINT `fk_SystemObject_InformationSystem`
    FOREIGN KEY (`InformationSystemId`)
    REFERENCES `IntelliTrackSolutionsDB`.`InformationSystem` (`idInformationSystem`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `IntelliTrackSolutionsDB`.`Task`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `IntelliTrackSolutionsDB`.`Task` (
  `idTask` INT NOT NULL AUTO_INCREMENT,
  `InformationSystemId` INT NOT NULL COMMENT 'Идентификатор информационной системы к которой принадлежит задача.',
  `Title` NVARCHAR(50) NOT NULL COMMENT 'Название задачи.',
  `Goal` NVARCHAR(400) NOT NULL COMMENT 'Цель задачи.',
  `DataRegistration` DATETIME NOT NULL DEFAULT NOW() COMMENT 'Дата регистрации задачи.',
  `LastUpdate` DATETIME NOT NULL DEFAULT NOW() COMMENT 'Последнее обновление задачи.',
  `Status` ENUM('Completed', 'PendingExecution', 'NotCompleted') NOT NULL DEFAULT 'PendingExecution' COMMENT 'Статус выполнения задачи.',
  `Deadline` DATETIME NOT NULL COMMENT 'Срок до которого нужно выполнить задачу.',
  PRIMARY KEY (`idTask`, `InformationSystemId`),
  INDEX `fk_Task_InformationSystem_idx` (`InformationSystemId` ASC) VISIBLE,
  CONSTRAINT `fk_Task_InformationSystem`
    FOREIGN KEY (`InformationSystemId`)
    REFERENCES `IntelliTrackSolutionsDB`.`InformationSystem` (`idInformationSystem`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `IntelliTrackSolutionsDB`.`Location`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `IntelliTrackSolutionsDB`.`Location` (
  `idLocation` INT NOT NULL AUTO_INCREMENT,
  `SystemObjectId` INT NOT NULL COMMENT 'Идентификатор системного объекта, к которому принадлежат данные кординаты.',
  `Latitude` FLOAT NOT NULL CHECK(Latitude >= -90.0 AND Latitude <= 90.0) COMMENT 'Широта.',
  `Longitude` FLOAT NOT NULL CHECK(Longitude >= -180.0 AND Longitude <= 180.0) COMMENT 'Долгота.',
  PRIMARY KEY (`idLocation`, `SystemObjectId`),
  INDEX `fk_Location_SystemObject_idx` (`SystemObjectId` ASC) VISIBLE,
  UNIQUE INDEX `uq_longitude_latitude` (`Longitude` ASC, `Latitude` ASC, `SystemObjectId` ASC) VISIBLE,
  CONSTRAINT `fk_Location_SystemObject`
    FOREIGN KEY (`SystemObjectId`)
    REFERENCES `IntelliTrackSolutionsDB`.`SystemObject` (`idSystemObject`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `IntelliTrackSolutionsDB`.`InformationUser`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `IntelliTrackSolutionsDB`.`InformationUser` (
  `idInformationUser` INT NOT NULL AUTO_INCREMENT,
  `UserId` INT NOT NULL COMMENT 'Идентификатор связанного пользователя.',
  `FirstName` NVARCHAR(50) NOT NULL COMMENT 'Имя пользователя.',
  `LastName` NVARCHAR(50) NOT NULL COMMENT 'Фамилия пользователя.',
  `MiddleName` NVARCHAR(50) NULL COMMENT 'Отчество пользователя.',
  `Avatar` TEXT NOT NULL,
  PRIMARY KEY (`idInformationUser`, `UserId`),
  INDEX `fk_InformationUser_User_idx` (`UserId` ASC) INVISIBLE,
  UNIQUE INDEX `User_idUser_UNIQUE` (`UserId` ASC) VISIBLE,
  CONSTRAINT `fk_InformationUser_User1`
    FOREIGN KEY (`UserId`)
    REFERENCES `IntelliTrackSolutionsDB`.`User` (`idUser`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `IntelliTrackSolutionsDB`.`Chat`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `IntelliTrackSolutionsDB`.`Chat` (
  `idChat` INT NOT NULL AUTO_INCREMENT,
  `Title` NVARCHAR(50) NOT NULL DEFAULT 'Название чата...' COMMENT 'Название чата.',
  `InformationSystemId` INT NOT NULL COMMENT 'Идентификатор информационной системы, к которой принадлежит чат.',
  PRIMARY KEY (`idChat`),
  INDEX `fk_Chat_InformationSystem_idx` (`InformationSystemId` ASC) VISIBLE,
  CONSTRAINT `fk_Chat_InformationSystem`
    FOREIGN KEY (`InformationSystemId`)
    REFERENCES `IntelliTrackSolutionsDB`.`InformationSystem` (`idInformationSystem`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `IntelliTrackSolutionsDB`.`Message`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `IntelliTrackSolutionsDB`.`Message` (
  `idMessage` INT NOT NULL AUTO_INCREMENT,
  `ChatId` INT NOT NULL COMMENT 'Идентификатор чата, в котором находится данное сообщение.',
  `TextMessage` TEXT NOT NULL COMMENT 'Текст сообщения.',
  `AuthorId` INT NOT NULL COMMENT 'Идентификатор автора сообщения.',
  PRIMARY KEY (`idMessage`, `ChatId`),
  INDEX `fk_Message_Chat_idx` (`ChatId` ASC) VISIBLE,
  INDEX `fk_Message_User_idx` (`AuthorId` ASC) VISIBLE,
  CONSTRAINT `fk_Message_Chat`
    FOREIGN KEY (`ChatId`)
    REFERENCES `IntelliTrackSolutionsDB`.`Chat` (`idChat`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Message_User`
    FOREIGN KEY (`AuthorId`)
    REFERENCES `IntelliTrackSolutionsDB`.`User` (`idUser`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;

SHOW WARNINGS;
USE `IntelliTrackSolutionsDB` ;

-- -----------------------------------------------------
-- Placeholder table for view `IntelliTrackSolutionsDB`.`UserInformation_View`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `IntelliTrackSolutionsDB`.`UserInformation_View` (`idUser` INT, `FirstName` INT, `LastName` INT, `FullName` INT, `RoleTitle` INT, `Permission` INT);
SHOW WARNINGS;

-- -----------------------------------------------------
-- Placeholder table for view `IntelliTrackSolutionsDB`.`SystemObjectInfo_View`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `IntelliTrackSolutionsDB`.`SystemObjectInfo_View` (`idSystemObject` INT, `InformationSystemTitle` INT, `Name` INT, `Description` INT, `Condition` INT, `DataRegistration` INT);
SHOW WARNINGS;

-- -----------------------------------------------------
-- Placeholder table for view `IntelliTrackSolutionsDB`.`Faulty_SystemObjects_View`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `IntelliTrackSolutionsDB`.`Faulty_SystemObjects_View` (`idSystemObject` INT, `Name` INT, `Description` INT, `Latitude` INT, `Longitude` INT);
SHOW WARNINGS;

-- -----------------------------------------------------
-- Placeholder table for view `IntelliTrackSolutionsDB`.`Serviceable_SystemObjects_View`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `IntelliTrackSolutionsDB`.`Serviceable_SystemObjects_View` (`idSystemObject` INT, `Name` INT, `Description` INT, `InformationSystemTitle` INT);
SHOW WARNINGS;

-- -----------------------------------------------------
-- function DecryptPassword
-- -----------------------------------------------------

DELIMITER $$
USE `IntelliTrackSolutionsDB`$$
CREATE FUNCTION DecryptPassword(
    encryptPassword NVARCHAR(255)
)
RETURNS NVARCHAR(255)
READS SQL DATA
DETERMINISTIC
BEGIN
    DECLARE outputString VARCHAR(255);
    DECLARE length INT;
    DECLARE i INT;
    
    SET length = CHAR_LENGTH(encryptPassword);
    SET outputString = '';
    SET i = 1;
    
    WHILE i <= length DO
        IF i < length THEN
            SET outputString = CONCAT(outputString, SUBSTRING(encryptPassword, i+1, 1));
        END IF;
        SET outputString = CONCAT(outputString, SUBSTRING(encryptPassword, i, 1));
        
        SET i = i + 2;
    END WHILE;
    
    RETURN outputString;
END$$

DELIMITER ;
SHOW WARNINGS;

-- -----------------------------------------------------
-- function EncryptPassword
-- -----------------------------------------------------

DELIMITER $$
USE `IntelliTrackSolutionsDB`$$
CREATE FUNCTION EncryptPassword(
    password NVARCHAR(255)
)
RETURNS NVARCHAR(255)
READS SQL DATA
DETERMINISTIC
BEGIN
   DECLARE outputString VARCHAR(255);
    DECLARE length INT;
    DECLARE i INT;
    
    SET length = CHAR_LENGTH(password);
    SET outputString = '';
    SET i = 1;
    
    WHILE i <= length DO
        IF i < length THEN
            SET outputString = CONCAT(outputString, SUBSTRING(password, i+1, 1));
        END IF;
        SET outputString = CONCAT(outputString, SUBSTRING(password, i, 1));
        
        SET i = i + 2;
    END WHILE;
    
    RETURN outputString;
END$$

DELIMITER ;
SHOW WARNINGS;

-- -----------------------------------------------------
-- procedure InsertRoleWithAccessLevel
-- -----------------------------------------------------

DELIMITER $$
USE `IntelliTrackSolutionsDB`$$
CREATE PROCEDURE InsertRoleWithAccessLevel(IN accessLevel INT,IN permission TINYINT, IN idInfoSystem INT)
BEGIN
	CASE
        WHEN (permission = 10) THEN
        BEGIN
            INSERT INTO Role (`AccessLevelId`, `Title`, `Description`) VALUES (accessLevel, 'Владелец', 'Имеют все права в организации и доступ ко всем данным организации.');
        END;
        WHEN (permission >= 5) THEN
        BEGIN
            INSERT INTO Role (`AccessLevelId`, `Title`, `Description`) VALUES (accessLevel, 'Администратор', 'Имеют почти все права в организации и доступ ко всем данным организации.');
        END;
        WHEN (permission < 5) THEN
        BEGIN
			INSERT INTO Role (`AccessLevelId`, `Title`, `Description`) VALUES (accessLevel, 'Сотрудник', 'Почти ничего не может в организации и также не имеет доступа к полным данным организации.');
		END;
        ELSE BEGIN END;
    END CASE;
END$$

DELIMITER ;
SHOW WARNINGS;

-- -----------------------------------------------------
-- procedure UpdateStatus
-- -----------------------------------------------------

DELIMITER $$
USE `IntelliTrackSolutionsDB`$$
CREATE PROCEDURE UpdateStatus(IN LastUpdate DATETIME, IN DeadLine DATETIME, INOUT Status ENUM('Completed', 'PendingExecution', 'NotCompleted'))
BEGIN
    IF (LastUpdate >= DeadLine AND Status != 'Completed') THEN
        SET Status = 'NotCompleted';
    END IF;
END$$

DELIMITER ;
SHOW WARNINGS;

-- -----------------------------------------------------
-- View `IntelliTrackSolutionsDB`.`UserInformation_View`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `IntelliTrackSolutionsDB`.`UserInformation_View`;
SHOW WARNINGS;
USE `IntelliTrackSolutionsDB`;
CREATE  OR REPLACE VIEW UserInformation_View AS
SELECT
  u.idUser,
  i.FirstName,
  i.LastName,
  CONCAT(i.FirstName, ' ', i.LastName, IFNULL(CONCAT(' ', i.MiddleName), '')) AS FullName,
  IFNULL(b.Title, 'Не назначена') AS RoleTitle,
  IFNULL(a.Permission, '0') AS Permission
FROM User u
  JOIN InformationUser i ON u.idUser = i.UserId
  LEFT JOIN AccessLevel a ON u.AccessLevelId = a.idAccessLevel
  LEFT JOIN Role b USING (AccessLevelId)
  LEFT JOIN InformationSystem ins ON ins.idInformationSystem = a.InformationSystemId;
SHOW WARNINGS;

-- -----------------------------------------------------
-- View `IntelliTrackSolutionsDB`.`SystemObjectInfo_View`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `IntelliTrackSolutionsDB`.`SystemObjectInfo_View`;
SHOW WARNINGS;
USE `IntelliTrackSolutionsDB`;
CREATE  OR REPLACE VIEW SystemObjectInfo_View AS
    SELECT 
        so.idSystemObject,
        isys.Title AS InformationSystemTitle,
        so.Name,
        so.Description,
        so.Condition,
        so.DataRegistration
    FROM SystemObject so
	JOIN InformationSystem isys ON so.InformationSystemId = isys.idInformationSystem;
SHOW WARNINGS;

-- -----------------------------------------------------
-- View `IntelliTrackSolutionsDB`.`Faulty_SystemObjects_View`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `IntelliTrackSolutionsDB`.`Faulty_SystemObjects_View`;
SHOW WARNINGS;
USE `IntelliTrackSolutionsDB`;
CREATE  OR REPLACE VIEW Faulty_SystemObjects_View AS
    SELECT 
        so.idSystemObject,
        so.Name,
        so.Description,
        loc.Latitude,
        loc.Longitude
    FROM SystemObject so
            JOIN Location loc ON so.idSystemObject = SystemObjectId
    WHERE so.Condition = 'Faulty' OR 'Disabled';
SHOW WARNINGS;

-- -----------------------------------------------------
-- View `IntelliTrackSolutionsDB`.`Serviceable_SystemObjects_View`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `IntelliTrackSolutionsDB`.`Serviceable_SystemObjects_View`;
SHOW WARNINGS;
USE `IntelliTrackSolutionsDB`;
CREATE  OR REPLACE VIEW Serviceable_SystemObjects_View AS
    SELECT 
        so.idSystemObject,
        so.Name,
        so.Description,
        isys.Title AS InformationSystemTitle
    FROM SystemObject so
		JOIN InformationSystem isys ON so.InformationSystemId = isys.idInformationSystem
WHERE so.Condition = 'Serviceable';
SHOW WARNINGS;
USE `IntelliTrackSolutionsDB`;

DELIMITER $$
SHOW WARNINGS$$
USE `IntelliTrackSolutionsDB`$$
CREATE TRIGGER `IntelliTrackSolutionsDB`.`User_BEFORE_INSERT` BEFORE INSERT ON `User` FOR EACH ROW
BEGIN
    SET NEW.Password = (SELECT EncryptPassword(NEW.Password));
END$$

SHOW WARNINGS$$
SHOW WARNINGS$$
USE `IntelliTrackSolutionsDB`$$
CREATE TRIGGER `IntelliTrackSolutionsDB`.`User_BEFORE_UPDATE` BEFORE UPDATE ON `User` FOR EACH ROW
BEGIN
	IF NEW.Password != OLD.Password 
    THEN
        SET NEW.Password = (SELECT EncryptPassword(NEW.Password));
    END IF;
END$$

SHOW WARNINGS$$
SHOW WARNINGS$$
USE `IntelliTrackSolutionsDB`$$
CREATE
TRIGGER `IntelliTrackSolutionsDB`.`Task_BEFORE_UPDATE`
BEFORE UPDATE ON `IntelliTrackSolutionsDB`.`Task`
FOR EACH ROW
BEGIN
	CALL UpdateStatus(NEW.LastUpdate, NEW.DeadLine, NEW.Status);
END$$

SHOW WARNINGS$$
SHOW WARNINGS$$
USE `IntelliTrackSolutionsDB`$$
CREATE TRIGGER `AccessLevel_AFTER_INSERT` AFTER INSERT ON `AccessLevel`
 FOR EACH ROW BEGIN
	CALL InsertRoleWithAccessLevel(NEW.idAccessLevel,NEW.Permission,NEW.InformationSystemId);
END$$

SHOW WARNINGS$$
SHOW WARNINGS$$
CREATE TRIGGER `InformationSystem_AFTER_INSERT` AFTER INSERT ON `InformationSystem`
 FOR EACH ROW BEGIN
	INSERT INTO `IntelliTrackSolutionsDB`.`AccessLevel` (`Permission`, `InformationSystemId`) VALUES(10, NEW.idInformationSystem);
END$$

SHOW WARNINGS$$

DELIMITER ;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;

INSERT INTO `InformationSystem` (`idInformationSystem`, `Title`, `Description`, `ApiKey`) VALUES
(1, 'Административная информационная система', 'Административная информационная система - контролирует остальные информационные системы.', '0');

INSERT INTO `User` (`idUser`, `Login`, `Password`, `AccessLevelId`) VALUES
(1, 'Admin', '$2a$11$BMTDglrdW89TLPdEuj3RGOQLZYPJTOFfQY/EVddAmHuTsapQ8oWSK', 1);

INSERT INTO `InformationUser` (`idInformationUser`, `UserId`, `FirstName`, `LastName`, `MiddleName`, `Avatar`) VALUES
(1, 1, 'Дмитрий', 'Колыхалов', 'Антонович', 'https://img-fotki.yandex.ru/get/6201/64843573.b6/0_7d8a2_5f5638ed_orig.jpg');