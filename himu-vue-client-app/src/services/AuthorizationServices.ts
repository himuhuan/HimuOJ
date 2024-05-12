import { AxiosDefaultInstance } from "./AxiosInstance";
import { HimuApiResultWithData } from "@/models/HimuApiResult";

export enum UserRole {
	Administrator = "Administrator",
	StandardUser = "StandardUser",
	ContestDistributor = "ContestDistributor",
}

export class AuthorizationServices {
	static async applyPermission(
		userId: string,
		permission: UserRole
	): Promise<boolean> {
		const resp = await AxiosDefaultInstance.post<HimuApiResultWithData>(
			`user/authorization`,
			{
                userId: userId,
				role: permission,
			}
		);
		return resp.status === 200;
	}
}
