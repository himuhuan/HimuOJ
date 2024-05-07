import {HimuApiResult} from "@/models/HimuApiResult.ts";

/**
 * To avoid annoying warnings for some ides
 */
export function usedVariables(v: any) {
    return v;
}

/**
 * Check if the response is valid
 * @param response the response from the server
 * @param showMessage if true, show a message in case of error
 */
export function checkResponse(response: any, showMessage: boolean = true): boolean {
    if (response.status >= 400) {
        const apiResult = response.data as HimuApiResult;
        if (!showMessage) return false;
        if (apiResult) {
            window.$message.error(`Error from server: (${apiResult.code}) ${apiResult.message}`);
        } else {
            window.$message.error(`Unknown error from server with status code ${response.status}`);
        }
        return false;
    }
    return true;
}