<script setup lang="ts">
import { onMounted, reactive, toRaw } from "vue";
import * as monaco from "monaco-editor";
import {
	NAlert,
	NButton,
	NCard,
	NIcon,
	NP,
	NSelect,
	NSpace,
	NTabPane,
	NSpin,
	NTabs,
	NText,
	NUpload,
	NUploadDragger,
	SelectOption,
	UploadCustomRequestOptions,
	useThemeVars,
} from "naive-ui";

import RealTimeConnection from "@/services/real_time/RealTimeConnection";

import { CodeTemplateInstance } from "@/utils/CodeTemplate.ts";
import { CommitsServices } from "@/services/CommitsServices.ts";
import { useUserState } from "@/services/UserStateServices.ts";
import {
	SaveOutlined as SaveCodeIcon,
	UploadFileOutlined as SubmitIcon,
	WarningRound as WarningIcon,
} from "@vicons/material";
import CommitList from "@/components/commits/CommitList.vue";
import CommitListFilter from "@/models/CommitListFilter";
import { HimuCommitList } from "@/models/HimuCommitList";

const themeVars = useThemeVars();
const userState = useUserState();

const languageOptions: SelectOption[] = [
	{
		label: "c++",
		value: "g++",
	},
	{
		label: "C",
		value: "c",
	},
	{
		label: "Java",
		value: "java",
	},
	{
		label: "Python",
		value: "python",
	},
];

const props = defineProps({
	problemId: {
		type: String,
		required: true,
	},
});

const defaultFilter: CommitListFilter = {
	page: 1,
	pageSize: 10,
	userId: userState.id,
	problemId: props.problemId,
};

const state = reactive({
	loading: true,
	currentTab: "code-edit",
	editor: null as monaco.editor.IStandaloneCodeEditor | null,
	fileToUpload: null as File | null,
	selectedLanguage: languageOptions[0],
	filter: {
		...defaultFilter,
	} as CommitListFilter,
	commitListData: null as HimuCommitList | null,
});

////////////////////////// function //////////////////////////

let realTimeConnection: RealTimeConnection | null = null;

function changeTabs(tab: string | number) {
	state.currentTab = tab as string;
}

function handleHandleLanguageChange(value: string, option: SelectOption) {
	const editorRaw = toRaw(state.editor)!;
	const model = editorRaw.getModel();
	state.selectedLanguage = option;
	monaco.editor.setModelLanguage(model!, value);
	editorRaw.setValue(CodeTemplateInstance.getTemplate(value));
}

async function uploadSourceFile(options: UploadCustomRequestOptions) {
	if (options.file.file) {
		state.fileToUpload = options.file.file;
		const resp = await CommitsServices.postSourceFile(
			props.problemId,
			options.file.file,
			state.selectedLanguage.value as string
		);
		console.log(`commit ${resp} has submitted`);
	}
}

function removeUploadSourceFile() {
	state.fileToUpload = null;
}

async function handleSubmitCode() {
	if (state.currentTab === "code-edit") {
		const editorRaw = toRaw(state.editor)!;
		const model = editorRaw.getModel();
		const code = model!.getValue();
		const commitId = await CommitsServices.postSourceCode(
			props.problemId,
			code,
			state.selectedLanguage.value as string
		);
		await realTimeConnection?.invoke("CommitJudgeTask", commitId);
	}
	// refresh commit list
	state.commitListData = await CommitsServices.filterCommitList(state.filter);
	state.currentTab = "commits";
}

async function fetchCommitListData(filter: CommitListFilter) {
	state.loading = true;
	state.commitListData = await CommitsServices.filterCommitList(filter);
	state.filter = { ...filter };
	state.loading = false;
}

