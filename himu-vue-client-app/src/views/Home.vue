<template>
	<div class="home-intro" v-if="userState.user == null">
		<n-h1>
			<n-gradient-text :gradient="gradient" class="italic-font home-title"> Himu Online Judge </n-gradient-text>
		</n-h1>
		<div style="text-align: center">简约但不简单。</div>
		<div style="text-align: center; margin-bottom: 5px">Himu 祝您的OJ之旅愉快!</div>
		<n-space justify="space-between">
			<n-button
				strong
				secondary
				type="primary"
				size="large"
				class="hvr-icon-forward"
				@click="
					() => {
						router.push({ name: 'authentication' });
					}
				"
			>
				<n-space justify="space-between">
					<n-icon class="hvr-icon">
						<arrow-forward />
					</n-icon>
					登录/注册
				</n-space>
			</n-button>
			<n-button strong secondary type="primary" size="large">
				<n-space justify="space-between">
					<n-icon>
						<logo-github />
					</n-icon>
					<span>GitHub</span>
				</n-space>
			</n-button>
		</n-space>
	</div>
	<himu-copyright v-if="userState.user == null" />
</template>

<script lang="ts" setup>
import { NGradientText, NH1, NButton, NSpace, NIcon, useThemeVars } from "naive-ui";

import { LogoGithub } from "@vicons/ionicons5";
import { ArrowForward } from "@vicons/ionicons5";
import router from "@/routers";
import HimuCopyright from "@/components/HimuCopyright.vue";
import { useUserState } from "@/services/UserStateServices";

const themeVars = useThemeVars();
const userState = useUserState();
const gradient = {
	from: themeVars.value.primaryColor,
	to: themeVars.value.primaryColorHover,
	deg: 180,
};
</script>

<style scoped>
.home-intro {
	width: 100%;
	height: 100vh;
	display: flex;
	flex-direction: column;
	align-items: center;
	justify-content: center;
}

.home-title {
	border-bottom: 3px dotted v-bind("themeVars.primaryColor");
}

n-text {
	color: v-bind("themeVars.textColor1");
	font-size: 1.5rem;
}
</style>
