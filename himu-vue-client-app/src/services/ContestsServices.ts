import {AxiosDefaultInstance} from "./AxiosInstance";
import {HimuApiResult, HimuApiResultWithData} from "@/models/HimuApiResult";
import ProblemDetail from "@/models/ProblemDetail.ts";
import {checkResponse} from "@/utils/HimuTools.ts";

export class ContestsServicesImpl {

    /**
     * Get contest ID by contest code
     * We agree that all apis use contest id instead of contest code
     * @return Contest ID if the contest exists, otherwise return undefined
     */
    async getContestId(contestCode: string): Promise<string | undefined> {
        const response = await AxiosDefaultInstance.get<HimuApiResultWithData>(
            `contests/${contestCode}/id`
        );
        if (!checkResponse(response)) {
            return undefined;
        }
        return response.data.value as string;
    }

    /**
     * Check if the user has permission to access the contest
     * @param contestId Contest ID
     * @param userId User ID
     * @returns True if the user has permission, otherwise false (Including contest not exist)
     */
    async checkUserPermission(contestId: string, userId: string): Promise<boolean> {
        const response = await AxiosDefaultInstance.get<HimuApiResultWithData>(
            `contests/${contestId}/check/permission?userId=${userId}`
        );
        return !(response.status !== 200 || !response.data.value);
    }

    async checkProblemCode(contestId: string, problemCode: string) {
        const response = await AxiosDefaultInstance.get<HimuApiResult>(
            `contests/${contestId}/check/problem_code/${problemCode}`
        );
        return checkResponse(response);
    }

    /**
     * Post Problem to contest
     * @return boolean True if the problem is posted successfully, otherwise false
     */
    async postProblem(contestId: string, data: ProblemDetail): Promise<boolean> {
        console.log(data);
        const response = await AxiosDefaultInstance.post<HimuApiResult>(
            `contests/${contestId}/problems`,
            data
        );
        return checkResponse(response);
    }

}

export const ContestsServices = new ContestsServicesImpl();