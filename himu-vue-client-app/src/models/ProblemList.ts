import ProblemListItem from "./ProblemListItem";

/**
 * Result of a problem list query.
 */
export default interface ProblemList
{
    problems: ProblemListItem[];

    totalCount: number;

    pageCount: number;
}