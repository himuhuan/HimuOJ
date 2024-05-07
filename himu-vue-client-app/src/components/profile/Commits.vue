<script setup lang="ts">
import {
	NCard,
	NGrid,
	NGridItem,
	NStatistic,
	NNumberAnimation,
	NProgress,
} from "naive-ui";
import { PropType } from "vue";
import { UserDetailInfo } from "@/models/User";
import { Status } from "naive-ui/es/progress/src/interface";
import CommitList from "@/components/commits/CommitList.vue";

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
	if (rate > 80) return { percentage: rate, status: "success" as Status };
	if (rate > 60) return { percentage: rate, status: "warning" as Status };
	return { percentage: rate, status: "error" as Status };
};
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
			<commit-list :user-id="userInfo.id.toString()" />
		</n-card>
	</div>
</template>
