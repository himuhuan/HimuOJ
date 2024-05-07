<template>
	<n-grid :cols="16" id="himu-navbar" item-responsive responsive="screen">
		<n-grid-item class="n-grid-item" span="8 m:2">
			<NSpace align="center">
				<n-avatar :src="src" size="large" />
				<n-a class="himu-nav-title hvr-underline-from-center">
					<router-link to="/"> {{ props.title }}</router-link>
				</n-a>
			</NSpace>
		</n-grid-item>
		<n-grid-item class="n-grid-item" span="0 m:8">
			<n-menu
				:options="mainNavMenuOptions"
				mode="horizontal"
				style="justify-content: space-around"
			>
			</n-menu>
		</n-grid-item>
		<n-grid-item class="n-grid-item" span="8 m:6">
			<n-menu
				v-if="userState.user == null"
				:options="authenticationMenuOptions"
				mode="horizontal"
			></n-menu>
			<n-dropdown
				animated
				:options="userInfoMenuOptions"
				@select="handleSelect"
			>
				<n-icon :color="themeVars.textColor1" size="large">
					<setting-icon></setting-icon>
				</n-icon>
			</n-dropdown>
		</n-grid-item>
	</n-grid>
</template>

<script lang="ts" setup>
import {
	NGrid,
	NGridItem,
	NAvatar,
	NSpace,
	NA,
	useMessage,
	useThemeVars,
	NMenu,
	NIcon,
	NDropdown,
} from "naive-ui";
import HimuLogo from "@/assets/images/himu-logo.svg";
import type { MenuOption, DropdownOption } from "naive-ui";
import { useUserState } from "@/services/UserStateServices";
import { RouterLink } from "vue-router";
import { h } from "vue";

// icons
import {
	HomeRound as HomeIcon,
	QuestionAnswerOutlined as DisscussionIcon,
	StarOutlineRound as ProblemIcon,
	PlaylistAddCheckRound as ContestIcon,
	SettingsSuggestFilled as SettingIcon,
} from "@vicons/material";

import {
	PersonAddOutline,
	ColorWandOutline as ThemeIcon,
} from "@vicons/ionicons5";

const props = defineProps({
	title: {
		type: String,
		default: "Himu OJ",
	},
	src: {
		type: String,
		default: HimuLogo,
	},
	height: {
		type: Number,
		default: 48,
	},
	userId: {
		type: String,
		default: null,
	},
});

const userState = useUserState();
const themeVars = useThemeVars();
window.$message = useMessage();

const mainNavMenuOptions: MenuOption[] = [
	{
		label: () =>
			h(
				RouterLink,
				{ to: `/profile` },
				{ default: () => (userState.user ? userState.user.name : "首页") }
			),
		key: "home",
		icon: () => h(NIcon, null, { default: () => h(HomeIcon) }),
	},
	{
		label: () => h(RouterLink, { to: "/problems" }, { default: () => "问题" }),
		key: "problems",
		icon: () => h(NIcon, null, { default: () => h(ProblemIcon) }),
	},
	{
		label: () => h(RouterLink, { to: "/contests" }, { default: () => "比赛" }),
		key: "contests",
		icon: () => h(NIcon, null, { default: () => h(ContestIcon) }),
	},
	{
		label: () =>
			h(RouterLink, { to: "/discussions" }, { default: () => "讨论" }),
		key: "discussions",
		icon: () => h(NIcon, null, { default: () => h(DisscussionIcon) }),
	},
];

const authenticationMenuOptions: MenuOption[] = [
	{
		label: () =>
			h(RouterLink, { to: "/authentication" }, { default: () => "注册/登录" }),
		key: "authentication",
		icon: () => h(NIcon, null, { default: () => h(PersonAddOutline) }),
	},
];

const userInfoMenuOptions: DropdownOption[] = [
	{
		label: () => "切换主题",
		key: "theme-switch",
		icon: () => h(NIcon, null, { default: () => h(ThemeIcon) }),
	},
];

const handleSelect = (key: string | number) => {
	if (key === "theme-switch") {
		userState.triggerThemeChange();
		window.$message.info("已切换主题");
	}
};
</script>

<style scoped>
#himu-navbar {
	z-index: 1000;
	position: sticky;
	backdrop-filter: blur(10px);
	top: 0;
	width: 100%;
	height: v-bind(height + "px");
	background-color: v-bind("themeVars.bodyColor");
	border-bottom: 1px solid v-bind("themeVars.borderColor");
}

.n-grid-item {
	display: flex;
	align-items: center;
	padding: 0 16px;
	justify-content: space-around;
}

.himu-nav-title {
	text-decoration: none;
	color: v-bind("themeVars.textColor1");
	font-size: v-bind(height / 3 + "px");
	font-weight: bold;
	font-style: italic;
	font-family: "Victor Mono", "Consolas", "Microsoft Yahei", "PingFang SC",
		"Helvetica Neue", "Helvetica", "Arial", sans-serif;
}

.hvr-underline-from-center {
	display: inline-block;
	vertical-align: middle;
	-webkit-transform: perspective(1px) translateZ(0);
	transform: perspective(1px) translateZ(0);
	box-shadow: 0 0 1px rgba(0, 0, 0, 0);
	position: relative;
	overflow: hidden;
}

.hvr-underline-from-center:before {
	content: "";
	position: absolute;
	z-index: -1;
	left: 51%;
	right: 51%;
	bottom: 0;
	background: v-bind("themeVars.primaryColor");
	height: 3px;
	-webkit-transition-property: left, right;
	transition-property: left, right;
	-webkit-transition-duration: 0.3s;
	transition-duration: 0.3s;
	-webkit-transition-timing-function: ease-out;
	transition-timing-function: ease-out;
}

.hvr-underline-from-center:hover:before,
.hvr-underline-from-center:focus:before,
.hvr-underline-from-center:active:before {
	left: 0;
	right: 0;
}

a {
	text-decoration: none;
}

a:visited {
	color: v-bind("themeVars.textColor1");
}
</style>
