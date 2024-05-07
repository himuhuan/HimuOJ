<template>
	<div
		v-if="props.data.testStatus === ExecutionStatus.ACCEPTED"
		style="margin: 20px 0"
	>
		<n-text strong> 该测试点已通过测试。</n-text>
	</div>
	<div v-else-if="props.data.testStatus === ExecutionStatus.PENDING">
		<n-text strong> 该测试点正在等待测试。</n-text>
	</div>
	<div v-else-if="props.data.testStatus === ExecutionStatus.RUNNING">
		<n-text strong> 该测试点正在测试中。</n-text>
	</div>
	<div v-else-if="props.data.testStatus === ExecutionStatus.WRONG_ANSWER">
		<n-space vertical>
			<n-text strong> 该测试点得到的结果与期望不符。</n-text>
			<n-input-group>
				<n-input-group-label>位于</n-input-group-label>
				<n-input
					readonly
					:default-value="props.data.difference?.position.toString()"
				></n-input>
			</n-input-group>
			<n-input-group>
				<n-input-group-label>期望结果</n-input-group-label>
				<n-input
					readonly
					:default-value="
						truncateStringWith(props.data.difference?.expected, 50)
					"
				></n-input>
			</n-input-group>
			<n-input-group>
				<n-input-group-label>实际结果</n-input-group-label>
				<n-input
					readonly
					status="error"
					:default-value="truncateStringWith(props.data.difference?.actual, 50)"
				></n-input>
			</n-input-group>
		</n-space>
	</div>
	<n-hr />
	<n-space>
		<n-button
			v-if="props.allowDownloadInput"
			tag="a"
			:href="
				TestPointServicesInstance.getTestPointInputUrl(
					props.data.testPointId.toString()
				)
			"
			download
		>
			下载输入
		</n-button>
		<n-button
			v-if="props.allowDownloadAnswer"
			tag="a"
			:href="
				TestPointServicesInstance.getTestPointAnswerUrl(
					props.data.testPointId.toString()
				)
			"
			>下载答案</n-button
		>
	</n-space>
</template>

<script setup lang="ts">
import { PointRunResult, ExecutionStatus } from "@/models/ResultModels.ts";
import {
	NText,
	NButton,
	NSpace,
	NHr,
	NInput,
	NInputGroup,
	NInputGroupLabel,
} from "naive-ui";
import { TestPointServicesInstance } from "@/services/TestPointServices";
import { truncateStringWith } from "@/utils/StringUtils";

const props = defineProps<{
	data: PointRunResult;
	allowDownloadAnswer: boolean;
	allowDownloadInput: boolean;
}>();
</script>
