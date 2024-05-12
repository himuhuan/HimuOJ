<template>
	<n-card :title="`提交 #${props.commitDetail?.id}`" size="large">
		<template #header-extra>
			<n-tag
				type="success"
				size="large"
				v-if="commitDetail?.status === ExecutionStatus.ACCEPTED"
			>
				已通过
			</n-tag>
			<n-tag
				type="error"
				size="large"
				v-else-if="commitDetail?.status === ExecutionStatus.COMPILE_ERROR"
			>
				编译错误
			</n-tag>
			<n-tag
				type="default"
				size="large"
				v-else-if="commitDetail?.status === ExecutionStatus.PENDING"
			>
				等待测试中
			</n-tag>
			<n-tag
				type="error"
				size="large"
				v-else-if="commitDetail?.status === ExecutionStatus.WRONG_ANSWER"
			>
				结果错误
			</n-tag>
			<n-tag
				type="error"
				size="large"
				v-else-if="commitDetail?.status === ExecutionStatus.RUNTIME_ERROR"
			>
				运行错误
			</n-tag>
		</template>
		<n-descriptions title="提交信息" label-placement="left">
			<n-descriptions-item label="提交编号">
				<n-a :href="`/commits/${props.commitDetail?.id}`">{{
					props.commitDetail?.id
				}}</n-a>
			</n-descriptions-item>
			<n-descriptions-item label="提交用户">
				<n-a :href="`/user/${props.commitDetail?.userId}/profile`" style="text-decoration: none">
					<n-space align="center">
						<n-avatar size="small" circle :src="userBrief?.avatarUri" />
						<n-text>{{ userBrief?.userName }}</n-text>
					</n-space>
				</n-a>
			</n-descriptions-item>
		</n-descriptions>
		<n-hr />
		<n-collapse :default-expanded-names="['source-code', 'run-result']">
			<n-collapse-item title="源代码" name="source-code">
				<n-card embedded>
					<n-spin :show="sourceCode === undefined">
						<n-code
							:code="sourceCode ?? ''"
							class="pre-wrapper"
							language="cpp"
							show-line-numbers
						/>
					</n-spin>
				</n-card>
			</n-collapse-item>
			<n-collapse-item
				v-if="commitDetail?.status === 'COMPILE_ERROR'"
				title="编译输出"
				name="compiler-output"
			>
				<n-card embedded title="编译错误">
					<n-text type="error">
						<pre class="pre-wrapper"
							>{{ commitDetail?.compilerInformation.messageFromCompiler }} </pre
						>
					</n-text>
				</n-card>
			</n-collapse-item>
			<n-collapse-item
				title="运行结果"
				name="run-result"
				v-if="commitDetail?.status !== ExecutionStatus.COMPILE_ERROR"
			>
				<n-alert type="warning" title="注意">
					HimuOJ 官方测试不会限制用户自由下载测试的相关数据与提交源代码,
					部分测试不支持下载源文件或测试数据，具体信息询问该测试的发布者。
				</n-alert>
				<n-collapse v-for="(result, index) in commitDetail?.testPointResults">
					<n-card
						size="small"
						style="margin-top: 5px"
						:class="{
							'error-result': commitDetail?.status !== ExecutionStatus.ACCEPTED,
							'success-result':
								commitDetail?.status === ExecutionStatus.ACCEPTED,
							'pending-result':
								commitDetail?.status === ExecutionStatus.PENDING,
						}"
					>
						<n-collapse-item :title="'测试 #' + index">
							<template #header-extra>
								<n-space justify="space-between">
									<n-tag type="info" size="large">
										{{ Math.ceil(result.usage?.memoryByteUsed! / 1000) }} KB
									</n-tag>
									<n-tag type="info" size="large">
										{{ result.usage?.timeUsed }} ms
									</n-tag>
									<status-tag :status="result.testStatus" />
								</n-space>
							</template>
							<run-result-point-detail
								:data="result"
								:allow-download-answer="
									problemDetail?.allowDownloadAnswer ?? false
								"
								:allow-download-input="
									problemDetail?.allowDownloadInput ?? false
								"
							/>
						</n-collapse-item>
					</n-card>
				</n-collapse>
			</n-collapse-item>
		</n-collapse>
	</n-card>
</template>

<script setup lang="ts">
import { CommitDetail } from "@/models/CommitDetail.ts";
import { ExecutionStatus } from "@/models/ResultModels.ts";
import { PropType, ref, onMounted } from "vue";
import {
	NTag,
	NText,
	NCard,
	NCollapse,
	NCollapseItem,
	NDescriptions,
	NDescriptionsItem,
	NHr,
	NA,
	NSpin,
	NCode,
	NAvatar,
	NSpace,
	NAlert,
} from "naive-ui";
import { StaticFileServicesInstance } from "@/services/StaticFileServices.ts";
import { UserBriefInfo } from "@/models/User.ts";
import { UserServices } from "@/services/UserServices.ts";
import { ProblemsServices } from "@/services/ProblemsServices.ts";
import { useThemeVars } from "naive-ui";
import StatusTag from "../StatusTag.vue";
import RunResultPointDetail from "@/components/RunResultPointDetail.vue";
import ProblemDetail from "@/models/ProblemDetail.ts";

const props = defineProps({
	commitDetail: {
		type: Object as PropType<CommitDetail>,
		required: true,
	},
});

const sourceCode = ref<string | undefined>(undefined);
const userBrief = ref<UserBriefInfo | undefined>(undefined);
const problemDetail = ref<ProblemDetail | null>(null);

const themeVars = useThemeVars();

onMounted(async () => {
	sourceCode.value = await StaticFileServicesInstance.getRawTextContent(
		props.commitDetail?.sourceUri
	);
	userBrief.value = await UserServices.getUserBriefInfo(
		props.commitDetail?.userId.toString()
	);
	problemDetail.value = await ProblemsServices.getProblemDetailById(
		props.commitDetail?.problemId.toString()
	);
});
</script>

<style scoped>
.pre-wrapper {
	font-family: v-mono, serif;
	height: 50vh;
	overflow-y: scroll;
	white-space: pre-wrap;
	word-wrap: break-word;
}

.error-result {
	background-color: #fae7e7;
	border: 1px solid v-bind("themeVars.borderColor");
}

.success-result {
	background-color: #e8f6f03f;
	border: 1px solid v-bind("themeVars.borderColor");
}

.pending-result {
	background-color: #f5f5f5;
	border: 1px solid v-bind("themeVars.borderColor");
}
</style>
