// Used to store user information after login
import { AxiosDefaultBaseUrl } from "@/services/AxiosInstance";
import { UserRole } from "@/services/AuthorizationServices";

export interface UserBriefInfo {
	userName: string;
	avatarUri: string;
}

export interface UserDetailInfo {
	id: string;
	userName: string;
	avatarUri: string;
	backgroundUri: string;
	email: string;
	phoneNumber: string;
	lastLoginDate: string;
	registerDate: string;
	totalCommits: number;
	problemSolved: number;
	commitAccepted: number;
	permission: UserRole;
}

export interface UserLoginInfo {
	userId: string;
	accessToken: string;
	userName: string;
	email: string;
	phoneNumber: string;
	avatarUri: string;
	backgroundUri: string;
}

export class User {
	public name: string;
	public id: string;
	public accessToken: string;
	public email: string;
	public phone: string;
	// required reload for relative path
	public avatarUri: string;
	public backgroundUri: string;
	// not saved in database
	public perferTheme: string = "light";

	constructor(loginInfo: UserLoginInfo) {
		this.name = loginInfo.userName;
		this.id = loginInfo.userId;
		this.accessToken = loginInfo.accessToken;
		this.email = loginInfo.email;
		this.phone = loginInfo.phoneNumber;
		this.avatarUri = AxiosDefaultBaseUrl + loginInfo.avatarUri;
		if (loginInfo.backgroundUri && loginInfo.backgroundUri.trim() !== "")
			this.backgroundUri = AxiosDefaultBaseUrl + loginInfo.backgroundUri;
		else this.backgroundUri = "";
	}
}
