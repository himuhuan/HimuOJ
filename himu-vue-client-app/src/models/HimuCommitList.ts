import CommitListInfo from "./CommitListInfo";

/** 
 * dto: Himu.HttpApi.Utility.Response.HimuCommitListResponseValue
*/
export interface HimuCommitList {
   data: CommitListInfo[];
   totalCount: number;
   pageCount: number; 
}