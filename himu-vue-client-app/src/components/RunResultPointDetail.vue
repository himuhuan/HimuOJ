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
	<div v-else-if="props.data.testStatus === ExecutionStatus.SKIPPED">
		<n-text strong>
			由于之前的测试点没能通过测试，因此该测试点已被跳过。
		</n-text>
	</div>
	<div v-else-if="props.data.testStatus === ExecutionStatus.RUNNING">
		<n-text strong> 该测试点正在测试中。</n-text>
	</div>
	<div
		v-else-if="props.data.testStatus === ExecutionStatus.TIME_LIMIT_EXCEEDED"
	>
		<n-text strong> 该测试点测试超时。</n-text>
	</div>
	<div
		v-else-if="props.data.testStatus === ExecutionStatus.MEMORY_LIMIT_EXCEEDED"
	>
		<n-text strong> 该测试点测试超内存限制。</n-text>
	</div>
	<div v-else-if="props.data.testStatus === ExecutionStatus.WRONG_ANSWER">
		<n-space vertical>
			<n-text strong> 该测试点得到的结果与期望不符。</n-text>
			<n-descriptions
				:columns="1"
				bordered
				label-placement="left"
				:label-style="{ width: '100px' }"
			>
				<n-descriptions-item label="位于">
					{{ props.data.difference?.position }}
				</n-descriptions-item>
				<n-descriptions-item label="期望结果">
					{{ truncateStringWith(props.data.difference?.expected, 100) }}
				</n-descriptions-item>
				<n-descriptions-item label="实际结果">
					<div style="color: red">
						{{ truncateStringWith(props.data.difference?.actual, 100) }}
					</div>
				</n-descriptions-item>
			</n-descriptions>
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
	NDescriptionsItem,
	NDescriptions,
} from "naive-ui";
import { TestPointServicesInstance } from "@/services/TestPointServices";
import { truncateStringWith } from "@/utils/StringUtils";

const props = defineProps<{
	data: PointRunResult;
	allowDownloadAnswer: boolean;
	allowDownloadInput: boolean;
}>();
</script>
