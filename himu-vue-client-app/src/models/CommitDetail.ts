import { PointRunResult, CompilerInfo } from "./ResultModels";


export interface CommitDetail {
	id: number;
	sourceUri: string;
	status: string;
	userId: number;
	problemId: number;
    compilerInformation: CompilerInfo;
	testPointResults: PointRunResult[];
}
