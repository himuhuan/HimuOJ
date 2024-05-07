export default class CommitListInfo {
	commitId: number;
	userId: number;
	problemId: number;
	problemName: string;
	sourceUri: string;
	commitStatus: string;
	compilerName: string;

	constructor(
		commitId: number,
		userId: number,
		problemId: number,
		problemName: string,
		sourceUri: string,
		commitStatus: string,
		compilerName: string
	) {
		this.commitId = commitId;
		this.userId = userId;
		this.problemId = problemId;
		this.problemName = problemName;
		this.sourceUri = sourceUri;
		this.commitStatus = commitStatus;
		this.compilerName = compilerName;
	}

    static fromJson(json: any): CommitListInfo {
        return new CommitListInfo(
            json.commitId,
            json.userId,
            json.problemId,
			json.problemName,
            json.sourceUri,
            json.commitStatus,
            json.compilerName
        );
    }
}
