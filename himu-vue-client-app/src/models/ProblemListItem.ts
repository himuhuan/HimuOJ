export default interface ProblemListItem
{
    problemId: string;

    problemCode: string;

    problemTitle: string;

    contestCode: string;

    contestTitle: string;

    acceptedCount: number;

    totalCommitCount: number;

    accuracyRate: number
}