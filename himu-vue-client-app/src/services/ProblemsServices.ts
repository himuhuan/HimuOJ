// ProblemsServices.ts --- Problems Services

import {AxiosDefaultInstance} from "./AxiosInstance";
import {HimuApiResultWithData} from "@/models/HimuApiResult";
import ProblemDetail from "@/models/ProblemDetail.ts";
import {checkResponse} from "@/utils/HimuTools.ts";
import ProblemListFilter from "@/models/ProblemListFilter.ts";
import ProblemList from "@/models/ProblemList.ts";

export class ProblemsServicesImpl {

    async getProblemsCount() {
        return await AxiosDefaultInstance.get<HimuApiResultWithData>(
            `problems/count`
        );
    }

    async getProblemId(contestCode: string, problemCode: string) {
        const response = await AxiosDefaultInstance.get<HimuApiResultWithData>(
            `contests/${contestCode}/problems/${problemCode}/id`
        );
        if (checkResponse(response)) {
            return response.data.value as string;
        } else {
            return undefined;
        }
    }

    async getProblemDetailById(problemId: string): Promise<ProblemDetail | null> {
        const resp = await AxiosDefaultInstance.get<HimuApiResultWithData>(
            `problems/${problemId}/detail`
        );
        if (resp.status === 200) {
            return new ProblemDetail(
                resp.data.value.code,
                resp.data.value.title,
                resp.data.value.content,
                resp.data.value.maxMemoryLimitByte,
                resp.data.value.maxExecuteTimeLimit,
                resp.data.value.allowDownloadInput,
                resp.data.value.allowDownloadAnswer
            );
        } else {
            return null;
        }
    }

    async getProblemList(page: number, size: number, filter?: ProblemListFilter): Promise<ProblemList | null> {
        let query = `problems?page=${page}&size=${size}`;
        if (filter) {
            if (filter.contestCode) {
                query += `&contestCode=${filter.contestCode}`;
            }
            if (filter.contestId) {
                query += `&problemCode=${filter.contestId}`;
            }
            if (filter.administratorId) {
                query += `&problemName=${filter.administratorId}`;
            }
            if (filter.authenticatedUserId) {
                query += `&problemStatus=${filter.authenticatedUserId}`;
            }
            if (filter.creatorId) {
                query += `&problemDateEnd=${filter.creatorId}`;
            }
        }
        const resp = await AxiosDefaultInstance.get<HimuApiResultWithData>(query);
        if (resp.status === 200) {
            return resp.data.value as ProblemList;
        } else {
            return null;
        }
    }

    async getDefaultProblemList(page: number, size: number): Promise<ProblemList | null> {
        return await this.getProblemList(page, size,  {
            contestCode: 'himu-contest'
        });
    }
}

export const ProblemsServices = new ProblemsServicesImpl();
