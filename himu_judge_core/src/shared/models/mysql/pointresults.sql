CREATE TABLE `pointresults` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `TestStatus` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Usage_MemoryByteUsed` bigint DEFAULT NULL,
  `Usage_TimeUsed` bigint DEFAULT NULL,
  `RunResult_ExitCode` int DEFAULT NULL,
  `RunResult_Message` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `TestPointId` bigint NOT NULL,
  `CommitId` bigint NOT NULL,
  `Difference_Actual` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Difference_Expected` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Difference_Position` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PointResults_CommitId` (`CommitId`),
  KEY `IX_PointResults_TestPointId` (`TestPointId`),
  CONSTRAINT `FK_PointResults_TestPoints_TestPointId` FOREIGN KEY (`TestPointId`) REFERENCES `testpoints` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PointResults_UserCommits_CommitId` FOREIGN KEY (`CommitId`) REFERENCES `usercommits` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=498847643889734 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;