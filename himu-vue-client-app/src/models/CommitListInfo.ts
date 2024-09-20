export default class CommitListInfo {
	commitId: number;
	userId: number;
	problemId: number;
	problemName: string;
	sourceUri: string;
	commitStatus: string;
	compilerName: string;
	commitDate: Date;

	constructor(
		commitId: number,
		userId: number,
		problemId: number,
		problemName: string,
		sourceUri: string,
		commitStatus: string,
		compilerName: string,
		commitDate: string,
	) {
		this.commitId = commitId;
		this.userId = userId;
		this.problemId = problemId;
		this.problemName = problemName;
		this.sourceUri = sourceUri;
		this.commitStatus = commitStatus;
		this.compilerName = compilerName;
		this.commitDate = new Date(commitDate);
	}

    static fromJson(json: any): CommitListInfo {
        return new CommitListInfo(
            json.commitId,
            json.userId,
            json.problemId,
			json.problemName,
            json.sourceUri,
            json.commitStatus,
            json.compilerName,
			json.commitDate,
        );
    }
}
