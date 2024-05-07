CREATE TABLE `testpoints` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Input` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Expected` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProblemId` bigint NOT NULL,
  `CaseName` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`Id`),
  KEY `IX_TestPoints_ProblemId` (`ProblemId`),
  CONSTRAINT `FK_TestPoints_ProblemSet_ProblemId` FOREIGN KEY (`ProblemId`) REFERENCES `problemset` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=491894687932486 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;