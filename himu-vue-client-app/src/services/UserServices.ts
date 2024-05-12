import {UserBriefInfo, UserDetailInfo} from "@/models/User";
import {AxiosDefaultBaseUrl, AxiosDefaultInstance} from "./AxiosInstance";
import {HimuApiResult, HimuApiResultWithData} from "@/models/HimuApiResult";
import router from "@/routers";
import {UserRole} from "@/services/AuthorizationServices.ts";
import {AuthorizedContestsList} from "@/models/AuthorizedContestsList.ts";

export class UserServicesImpl {
    async getUserBriefInfo(userId: string): Promise<UserBriefInfo> {
        const result = await AxiosDefaultInstance.get<HimuApiResultWithData>(
            `/users/${userId}/brief`
        );
        result.data.value.avatarUri =
            AxiosDefaultBaseUrl + result.data.value.avatarUri;
        return result.data.value as UserBriefInfo;
    }

    async getUserDetailInfo(userId: string): Promise<UserDetailInfo> {
        const result = await AxiosDefaultInstance.get<HimuApiResultWithData>(
            `/users/${userId}/detail`
        );
        if (result.status !== 200) {
            console.error("cannot get user detail info: " + result.data.message);
            router.push({name: "not-found"});
        }
        result.data.value.avatarUri =
            AxiosDefaultBaseUrl + result.data.value.avatarUri;
        result.data.value.backgroundUri =
            AxiosDefaultBaseUrl + result.data.value.backgroundUri;
        return result.data.value as UserDetailInfo;
    }

    async uploadAvatar(avatar: File): Promise<HimuApiResult> {
        const formData = new FormData();
        formData.append("avatar", avatar);
        const result = await AxiosDefaultInstance.post<HimuApiResult>(
            "/users/avatar",
            formData
        );
        return result.data;
    }

    async uploadBackground(background: File): Promise<HimuApiResult> {
        const formData = new FormData();
        formData.append("background", background);
        const result = await AxiosDefaultInstance.post<HimuApiResult>(
            "/users/background",
            formData
        );
        return result.data;
    }

    async getUserAvatar(userId: string): Promise<string> {
        const result = await AxiosDefaultInstance.get<HimuApiResultWithData>(
            `/users/${userId}/avatar`
        );
        return AxiosDefaultBaseUrl + result.data.value;
    }

    async getUserBackground(userId: string): Promise<string> {
        const result = await AxiosDefaultInstance.get<HimuApiResultWithData>(
            `/users/${userId}/background`
        );
        return AxiosDefaultBaseUrl + result.data.value;
    }

    async resetAvatar(): Promise<HimuApiResult> {
        const result = await AxiosDefaultInstance.delete<HimuApiResult>(
            "/users/avatar"
        );
        return result.data;
    }

    async resetBackground(): Promise<HimuApiResult> {
        const result = await AxiosDefaultInstance.delete<HimuApiResult>(
            "/users/background"
        );
        return result.data;
    }

    async getUserRole(userId: string): Promise<UserRole> {
        const resp = await AxiosDefaultInstance.get<HimuApiResultWithData>(
            `users/${userId}/authorization`
        );
        if (resp.status === 200) {
            return resp.data.value as UserRole;
        } else {
            throw new Error("Failed to get user permission");
        }
    }

	async hasContestDistributorPermissionById(userId: string): Promise<boolean> {
		const permission = await this.getUserRole(userId);
		return (
			permission === UserRole.Administrator ||
			permission === UserRole.ContestDistributor
		);
	}

    hasContestDistributorPermission(permission: string) : boolean {
        return (
            permission === UserRole.Administrator ||
            permission === UserRole.ContestDistributor
        );
    }

    async getAuthorizedContests(userId: string): Promise<AuthorizedContestsList> {
        const resp = await AxiosDefaultInstance.get<HimuApiResultWithData>(
            `users/${userId}/authorized_contests`
        );
        if (resp.status === 200) {
            return resp.data.value as AuthorizedContestsList;
        } else {
            throw new Error("Failed to get user permission");
        }
    }
}

export const UserServices = new UserServicesImpl();
