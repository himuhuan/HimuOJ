<template>
	<content-with-side-bar
		@menu-update="
			(item) => {
				currentComponent = item.key;
				state.menuItem = item.key;
			}
		"
		v-if="!state.loading"
		:value="state.menuItem"
		:user-info="state.userInfo!"
	>
		<transition name="slide-fade" mode="out-in">
			<component :is="componentToShow" :user-info="state.userInfo!"></component>
		</transition>
	</content-with-side-bar>
	<n-spin v-else>
		<div style="height: 100vh; backdrop-filter: blur(20px)"></div>
	</n-spin>
</template>

<!--suppress JSUnusedLocalSymbols -->
<script lang="ts" setup>
import { NSpin } from "naive-ui";
import router from "@/routers";
import {
	reactive,
	onMounted,
	nextTick,
	defineAsyncComponent,
	computed,
	ref,
} from "vue";
import { UserDetailInfo } from "@/models/User";
import { UserServices } from "@/services/UserServices.ts";
import ContentWithSideBar from "@/components/ContentWithSideBar.vue";
import { useUserState } from "@/services/UserStateServices";

////////////////////////////// variables //////////////////////////////

const userState = useUserState();

const props = defineProps({
	userId: {
		type: String,
	},
});

const state = reactive({
	userId: props.userId as string,
	userInfo: null as UserDetailInfo | null,
	userCommitTotalCount: 0,
	loading: true,
	menuItem: "info",
});

const vaildPages = [
	"info",
	"commits",
	"my-problem-list",
	"friends",
	"creator-center",
	"favourite",
];

////////////////////////////// components //////////////////////////////

const InfoComponent = defineAsyncComponent(() => {
	return import("@/components/profile/Info.vue");
});

const CommitsComponent = defineAsyncComponent(() => {
	return import("@/components/profile/Commits.vue");
});

const CreatorDashboardComponent = defineAsyncComponent(() => {
	return import("@/components/profile/CreatorCenter.vue");
});

const currentComponent = ref("info");
const componentToShow = computed(() => {
	return getCurrentComponent(currentComponent.value);
});

////////////////////////////// mount & computed //////////////////////////////
onMounted(async () => {
	// check if the user is logged in
	if (!props.userId) {
		if (userState.isLogin) {
			state.userId = userState.id;
		} else {
			window.$message.error("未登录或登录已过期，请重新登录");
			await router.push({name: "authentication"});
		}
	}
	state.loading = true;
	await nextTick();
	UserServices.getUserDetailInfo(state.userId).then(
		async (res) => {
			state.userInfo = res;
			state.loading = false;
			await nextTick();
		}
	);
	if (router.currentRoute.value.query.page) {
		const target = router.currentRoute.value.query.page as string;
		currentComponent.value = target;
		state.menuItem = target;
		if (!vaildPages.includes(target)) {
			await router.push({ name: "not-found" });
		}
	}
});

////////////////////////////// functions //////////////////////////////

function getCurrentComponent(componentName: string) {
	switch (componentName) {
		case "info":
			return InfoComponent;
		case "commits":
			return CommitsComponent;
		case "creator-dashboard":
			return CreatorDashboardComponent;
		default:
			return InfoComponent;
	}
}
</script>

<style scoped></style>
