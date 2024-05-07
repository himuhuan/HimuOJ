<template>
	<center-content-container>
		<n-card>
			<n-space justify="center" align="center">
				<n-icon :size="32" :color="themeVars.primaryColor">
					<mark-email-read-outlined />
				</n-icon>
				<div id="verify-title">验证您的邮箱</div>
			</n-space>
			<n-hr />
			<n-text>
				Himu 已将含有<n-text type="primary" strong> 6位数字 </n-text
				>的账户激活令牌发放至您注册时绑定的邮件,
				请检测您的收件箱并在下方输入框内验证您的令牌。
			</n-text>
			<n-card :bordered="false">
				<n-space vertical>
					<n-input
						:allow-input="onlyInputNumber"
						placeholder="令牌"
						v-model:value="state.userVerification.confirmationToken"
					/>
					<n-space style="margin-top: 5vh" justify="space-between">
						<n-button tertiary type="primary" @click="showUserInfoForm = true">
							未收到邮件或者令牌已过期？
						</n-button>
						<hvr-icon-button text="验证" @click="handleVerifyClick" />
					</n-space>
					<transition name="slide-fade">
						<n-spin :show="spiningResent">
							<n-card
								style="margin-top: 5vh"
								title="输入您的注册时的用户信息以重新验证:"
								size="small"
								closable
								embedded
								@close="showUserInfoForm = false"
								v-if="showUserInfoForm"
							>
								<n-space vertical>
									<n-input
										placeholder="用户名"
										v-model:value="state.userVerification.userName"
									/>
									<n-input
										placeholder="邮箱"
										v-model:value="state.userVerification.userMail"
									/>
									<n-button
										secondary
										style="float: right"
										type="primary"
										@click="handleResendClick"
									>
										验证
									</n-button>
								</n-space>
							</n-card>
						</n-spin>
					</transition>
				</n-space>
			</n-card>
		</n-card>
	</center-content-container>
</template>

<script lang="ts" setup>
import CenterContentContainer from "@/components/CenterContentContainer.vue";
import {
	NCard,
	NIcon,
	NSpace,
	NHr,
	NText,
	NInput,
	useThemeVars,
	NButton,
	NSpin,
	useMessage,
	useLoadingBar,
} from "naive-ui";
import { MarkEmailReadOutlined } from "@vicons/material";
import HvrIconButton from "@/components/HvrIconButton.vue";
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useUserState } from "@/services/UserStateServices";

const themeVars = useThemeVars();
const router = useRouter();
const message = useMessage();
const loadingBar = useLoadingBar();

const state = ref({
	userVerification: {
		confirmationToken: "",
		userMail: "",
		userName: router.currentRoute.value.query.userName as string,
	},
});
const userState = useUserState();

const showUserInfoForm = ref(false);
const spiningResent = ref(false);

const onlyInputNumber = (value: string) => !value || /^\d+$/.test(value);

const handleVerifyClick = async () => {
	loadingBar.start();
	if (state.value.userVerification.confirmationToken.length !== 6) {
		message.error("令牌长度不正确");
		return;
	}

	const res = await userState.vaildateRegisterCode(
		state.value.userVerification.userName,
		state.value.userVerification.confirmationToken
	);

	if (!res) {
		loadingBar.error();
	} else {
		loadingBar.finish();
		message.success("验证成功");
		router.push({ name: "authentication" });
	}
};

const handleResendClick = async () => {
	spiningResent.value = true;
	const res = await userState.resentRegisterCode(
		state.value.userVerification.userName,
		state.value.userVerification.userMail
	);
	spiningResent.value = false;
	if (res) {
		message.success("发送成功");
	}
};
</script>

<style scoped>
#verify-title {
	font-size: 16px;
	font-weight: bold;
	border-bottom: 3px v-bind("themeVars.primaryColor") dotted;
}
</style>
