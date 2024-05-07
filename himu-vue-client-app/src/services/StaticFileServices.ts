import { AxiosDefaultBaseUrl } from "./AxiosInstance";
import axios from "axios";

export class StaticFileServices {
	async getRawTextContent(url: string): Promise<string> {
		const resp = await axios.get(url, { baseURL: AxiosDefaultBaseUrl, responseType: "text" });
		if (resp.status === 200) return resp.data as string;
		else throw new Error("Failed to get raw text content from url: " + url);
	}
}

export const StaticFileServicesInstance = new StaticFileServices();