onMounted(async () => {
	state.editor = monaco.editor.create(document.getElementById("user-editor")!, {
		value: CodeTemplateInstance.getTemplate("cpp"),
		language: "cpp",
		fontSize: 14,
		fontLigatures: true,
		automaticLayout: true,
		lineHeight: 1.5,
		fontFamily: themeVars.value.fontFamilyMono,
		lineNumbers: "on",
		roundedSelection: false,
		scrollBeyondLastLine: false,
		readOnly: false,
		theme: userState.perferTheme === "dark" ? "vs-dark" : "vs",
	});
	try {
		realTimeConnection = await RealTimeConnection.create();
		realTimeConnection.on('CommitStatus', (id, stat) => {
			if (state.commitListData) {
				const commit = state.commitListData.data.find((c) => c.commitId === id);
				if (commit) {
					console.log('CommitStatus', id, stat);
					commit.commitStatus = stat;
				}
			}
		});
		fetchCommitListData(state.filter);
	} catch (e) {
		window.$message.error(`无法连接服务器: ${e}`);
		console.error(e);
	}
});
</script>

<template>
	<div class="problem-commit-tabs-container">
		<n-tabs
			default-value="code-edit"
			:on-update:value="changeTabs"
			:value="state.currentTab"
			animated
		>
			<template #suffix>
				<n-space>
					<n-select
						filterable
						default-value="C++"
						placeholder="选择语言..."
						:options="languageOptions"
						style="width: 200px"
						:on-update:value="handleHandleLanguageChange"
					></n-select>
					<n-button
						type="primary"
						:disabled="state.currentTab == 'commits'"
						@click="handleSubmitCode"
					>
						<n-icon size="medium" :component="SubmitIcon" /> &nbsp; 提交
					</n-button>
					<n-button
						type="primary"
						secondary
						:disabled="state.currentTab != 'code-edit'"
					>
						<n-icon size="medium" :component="SaveCodeIcon" /> &nbsp; 保存
					</n-button>
				</n-space>
			</template>
			<n-tab-pane
				name="code-edit"
				tab="代码"
				display-directive="show"
				style="height: 90vh; width: 100%"
			>
				<n-card bordered class="monaco-editor" id="user-editor"></n-card>
			</n-tab-pane>
			<n-tab-pane name="source" tab="源文件">
				<n-space vertical>
					<n-alert title="安全警告" type="error" closable>
						<template #icon>
							<n-icon size="larger" :component="WarningIcon" />
						</template>
						请不要试图上传一切可能危害服务器的代码或者文件。<br />
						否则，一旦被发现，你的账号将会被永久封禁。
					</n-alert>
					<n-alert title="注意" type="info" closable>
						请注意，你上传的文件必须与你选择的语言(
						{{ state.selectedLanguage?.label }} )相匹配，否则将会导致编译错误。
					</n-alert>
					<n-upload
						:custom-request="uploadSourceFile"
						:on-remove="removeUploadSourceFile"
						:max="1"
					>
						<n-upload-dragger>
							<div style="margin-bottom: 12px">
								<n-icon size="large" :depth="3">
									<submit-icon />
								</n-icon>
								<n-text style="font-size: 16px">
									点击或者拖动文件到该区域来上传
								</n-text>
								<n-p depth="3" style="margin: 8px 0 0 0">
									请仔细阅读上方的安全警告和注意事项。
								</n-p>
							</div>
						</n-upload-dragger>
					</n-upload>
					<transition name="fade">
						<n-alert
							v-if="state.fileToUpload != null"
							title="就绪"
							type="success"
							closable
						>
							你已经准备好了上传文件。请点击上方的上传按钮来上传你的文件。
						</n-alert>
					</transition>
				</n-space>
			</n-tab-pane>
			<n-tab-pane name="commits" tab="提交">
				<n-spin :show="state.loading">
					<commit-list
						:default-filter="defaultFilter"
						:commit-list-data="state.commitListData"
						:filter="state.filter"
						:filter-enabled="true"
						@updateData="fetchCommitListData"
					/>
				</n-spin>
			</n-tab-pane>
		</n-tabs>
	</div>
</template>

<style scoped>
.problem-commit-tabs-container {
	padding: 24px;
	height: 100vh;
}

.monaco-editor {
	height: 100%;
	width: 100%;
	resize: both;
	overflow: auto;
	align-items: center;
}
</style>
