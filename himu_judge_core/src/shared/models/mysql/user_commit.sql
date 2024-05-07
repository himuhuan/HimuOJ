-- himuoj.usercommits definition

CREATE TABLE `usercommits` (
  `Id` bigint NOT NULL,
  `SourceUri` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UserId` bigint NOT NULL,
  `ProblemId` bigint NOT NULL,
  `CompilerInformation_CompilerName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CompilerInformation_MessageFromCompiler` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_UserCommits_ProblemId` (`ProblemId`),
  KEY `IX_UserCommits_UserId` (`UserId`),
  KEY `IX_UserCommits_CompilerInformation_CompilerName` (`CompilerInformation_CompilerName`),
  KEY `IX_UserCommits_Status` (`Status`),
  CONSTRAINT `FK_UserCommits_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_UserCommits_ProblemSet_ProblemId` FOREIGN KEY (`ProblemId`) REFERENCES `problemset` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;