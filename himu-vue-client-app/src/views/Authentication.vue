<template>
	<center-content-container
		:background-color="themeVars.baseColor"
		top-offset="5vh"
	>
		<n-card>
			<n-space justify="center" align="center">
				<img src="@/assets/images/himu-logo.svg" width="64" height="64" />
				<div class="italic-font" style="font-size: 16px">Himu Online Judge</div>
			</n-space>
			<n-card :bordered="false">
				<n-tabs
					justify-content="space-around"
					type="line"
					class="card-tabs"
					default-value="login"
					size="large"
					animated
					pane-wrapper-style="margin: 0 -4px"
					pane-style="padding-left: 4px; padding-right: 4px; box-sizing: border-box;"
				>
					<n-tab-pane name="login" tab="登录">
						<n-form :rules="rules" ref="modelRef" :model="formState">
							<n-form-item-row label="用户名" path="login.input">
								<n-input
									placeholder="用户名"
									v-model:value="formState.login.input"
								/>
							</n-form-item-row>
							<n-form-item-row label="密码" path="login.password">
								<n-input
									type="password"
									placeholder="密码"
									show-password-on="click"
									v-model:value="formState.login.password"
								/>
							</n-form-item-row>
						</n-form>
						<n-button
							type="primary"
							block
							secondary
							strong
							@click="loginButtonClick"
						>
							登录
						</n-button>
					</n-tab-pane>
					<n-tab-pane name="register" tab="注册">
						<n-form :rules="rules" ref="modelRef" :model="formState">
							<n-form-item-row label="邮箱" path="register.mail">
								<n-input
									placeholder="邮箱"
									v-model:value="formState.register.mail"
								>
									<template #prefix>
										<n-icon>
											<mail-outline />
										</n-icon>
									</template>
								</n-input>
							</n-form-item-row>
							<n-form-item-row label="用户名" path="register.userName">
								<n-input
									placeholder="用户名"
									v-model:value="formState.register.userName"
								/>
							</n-form-item-row>
							<n-form-item-row label="密码" path="register.password">
								<n-input
									type="password"
									show-password-on="click"
									placeholder="密码"
									v-model:value="formState.register.password"
								/>
							</n-form-item-row>
							<n-form-item-row
								label="重复密码"
								path="register.repeatedPassword"
							>
								<n-input
									type="password"
									placeholder="重复密码"
									show-password-on="click"
									v-model:value="formState.register.repeatedPassword"
								/>
							</n-form-item-row>
							<n-form-item-row label="联系电话" path="register.phone">
								<n-input
									placeholder="联系电话"
									v-model:value="formState.register.phone"
								/>
							</n-form-item-row>
							<n-alert type="info" title="注意">
								注册账户，即表明您已同意我们的<n-a href="#"
									>隐私政策和服务条款。</n-a
								>
								<br />
								在点击注册后, Himu酱将会向您的邮箱发送一封包含验证令牌的邮件,
								请注意查收。
							</n-alert>
							<n-form-item-row>
								<n-button
									type="primary"
									block
									secondary
									strong
									@click="registerButtonClick"
								>
									注册
								</n-button>
							</n-form-item-row>
						</n-form>
					</n-tab-pane>
				</n-tabs>
			</n-card>
		</n-card>
	</center-content-container>
</template>

<script lang="ts" setup>
import {
	NCard,
	NTabs,
	NTabPane,
	NForm,
	NFormItemRow,
	NInput,
	NButton,
	useThemeVars,
	NSpace,
	NIcon,
	NA,
	NAlert,
	FormRules,
	FormItemRule,
	useMessage,
	useLoadingBar,
} from "naive-ui";

import { ref } from "vue";
import CenterContentContainer from "@/components/CenterContentContainer.vue";
import { MailOutline } from "@vicons/ionicons5";
import router from "@/routers";
import { useUserState } from "@/services/UserStateServices";
import { AuthServices } from "@/services/AuthServices";

const themeVars = useThemeVars().value;

// user input
const modelRef = ref(null);
const formState = ref({
	login: {
		input: "",
		password: "",
		method: "user",
	},
	register: {
		userName: "",
		password: "",
		mail: "",
		repeatedPassword: "",
		phone: "",
	},
});

const rules: FormRules = {
	login: {
		input: {
			required: true,
			message: "请输入用户名",
			trigger: "blur",
		},
		password: {
			required: true,
			message: "请输入密码",
			trigger: ["blur"],
		},
	},
	register: {
		userName: {
			required: true,
			message: "请输入用户名",
			trigger: "blur",
		},
		password: {
			required: true,
			validator: (rule: FormItemRule, value: string) => {
				if (!value) return new Error("请输入密码");
				else if (
					value.length < 8 ||
					value.indexOf(formState.value.register.userName) != -1
				) {
					return new Error("密码长度至少为8位且不能包含用户名");
				}
				return true;
			},
			trigger: ["blur"],
		},
		repeatedPassword: {
			required: true,
			validator: (rule: FormItemRule, value: string) => {
				if (!value) return new Error("请重复输入密码");
				else if (value != formState.value.register.password) {
					return new Error("两次输入的密码不一致");
				}
				return true;
			},
			trigger: ["blur"],
		},
		mail: {
			required: true,
			validator: (rule: FormItemRule, value: string) => {
				if (!value) return new Error("请输入邮箱");
				else if (
					!value.match(/^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$/)
				) {
					return new Error("请输入正确的邮箱地址");
				}
				return true;
			},
			trigger: ["blur"],
		},
	},
};

const message = useMessage();
const loadingBar = useLoadingBar();
const userState = useUserState();

const loginButtonClick = async () => {
	loadingBar.start();
	const res = await userState
		.login(formState.value.login.input, formState.value.login.password)
		.catch((e) => {
			return undefined;
		});
	if (res) {
		loadingBar.finish();
		router.push({ name: "self-profile"});
	} else {
		loadingBar.error();
	}
};

const registerButtonClick = async () => {
	loadingBar.start();
	const result = await AuthServices.register(
		formState.value.register.userName,
		formState.value.register.password,
		formState.value.register.repeatedPassword,
		formState.value.register.mail,
		formState.value.register.phone
	).catch((e) => {
		return null;
	});

	if (!result || result.code != 0) {
		loadingBar.error();
	} else {
		message.success("注册成功! 请前往邮箱查收验证邮件");
		loadingBar.finish();
		router.push({
			name: "validate-code",
			query: { userName: formState.value.register.userName },
		});
	}
};
</script>

<style scoped>
.card-tabs .n-tabs-nav--bar-type {
	padding-left: 4px;
}
</style>
