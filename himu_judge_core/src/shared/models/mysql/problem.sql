-- himuoj.problemset definition

CREATE TABLE `problemset` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Detail_Code` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Detail_Title` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Detail_Content` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Detail_MaxMemoryLimitByte` bigint NOT NULL,
  `Detail_MaxExecuteTimeLimit` bigint NOT NULL,
  `ContestId` bigint NOT NULL,
  `Detail_AllowDownloadAnswer` tinyint(1) NOT NULL DEFAULT '0',
  `Detail_AllowDownloadInput` tinyint(1) NOT NULL DEFAULT '1',
  `DistributorId` bigint NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `IX_ProblemSet_ContestId` (`ContestId`),
  KEY `IX_ProblemSet_Detail_Code` (`Detail_Code`),
  KEY `IX_ProblemSet_DistributorId` (`DistributorId`),
  KEY `IX_ProblemSet_Detail_Title` (`Detail_Title`(10)),
  CONSTRAINT `FK_ProblemSet_AspNetUsers_DistributorId` FOREIGN KEY (`DistributorId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ProblemSet_Contests_ContestId` FOREIGN KEY (`ContestId`) REFERENCES `contests` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=490642060533830 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;