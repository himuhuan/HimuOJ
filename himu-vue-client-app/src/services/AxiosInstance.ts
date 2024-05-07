// AxioInstance.ts -- Axios Instance

import axios from "axios";
import Cookies from "js-cookie";
import type { AxiosInstance, AxiosRequestConfig, AxiosResponse } from "axios";
import router from "@/routers";

export class AxiosInstanceService {
	instance: AxiosInstance;

	constructor(config: AxiosRequestConfig) {
		this.instance = axios.create(config);

		this.instance.interceptors.request.use(
			(config) => {
				const token = Cookies.get("accessToken");
				if (token) {
					config.headers!.Authorization = `Bearer ${token}`;
				}
				return config;
			},
			(error) => {
				if (window.$message) {
					window.$message.error("请求错误: " + error);
				}
				return Promise.reject(error);
			}
		);

		this.instance.interceptors.response.use(
			(error) => {
				return error;
			},
			(error) => {
				if (error.response.status === 401) {
					Cookies.remove("accessToken");
					router.push("/authentication");
					if (window.$message) {
						window.$message.error("您的登录已过期，请重新登录。");
					}
				}
				return Promise.reject(error);
			}
		);
	}

	public async get<T>(url: string, data?: any): Promise<AxiosResponse<T>> {
		return AxiosDefaultInstance.instance.get<T>(url, data).catch((res) => {
			if (res && res.response) {
				return res.response;
			} else {
				console.error("Web server is not responding. Please try again later.");
				window.$message.error("您的网络连接存在问题或服务器内部发生错误，请稍后再试。");
				throw res;
			}
		});
	}

	public async post<T>(url: string, data?: any): Promise<AxiosResponse<T>> {
		return await AxiosDefaultInstance.instance.post<T>(url, data).catch((res) => {
			if (res && res.response) {
				return res.response;
			} else {
				console.error("Web server is not responding. Please try again later.");
				window.$message.error("您的网络连接存在问题或服务器内部发生错误，请稍后再试。");
				throw res;
			}
		});
	}

	public async delete<T>(url: string, data?: any): Promise<AxiosResponse<T>> {
		return await AxiosDefaultInstance.instance.delete<T>(url, data).catch((res) => {
			if (res && res.response) {
				return res.response;
			} else {
				console.error("Web server is not responding. Please try again later.");
				window.$message.error("您的网络连接存在问题或服务器内部发生错误，请稍后再试。");
				throw res;
			}
		});
	}

	public async put<T>(url: string, data?: any): Promise<AxiosResponse<T>> {
		return await AxiosDefaultInstance.instance.put<T>(url, data).catch((res) => {
			if (res && res.response) {
				return res.response;
			} else {
				console.error("Web server is not responding. Please try again later.");
				window.$message.error("您的网络连接存在问题或服务器内部发生错误，请稍后再试。");
				throw res;
			}
		});
	}
}

const AxiosDefaultConfig: AxiosRequestConfig = {
	baseURL: "https://localhost:7297/api",
	timeout: 10000,
	headers: {
		Accept: "application/json",
	},
};

export const AxiosDefaultInstance = new AxiosInstanceService(AxiosDefaultConfig);
export const AxiosDefaultBaseUrl = "https://localhost:7297/";
