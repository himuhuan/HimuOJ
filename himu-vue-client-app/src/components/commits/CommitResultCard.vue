<template>
	<n-card :title="`提交 #${props.commitDetail?.id}`" size="large">
		<template #header-extra>
			<status-tag :status="props.commitDetail?.status" />
		</template>
		<n-descriptions title="提交信息" label-placement="left" :column="1" bordered>
			<n-descriptions-item label="提交编号">
				<n-a :href="`/commits/${props.commitDetail?.id}`">{{
					props.commitDetail?.id
				}}</n-a>
			</n-descriptions-item>
			<n-descriptions-item label="提交用户">
				<n-a
					:href="`/user/${props.commitDetail?.userId}/profile`"
					style="text-decoration: none"
				>
					<n-space align="center">
						<n-avatar size="small" circle :src="userBrief?.avatarUri" />
						<n-text>{{ userBrief?.userName }}</n-text>
					</n-space>
				</n-a>
			</n-descriptions-item>
			<n-descriptions-item label="提交时间">
				{{ props.commitDetail?.commitDate }}
			</n-descriptions-item>
			<n-descriptions-item label="编译信息">
				<n-code>
				{{ props.commitDetail?.compilerPreset.language }} ({{
					props.commitDetail?.compilerPreset.command
				}})
				</n-code>
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
							>{{ commitDetail?.messageFromCompiler }} </pre
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
							'other-result':
								result?.testStatus === ExecutionStatus.PENDING ||
								result?.testStatus === ExecutionStatus.SKIPPED,
							'success-result': result?.testStatus === ExecutionStatus.ACCEPTED,
							'error-result': result?.testStatus !== ExecutionStatus.ACCEPTED,
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
import { PropType, ref, onMounted, onActivated } from "vue";
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

const customThemeVars = {
	errorCardColor: themeVars.value.errorColor + "10",
	successCardColor: themeVars.value.successColor + "10",
	otherCardColor: themeVars.value.warningColor + "10",
};


onMounted(async () => {
	console.log(themeVars);
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
	background-color: v-bind("customThemeVars.errorCardColor");
	border: 1px solid v-bind("themeVars.borderColor");
}

.success-result {
	background-color: v-bind("customThemeVars.successCardColor");
	border: 1px solid v-bind("themeVars.borderColor");
}

.other-result {
	background-color: v-bind("customThemeVars.otherCardColor");
	border: 1px solid v-bind("themeVars.borderColor");
}
</style>
