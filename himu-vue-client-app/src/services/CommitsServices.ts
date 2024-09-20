import { AxiosDefaultInstance } from "./AxiosInstance";
import { HimuApiResultWithData } from "@/models/HimuApiResult";
import { CommitDetail } from "@/models/CommitDetail";
import { HimuCommitList } from "@/models/HimuCommitList.ts";
import CommitListFilter from "@/models/CommitListFilter";

export class CommitsServicesImpl {
	/**
	 * api: user/{userId}/commits?page={page}&pageSize={pageSize} (filter)
	 */
	async filterCommitList(
		filter: CommitListFilter
	): Promise<HimuCommitList> {
		let query = `commits?page=${filter.page}&size=${filter.pageSize}`;
		if (filter) {
			if (filter.problemName)
				query += `&problemName=${filter.problemName}`;
			if (filter.language)
				query += `&language=${encodeURIComponent(filter.language as string)}`;
			if (filter.commitStatus)
				query += `&commitStatus=${filter.commitStatus}`;
			if (filter.commitDateEnd)
				query += `&commitDateEnd=${filter.commitDateEnd}`;
			if (filter.userId)
				query += `&userId=${filter.userId}`;
			if (filter.problemId)
				query += `&problemId=${filter.problemId}`;
		}
		const resp = await AxiosDefaultInstance.get<HimuApiResultWithData>(query);
		if (resp.status === 200) return resp.data.value as HimuCommitList;
		else throw new Error("Failed to get commit list");
	}

	async getUserCommitListCount(userId: string): Promise<number> {
		const resp = await AxiosDefaultInstance.get<HimuApiResultWithData>(
			`user/${userId}/commits/count`
		);
		if (resp.status === 200) return resp.data.value as number;
		else throw new Error("Failed to get commit list count");
	}

	/**
	 * Get commit detail
	 * @param commitId Commit Id
	 * @returns Commit detail
	 * @throws Error if failed to get commit detail
	 */
	async getCommitDetail(commitId: string): Promise<CommitDetail> {
		const resp = await AxiosDefaultInstance.get<HimuApiResultWithData>(
			`commits/${commitId}/detail`
		);
		if (resp.status === 200) {
			return resp.data.value as CommitDetail;
		} else {
			throw new Error("Failed to get commit detail");
		}
	}

	/**
	 * Submit source file to server, but not judge it
	 * @param problemId problem ID
	 * @param sourceFile Source file to submit
	 * @param language language of commit
	 * @returns commit id
	 * @throws Error if failed to submit source file
	 */
	async postSourceFile(problemId: string, sourceFile: File, language: string) {
		const formData = new FormData();
		formData.append("sourceFile", sourceFile);
		const resp = await AxiosDefaultInstance.post<HimuApiResultWithData>(
			`commits/submit_file?problemId=${problemId}&language=${language}`,
			formData
		);
		if (resp.status === 200) return resp.data.value as string;
		else throw new Error("Failed to submit source file");
	}

	async postSourceCode(
		problemId: string,
		sourceCode: string,
		language: string
	) {
		const resp = await AxiosDefaultInstance.post<HimuApiResultWithData>(
			`commits/submit_code`,
			{
				ProblemId: problemId,
				SourceCode: sourceCode,
				CompilerPresetName: language,
			}
		);
		if (resp.status === 200) return resp.data.value as string;
		else throw new Error("Failed to submit source code");
	}
}

export const CommitsServices = new CommitsServicesImpl();
