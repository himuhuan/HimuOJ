import { AxiosDefaultInstance } from "./AxiosInstance";
import { HimuApiResult, HimuApiResultWithData } from "@/models/HimuApiResult";
import { UserLoginInfo } from "@/models/User";
import Cookie from "js-cookie";

export class AuthServicesImpl {
	async login(
		username: string,
		password: string
	): Promise<UserLoginInfo | undefined> {
		const result = await AxiosDefaultInstance.post<HimuApiResultWithData>(
			"users/authentication",
			{
				input: username,
				// TODO: Support other login methods
				method: "user",
				password: password,
			}
		);

		if (result.status != 200) {
			window.$message.error(`登录失败：${result.data.message}`);
			return undefined;
		}

		const user = result.data.value as UserLoginInfo;
		return user;
	}

	async register(
		userName: string,
		password: string,
		passwordConfirmation: string,
		mail: string,
		phone: string | null = null
	): Promise<HimuApiResult> {
		const result = await AxiosDefaultInstance.post<HimuApiResult>("/users", {
			userName: userName,
			password: password,
			repeatedPassword: passwordConfirmation,
			mail: mail,
			phoneNumber: phone,
		});

		if (result.status != 200) {
			window.$message.error(`注册失败：${result.data.message}`);
			throw new Error(result.data.message);
		}

		return result.data;
	}

	async vaildateRegisterCode(userName: string, code: string): Promise<boolean> {
		const result = await AxiosDefaultInstance.post<HimuApiResult>(
			"/users/confirmation",
			{
				userName: userName,
				confirmationToken: code,
			}
		);
		if (result.status != 200) {
			window.$message.error(`验证失败：${result.data.message}`);
			return false;
		}
		return true;
	}

	async resentRegisterCode(
		userName: string,
		userMail: string
	): Promise<boolean> {
		const result = await AxiosDefaultInstance.post<HimuApiResult>(
			"/users/confirmation/retry",
			{
				userName: userName,
				mail: userMail,
			}
		);
		if (result.status != 200) {
			window.$message.error(`在试图重新发送邮件时失败：${result.data.message}`);
			return false;
		}
		return true;
	}

	async logout(): Promise<void> {
		const result = await AxiosDefaultInstance.delete<HimuApiResult>(
			"users/authentication"
		);
		if (result.status === 200) {
			Cookie.remove("accessToken");
		}
	}

	async sentResetPasswordCode(email: string): Promise<HimuApiResult> {
		const result = await AxiosDefaultInstance.post<HimuApiResult>(
			`users/authentication/request_reset?email=${email}`
		);
		return result.data;
	}

	async resetPassword(
		email: string,
		code: string,
		password: string,
		repeatedPassword: string
	): Promise<HimuApiResult> {
		const result = await AxiosDefaultInstance.put<HimuApiResult>(
			`users/authentication/reset`,
			{
				mail: email,
				token: code,
				newPassword: password,
			}
		);
		if (result.status == 200) {
			Cookie.remove("accessToken");
		}
		return result.data;
	}
}

export const AuthServices = new AuthServicesImpl();
