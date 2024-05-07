import { AxiosDefaultBaseUrl } from "./AxiosInstance";

export class TestPointServices {
	getTestPointInputUrl(testpointId: string): string {
        return AxiosDefaultBaseUrl + `api/testpoints/${testpointId}/input`;
    }
    getTestPointAnswerUrl(testpointId: string): string {
        return AxiosDefaultBaseUrl + `api/testpoints/${testpointId}/answer`;
    }
}

export const TestPointServicesInstance = new TestPointServices();