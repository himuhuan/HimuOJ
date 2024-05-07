<script setup lang="ts">
import { PropType } from "vue";
import { UserDetailInfo } from "@/models/User";

// naive-ui
import {
	NAvatar,
	NThing,
	NCard,
	NSpace,
	NTag,
	NIcon,
	NButton,
	NText,
	NDescriptions,
	NDescriptionsItem,
	NList,
} from "naive-ui";
// icons
import { UserEdit as UserEditIcon } from "@vicons/fa";
import {
	LockResetRound as EmailEditIcon,
	LogOutRound as LogOutIcon,
} from "@vicons/material";
// services
import { useUserState } from "@/services/UserStateServices";
import router from "@/routers";
import { useDialog } from "naive-ui";

// components
import ListGotoItem from "../shared/ListGotoItem.vue";

const props = defineProps({
	userInfo: {
		type: Object as PropType<UserDetailInfo>,
		required: true,
	},
});

const userState = useUserState();
const dialog = useDialog();

/////////////////////////////// functions ///////////////////////////////
function uploadAvatar(e: Event) {
	const input = document.getElementById(
		"upload-avatar-input"
	) as HTMLInputElement;
	input.click();
}

async function doUploadAvatar(e: Event) {
	const input = e.target as HTMLInputElement;
	if (input.files && input.files.length > 0) {
		const file = input.files[0];
		const res = await userState.uploadAvatar(file).catch((err) => {
			console.error(err);
			return null;
		});
		if (!res) {
			console.error("上传头像失败: 发生未知错误");
		} else if (res.status != 200) {
			window.$message.error(`上传头像失败: ${res.data.message}`);
		} else {
			window.$message.success("上传头像成功");
		}
	}
}

function uploadBackground(e: Event) {
	const input = document.getElementById(
		"upload-background-input"
	) as HTMLInputElement;
	input.click();
}

async function doUploadBackground(e: Event) {
	const input = e.target as HTMLInputElement;
	if (input.files && input.files.length > 0) {
		const file = input.files[0];
		const res = await userState.uploadBackground(file).catch((err) => {
			console.error(err);
			return null;
		});
		if (!res) {
			console.error("上传背景失败: 发生未知错误");
		} else if (res.status != 200) {
			window.$message.error(`上传背景失败: ${res.data.message}`);
		} else {
			window.$message.success("上传背景成功");
		}
	}
}

function resetPasswordClick() {
	dialog.info({
		title: "重置密码",
		content: "确定要重置密码吗？点击确定之后请按重置密码向导的提示进行操作。",
		positiveText: "确定",
		negativeText: "算了",
		onPositiveClick: () => {
			router.push({ name: "reset-password" });
		},
		onNegativeClick: () => {},
	});
}

function userLogoutClick() {
	dialog.info({
		title: "登出",
		content: "确定要登出吗？",
		positiveText: "确定",
		negativeText: "算了",
		onPositiveClick: () => {
			userState.logout();
			router.push({ name: "authentication" });
		},
		onNegativeClick: () => {},
	});
}
</script>

<template>
	<div>
		<n-card class="info-card">
			<n-space vertical>
				<n-thing content-indented>
					<template #avatar>
						<n-avatar :size="48" :src="props.userInfo.avatarUri" />
					</template>
					<template #header>
						{{ props.userInfo.userName }}
					</template>
					<template #header-extra>
						<n-tag type="error"> {{ props.userInfo.permission }} </n-tag>
					</template>
					<template #description>
						<span> 他很懒，什么也没留下~ </span>

						<div style="margin-top: 10px">
							<p>
								<n-text
									>编辑您的头像与背景，个性化你的账户。您头像与背景将会显示在
									HimuOJ 网站上。</n-text
								>
							</p>
							<n-space>
								<n-button @click="uploadAvatar"> 添加头像 </n-button>
								<n-button @click="uploadBackground"> 添加背景 </n-button>
								<n-button @click.stop="() => userState.resetBackground()">
									重置背景
								</n-button>
								<input
									type="file"
									accept="image/*"
									style="display: none"
									id="upload-avatar-input"
									@change="doUploadAvatar"
								/>
								<input
									type="file"
									accept="image/*"
									style="display: none"
									id="upload-background-input"
									@change="doUploadBackground"
								/>
							</n-space>
						</div>
					</template>
				</n-thing>
			</n-space>
		</n-card>
		<n-card class="info-card" title="个人信息">
			<template #header-extra>
				<n-button type="primary" size="small" icon-placement="right">
					<template #icon>
						<n-icon> <user-edit-icon /> </n-icon>
					</template>
					编辑
				</n-button>
			</template>
			<n-descriptions label-placement="top" :columns="2">
				<n-descriptions-item>
					<template #label> 邮箱 </template>
					{{ props.userInfo.email }}
				</n-descriptions-item>
				<n-descriptions-item>
					<template #label> 电话 </template>
					{{
						props.userInfo.phoneNumber === ""
							? "未填写"
							: props.userInfo.phoneNumber
					}}
				</n-descriptions-item>
				<n-descriptions-item :span="1">
					<template #label> 注册于 </template>
					{{ props.userInfo.registerDate }}
				</n-descriptions-item>
				<n-descriptions-item :span="1">
					<template #label> 最后登录于 </template>
					{{ props.userInfo.lastLoginDate }}
				</n-descriptions-item>
				<n-descriptions-item :span="1">
					<template #label> 好友 </template>
					0
				</n-descriptions-item>
				<n-descriptions-item :span="1">
					<template #label> 关注 </template>
					0
				</n-descriptions-item>
			</n-descriptions>
		</n-card>
		<n-card class="info-card" title="账号安全">
			<n-list hoverable clickable :show-divider="false">
				<list-goto-item @click="resetPasswordClick">
					<template #icon>
						<n-icon> <user-edit-icon /> </n-icon>
					</template>
					修改密码
				</list-goto-item>
				<list-goto-item>
					<template #icon>
						<n-icon> <email-edit-icon /> </n-icon>
					</template>
					修改绑定邮箱
				</list-goto-item>
				<list-goto-item @click="userLogoutClick">
					<template #icon>
						<n-icon> <log-out-icon /> </n-icon>
					</template>
					退出登录
				</list-goto-item>
			</n-list>
		</n-card>
	</div>
</template>

<style scoped>
.info-card {
	margin: 5px;
	width: auto;
}

</style>
