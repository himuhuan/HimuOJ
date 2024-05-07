<template>
	<center-content-container top-offset="5vh">
		<n-card>
			<n-space justify="center" align="center">
				<img src="@/assets/images/himu-logo.svg" width="64" height="64" />
				<div style="font-size: 16px">重置您的密码</div>
			</n-space>
			<n-card>
				<n-spin :show="showSpin" size="small">
					<n-form :model="formState" :rules="rules">
						<n-space vertical>
							<n-form-item-row label="您账号对应的邮箱" path="info.email">
								<n-input placeholder="邮箱" v-model:value="formState.info.email" />
							</n-form-item-row>

							<n-button type="primary" block secondary strong @click="sendResetPasswordEmail"> 发送重置邮件 </n-button>

							<transition name="slide-fade">
								<n-card embedded v-if="showResetPasswordForm">
									<n-form-item-row label="重置令牌" path="info.token">
										<n-input placeholder="重置令牌" v-model:value="formState.info.token" />
									</n-form-item-row>
									<n-form-item-row label="新密码" path="info.newPassword">
										<n-input
											type="password"
											show-password-on="mousedown"
											placeholder="新密码"
											v-model:value="formState.info.newPassword"
										/>
									</n-form-item-row>
									<n-form-item-row>
										<n-button type="primary" block secondary strong @click="resetPassword"> 重置密码 </n-button>
									</n-form-item-row>
								</n-card>
							</transition>
						</n-space>
					</n-form>
				</n-spin>
			</n-card>
		</n-card>
	</center-content-container>
</template>

<script lang="ts" setup>
import CenterContentContainer from "@/components/CenterContentContainer.vue";
import { NCard, NForm, NFormItemRow, NInput, NButton, useMessage, NSpace, NSpin, FormRules } from "naive-ui";
import { ref } from "vue";
import router from "@/routers";
import { useUserState } from "@/services/UserStateServices";

const message = useMessage();
const showSpin = ref(false);
const showResetPasswordForm = ref(false);
const userState = useUserState();
const formState = ref({
	info: {
		email: (userState.user != null && userState.user.email) ? userState.user.email : "",
		token: "",
		newPassword: "",
	},
});

const rules: FormRules = {
	info: {
		email: [
			{
				required: true,
				message: "请输入您的邮箱",
				trigger: "blur",
			},
		],
		token: [
			{
				required: true,
				message: "请输入重置令牌",
				trigger: "blur",
			},
		],
		newPassword: [
			{
				required: true,
				message: "请输入新密码",
				trigger: "blur",
			},
		],
	},
};

const sendResetPasswordEmail = async () => {
	showSpin.value = true;
	const res = await userState.sentResetPasswordCode(formState.value.info.email).catch((err) => {
		console.error(err);
		return null;
	});
	showSpin.value = false;
	if (!res) {
		message.error("发送重置邮件失败: 发生未知错误");
	} else if (res.status !== 200) {
		message.error("发送重置邮件失败: " + res.data.message);
	} else {
		message.success("发送重置邮件成功");
		showResetPasswordForm.value = true;
	}
};

const resetPassword = async () => {
	showSpin.value = true;
	const res = await userState
		.resetPassword(formState.value.info.email, formState.value.info.token, formState.value.info.newPassword)
		.catch((err) => {
			console.error(err);
			return null;
		});
	showSpin.value = false;
	if (!res) {
		message.error("重置密码失败: 发生未知错误");
	} else if (res.status !== 200) {
		message.error("重置密码失败: " + res.data.message);
	} else {
		message.success("重置密码成功");
		router.push({ name: "authentication" });
	}
};
</script>

<style scoped>
.slide-fade-enter-active {
	transition: all 0.3s ease-out;
}

.slide-fade-leave-active {
	transition: all 0.3s ease-in;
}

.slide-fade-enter-from,
.slide-fade-leave-to {
	transform: translateX(20px);
	opacity: 0;
}
</style>
