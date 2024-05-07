import { createRouter, createWebHistory } from "vue-router";
import Cookie from "js-cookie";
import { useUiServices } from "@/services/UiServicesProvider";

const routes = [
	{
		path: "/",
		name: "home",
		meta: { requireAuthentication: true },
		component: () => import("../views/Home.vue"),
	},
	{
		path: "/authentication",
		name: "authentication",
		component: () => import("@/views/Authentication.vue"),
	},
	{
		path: "/validate-code",
		name: "validate-code",
		component: () => import("@/views/ValidateCode.vue"),
	},
	{
		path: "/user/:userId/profile",
		name: "profile",
		props: true,
		meta: { requireAuthentication: true },
		component: () => import("@/views/Profile.vue"),
	},
	{
		path: "/profile",
		name: "self-profile",
		props: true,
		meta: { requireAuthentication: true },
		component: () => import("@/views/Profile.vue"),
	},
	{
		path: "/error/not-found",
		name: "not-found",
		component: () => import("@/views/NotFoundView.vue"),
	},
	{
		path: "/reset-password",
		name: "reset-password",
		meta: { requireAuthentication: true },
		component: () => import("@/views/ResetPassword.vue"),
	},
	{
		path: "/problems",
		name: "problems",
		component: () => import("@/views/Problems.vue"),
	},
	// visit problem by problem id
	{
		path: "/problems/:problemId",
		name: "problem-id-detail",
		meta: { requireAuthentication: true },
		props: true,
		component: () => import("@/views/ProblemView.vue"),
	},
	// visit problem by contest & problem code
	{
		path: "/contests/:contestCode/problems/:problemCode",
		name: "problem-detail",
		meta: { requireAuthentication: true },
		props: true,
		component: () => import("@/views/ProblemView.vue"),
	},
	{
		path: "/commits/:commitId",
		name: "commit-detail",
		meta: { requireAuthentication: true },
		props: true,
		component: () => import("@/views/CommitResultView.vue"),
	},
	{
		path: "/creator/apply",
		name: "creator-apply",
		meta: { requireAuthentication: true },
		component: () => import("@/views/ApplyCreator.vue"),
	},
	{
		path: "/creator/problem_guide",
		name: "create-problem-guide",
		props: (router: any) => ({ contestCode: router.query.contestCode }),
		meta: { requireAuthentication: true },
		component: () => import("@/views/guide/PublishProblem.vue"),
	},
	{
		path: "/_debug",
		component: () => import("@/views/Debug.vue"),
	},
];

const router = createRouter({ history: createWebHistory(), routes });

router.beforeEach((to, from, next) => {
	useUiServices().loadingBar?.start();
	if (to.matched.length === 0) {
		next({ name: "not-found" });
	} else if (to.matched.some((record) => record.meta.requireAuthentication)) {
		if (Cookie.get("accessToken") != null) {
			if (to.name === "home") next({ name: "problems" });
			next();
		} else {
			window.$message.error("未登录或登录已过期，请重新登录");
			next({ name: "authentication" });
		}
	} else {
		next();
	}
});

router.afterEach(() => {
	useUiServices().loadingBar?.finish();
});

export default router;
