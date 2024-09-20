export default interface CommitListFilter {
	page: number;
	pageSize: number;
	commitDateEnd?: number;
	commitStatus?: string;
	language?: string;
	problemId?: string;
	problemName?: string;
	userId?: string;
}