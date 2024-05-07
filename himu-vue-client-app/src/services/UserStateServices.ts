// UserStateServices.ts --- User Services

import { User } from "@/models/User";
import { defineStore } from "pinia";
import { AxiosDefaultBaseUrl, AxiosDefaultInstance } from "./AxiosInstance";
import { HimuApiResult } from "@/models/HimuApiResult";
import { AxiosResponse } from "axios";
import Cookie from "js-cookie";
import { AuthServices } from "./AuthServices";

/**
 * User state only store the current user's 
 * login information and provide some basic operations.
 */
export const useUserState = defineStore("user", {
	persist: true,
	state: () => {
		return {
			user: null as User | null,
			hasLogin: false,
		};
	},
	getters: {
		name(): string {
			return this.user?.name ?? "";
		},
		id(): string {
			return this.user?.id ?? "";
		},
		accessToken(): string {
			return this.user?.accessToken ?? "";
		},
		phone(): string {
			return this.user?.phone ?? "";
		},
		avatarUri(): string {
			return this.user?.avatarUri!;
		},
		backgroundUri(): string {
			return this.user?.backgroundUri!;
		},
		perferTheme(): string {
			return this.user?.perferTheme ?? "light";
		},
		isLogin(): boolean {
			return this.hasLogin;
		},
	},
	actions: {
		async login(userName: string, password: string): Promise<boolean> {
			const result = await AuthServices.login(userName, password);
			if (result == undefined) return false;
			Cookie.set("accessToken", result.accessToken, { expires: 7 });
			this.hasLogin = true;
			this.user = new User(result);
			return true;
		},

		async vaildateRegisterCode(
			userName: string,
			code: string
		): Promise<boolean> {
			const success = await AuthServices.vaildateRegisterCode(userName, code);
			return success;
		},

		async resentRegisterCode(
			userName: string,
			userMail: string
		): Promise<boolean> {
			return AuthServices.resentRegisterCode(userName, userMail);
		},

		async uploadAvatar(avatar: File): Promise<AxiosResponse<HimuApiResult>> {
			const formData = new FormData();
			formData.append("avatar", avatar);
			const response = await AxiosDefaultInstance.instance
				.post<HimuApiResult>("users/avatar", formData, {
					headers: {
						"Content-Type": "multipart/form-data",
					},
				})
				.catch((error) => {
					if (error && error.response) {
						return error.response;
					} else {
						console.error(
							"Web server is not responding. Please try again later."
						);
						window.$message.error(
							"您的网络连接存在问题或服务器内部发生错误，请稍后再试。"
						);
						throw error;
					}
				});
			if (response.status == 200) {
				this.user!.avatarUri = AxiosDefaultBaseUrl + response.data.value;
			}
			return response;
		},

		async uploadBackground(
			background: File
		): Promise<AxiosResponse<HimuApiResult>> {
			const formData = new FormData();
			formData.append("background", background);
			const response = await AxiosDefaultInstance.instance
				.post<HimuApiResult>("users/background", formData, {
					headers: {
						"Content-Type": "multipart/form-data",
					},
				})
				.catch((error) => {
					if (error && error.response) {
						return error.response;
					} else {
						console.error(
							"Web server is not responding. Please try again later."
						);
						window.$message.error(
							"您的网络连接存在问题或服务器内部发生错误，请稍后再试。"
						);
						throw error;
					}
				});
			if (response.status == 200) {
				this.user!.backgroundUri = AxiosDefaultBaseUrl + response.data.value;
			}
			return response;
		},

		async resetBackground(): Promise<AxiosResponse<HimuApiResult>> {
			const response = await AxiosDefaultInstance.instance
				.delete<HimuApiResult>("users/background")
				.catch((error) => {
					if (error && error.response) {
						return error.response;
					} else {
						console.error(
							"Web server is not responding. Please try again later."
						);
						window.$message.error(
							"您的网络连接存在问题或服务器内部发生错误，请稍后再试。"
						);
						throw error;
					}
				});
			if (response.status == 200) {
				this.user!.backgroundUri = "";
			}
			return response;
		},

		async resetAvatar(): Promise<AxiosResponse<HimuApiResult>> {
			const response = await AxiosDefaultInstance.instance
				.delete<HimuApiResult>("users/avatar")
				.catch((error) => {
					if (error && error.response) {
						return error.response;
					} else {
						console.error(
							"Web server is not responding. Please try again later."
						);
						window.$message.error(
							"您的网络连接存在问题或服务器内部发生错误，请稍后再试。"
						);
						throw error;
					}
				});
			if (response.status == 200) {
				this.user!.avatarUri = "";
			}
			return response;
		},

		async logout(): Promise<AxiosResponse<HimuApiResult>> {
			const response = await AxiosDefaultInstance.instance
				.delete<HimuApiResult>("users/authentication")
				.catch((error) => {
					if (error && error.response) {
						return error.response;
					} else {
						console.error(
							"Web server is not responding. Please try again later."
						);
						throw error;
					}
				});
			if (response.status == 200) {
				this.user = null;
				Cookie.remove("accessToken");
				this.hasLogin = false;
			}
			return response;
		},

		async sentResetPasswordCode(
			email: string
		): Promise<AxiosResponse<HimuApiResult>> {
			const response = await AxiosDefaultInstance.instance
				.post<HimuApiResult>(
					`users/authentication/request_reset?email=${email}`
				)
				.catch((error) => {
					if (error && error.response) {
						return error.response;
					} else {
						throw error;
					}
				});
			return response;
		},

		async resetPassword(
			email: string,
			code: string,
			password: string
		): Promise<AxiosResponse<HimuApiResult>> {
			const response = await AxiosDefaultInstance.instance
				.put<HimuApiResult>(`users/authentication/reset`, {
					mail: email,
					token: code,
					newPassword: password,
				})
				.catch((error) => {
					if (error && error.response) {
						return error.response;
					} else {
						throw error;
					}
				});
			if (response.status == 200) {
				this.user = null;
				Cookie.remove("accessToken");
			}
			return response;
		},

		triggerThemeChange() {
			if (this.user == null) return;
			this.user.perferTheme =
				this.user.perferTheme == "light" ? "dark" : "light";
		},
	},
});
