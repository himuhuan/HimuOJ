import { PointRunResult, CompilerInfo, ExecutionStatus } from "./ResultModels";

export interface CommitDetail {
	id: number;
	sourceUri: string;
	commitDate: Date;
	status: ExecutionStatus;
	messageFromCompiler: string;
	compilerPreset: {
		language: string;
		command: string;
		name: string;
	}
	userId: number;
	problemId: number;
	testPointResults: PointRunResult[];
}
