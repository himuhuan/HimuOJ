<script setup lang="ts">
import {
	NCard,
	NGrid,
	NGridItem,
	NNumberAnimation,
	NProgress,
	NSkeleton,
	NStatistic,
	useLoadingBar,
} from "naive-ui";
import { onMounted, PropType, reactive } from "vue";
import { UserDetailInfo } from "@/models/User";
import { Status } from "naive-ui/es/progress/src/interface";
import { HimuCommitList } from "@/models/HimuCommitList.ts";
import { CommitsServices } from "@/services/CommitsServices.ts";
import CommitListFilter from "@/models/CommitListFilter.ts";
import CommitList from "@/components/commits/CommitList.vue";
import RealTimeConnection from "@/services/real_time/RealTimeConnection";

const loadingBar = useLoadingBar();

const props = defineProps({
	userInfo: {
		type: Object as PropType<UserDetailInfo>,
		required: true,
	},
});

const commitAcceptedRate = function getCommitAcceptedRate() {
	const rate = Math.ceil(
		(props.userInfo.commitAccepted / props.userInfo.totalCommits) * 100
	);
	if (rate > 80) {
		return {
			percentage: rate,
			status: "success" as Status,
		};
	}
	if (rate > 60) {
		return {
			percentage: rate,
			status: "warning" as Status,
		};
	}
	return {
		percentage: rate,
		status: "error" as Status,
	};
};

const defaultFilter: CommitListFilter = {
	page: 1,
	pageSize: 10,
	userId: props.userInfo?.id,
};

const state = reactive({
	userCommitListData: null as HimuCommitList | null,
	loading: true,
	filter: {
		...defaultFilter,
	} as CommitListFilter,
});

async function fetchCommitListData(filter: CommitListFilter) {
	state.loading = true;
	state.userCommitListData = await CommitsServices.filterCommitList(filter);
	// for testing loading effect
	// await new Promise((resolve) => setTimeout(resolve, 1000));
	state.filter = { ...filter };
	state.loading = false;
}

let realTimeConnection: RealTimeConnection | null = null;

onMounted(async () => {
	loadingBar.start();
	try {
		realTimeConnection = await RealTimeConnection.create();
		await fetchCommitListData(state.filter);
	} catch (e) {
		console.error(e);
		loadingBar.error()
	} finally {
		loadingBar.finish();
	}
});
</script>

<template>
	<div style="margin: 5px">
		<n-card title="提交分析">
			<n-grid :x-gap="12" cols="1 m:2" responsive="screen">
				<n-grid-item :span="1">
					<n-statistic label="总提交次数" tabular-nums>
						<n-number-animation
							:from="0"
							:to="userInfo.totalCommits"
							:duration="1000"
						/>
					</n-statistic>
				</n-grid-item>
				<n-grid-item :span="1">
					<n-statistic label="已解决问题" tabular-nums>
						<n-number-animation
							:from="0"
							:to="userInfo.problemSolved"
							:duration="1000"
						/>
					</n-statistic>
				</n-grid-item>
				<n-grid-item :span="1">
					<n-statistic label="通过的提交" tabular-nums>
						<n-number-animation
							:from="0"
							:to="userInfo.commitAccepted"
							:duration="1000"
						/>
					</n-statistic>
				</n-grid-item>
				<n-grid-item :span="1">
					<n-statistic label="提交成功率" tabular-nums>
						<n-number-animation
							:from="0"
							:to="commitAcceptedRate().percentage"
							:duration="1000"
						/>
						<template #suffix>%</template>
					</n-statistic>
					<n-progress
						:status="commitAcceptedRate().status"
						:percentage="commitAcceptedRate().percentage"
					/>
				</n-grid-item>
			</n-grid>
		</n-card>
		<n-card title="提交记录" style="margin-top: 5px">
			<transition name="slide-up" mode="out-in">
				<div v-if="state.loading">
					<n-skeleton :height="60" text :repeat="10" />
				</div>
				<div v-else>
					<commit-list
						:default-filter="defaultFilter"
						:commit-list-data="state.userCommitListData"
						:filter="state.filter"
						:filter-enabled="true"
						@updateData="fetchCommitListData"
					/>
				</div>
			</transition>
		</n-card>
	</div>
</template>
