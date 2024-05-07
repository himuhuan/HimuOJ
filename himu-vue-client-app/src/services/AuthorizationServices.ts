import { AxiosDefaultInstance } from "./AxiosInstance";
import { HimuApiResultWithData } from "@/models/HimuApiResult";

export enum UserPermission {
	Administrator = "Administrator",
	StandardUser = "StandardUser",
	ProblemPublisher = "ProblemPublisher",
}

export class AuthorizationServices {
	static async getUserPermission(userId: string): Promise<UserPermission> {
		const resp = await AxiosDefaultInstance.get<HimuApiResultWithData>(
			`user/${userId}/authorization`
		);
		if (resp.status === 200) {
			return resp.data.value as UserPermission;
		} else {
			throw new Error("Failed to get user permission");
		}
	}

	/**
	 * A user who has 'ProblemPublisher' or upper permission can publish problems
	 */
	static async hasProblemPublishPermissionById(userId: string): Promise<boolean> {
		const permission = await this.getUserPermission(userId);
		return (
			permission === UserPermission.Administrator ||
			permission === UserPermission.ProblemPublisher
		);
	}

	/**
	 * A user who has 'ProblemPublisher' or upper permission can publish problems
	 */
	static hasProblemPublishPermission(permission: UserPermission): boolean {
		return (
			permission === UserPermission.Administrator ||
			permission === UserPermission.ProblemPublisher
		);
	}

	static async applyPermission(
		userId: string,
		permission: UserPermission
	): Promise<boolean> {
		const resp = await AxiosDefaultInstance.post<HimuApiResultWithData>(
			`user/authorization`,
			{
                userId: userId,
				role: permission,
			}
		);
		if (resp.status === 200) {
			return true;
		} else {
			return false;
		}
	}
}
