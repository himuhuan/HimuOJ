import CommitListInfo from "./CommitListInfo";

/** 
 * dto: Himu.HttpApi.Utility.Response.HimuCommitListResponseValue
*/
export interface UserCommitList {
   data: CommitListInfo[];
   totalCount: number;
   pageCount: number; 
}