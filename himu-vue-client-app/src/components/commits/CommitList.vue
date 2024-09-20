<script setup lang="ts">
///////////////////////// imports /////////////////////////
import CommitListFilter from "@/models/CommitListFilter.ts";
import { h, reactive, ref, onMounted, PropType } from "vue";
import {
	DataTableColumn,
	NAlert,
	NButton,
	NDataTable,
	NDatePicker,
	NForm,
	NFormItem,
	NIcon,
	NInput,
	NModal,
	NSelect,
	NSpace,
	NTag,
	PaginationInfo,
	SelectOption,
} from "naive-ui";
import { ExecutionStatus } from "@/models/ResultModels.ts";
import { HimuCommitList } from "@/models/HimuCommitList.ts";
import StatusTag from "@/components/StatusTag.vue";
import { usedVariables } from "@/utils/HimuTools.ts";
import {
	AutorenewRound as ResetFilterIcon,
	FilterAltRound,
	RefreshRound,
} from "@vicons/material";
import CommitListInfo from "@/models/CommitListInfo.ts";
import router from "@/routers";

///////////////////////// variables /////////////////////////

const props = defineProps({
	defaultFilter: {
		type: Object as PropType<CommitListFilter>,
		required: true,
	},
    filter: {
        type: Object as PropType<CommitListFilter>,
        required: true,
    },
    commitListData: {
        type: Object as PropType<HimuCommitList | null>,
        required: true,
    },
    filterEnabled: {
        type: Boolean,
        default: false
    }
});
//
// emits
//
const emit = defineEmits<{
	(e: "updateData", filter: CommitListFilter): void;
}>();

const commitStatusOptions: SelectOption[] = [
	{ label: "全部状态", value: "" },
	{ label: "通过", value: ExecutionStatus.ACCEPTED },
	{ label: "内存超限", value: ExecutionStatus.MEMORY_LIMIT_EXCEEDED },
	{ label: "时间超限", value: ExecutionStatus.TIME_LIMIT_EXCEEDED },
	{ label: "运行时错误", value: ExecutionStatus.RUNTIME_ERROR },
	{ label: "答案错误", value: ExecutionStatus.WRONG_ANSWER },
	{ label: "编译错误", value: ExecutionStatus.COMPILE_ERROR },
	{ label: "运行中", value: ExecutionStatus.RUNNING },
	{ label: "等待中", value: ExecutionStatus.PENDING },
];

const languageSelectOptions: SelectOption[] = [
	{ label: "g++", value: "g++" },
	{ label: "Java", value: "Java" },
	{ label: "Python", value: "Python" },
	{ label: "JavaScript", value: "JavaScript" },
	{ label: "TypeScript", value: "TypeScript" },
	{ label: "C#", value: "C#" },
];

const tableColumns = ref<DataTableColumn[]>([
	{
		key: "commitId",
		title: "提交编号",
	},
	{
		key: "problemName",
		title: "题目名称",
	},
	{
		key: "commitStatus",
		title: "结果",
		render(row) {
			return h(StatusTag, { status: String(row.commitStatus) });
		},
	},
	{
		key: "compilerName",
		title: "语言",
	},
	{
		key: "commitDate",
		title: "提交时间",
	},
]);

///////////////////////// states /////////////////////////
const state = reactive({
	showFilterModal: false,
	filter: {
		...props.filter,
	} as CommitListFilter,
	usingFilter: false,
	pagination: {
		page: 1,
		pageCount: 0,
		pageSize: 1,
		itemCount: 0,
		prefix: paginationPrefix,
	},
});

///////////////////////// functions /////////////////////////

function resetFilter() {
	state.filter = {...props.defaultFilter};
	emit("updateData", state.filter);
	state.usingFilter = false;
}

function paginationPrefix(info: PaginationInfo) {
	usedVariables(info);
	return h(NSpace, null, {
		default: () => [
			h(
				NButton,
				{
					disabled: !props.filterEnabled,
					renderIcon: () =>
						h(NIcon, null, { default: () => h(ResetFilterIcon) }),
					onClick: resetFilter,
				},
				{
					default: () => "重置筛选",
				}
			),
			h(
				NButton,
				{
					disabled: !props.filterEnabled,
					type: "primary",
					secondary: true,
					renderIcon: () =>
						h(NIcon, null, { default: () => h(FilterAltRound) }),
					onClick: () => {
						state.showFilterModal = true;
					},
				},
				{
					default: () => "筛选",
				}
			),
			h(
				NButton,
				{
					type: "primary",
					renderIcon: () => h(NIcon, null, { default: () => h(RefreshRound) }),
					onClick: handleRefreshRequest,
				},
				{
					default: () => "刷新",
				}
			),
		],
	});
}

function handleRefreshRequest() {
	emit("updateData", state.filter);
}

function handleRowProps(rowData: CommitListInfo, rowIndex: number) {
	usedVariables(rowIndex);
	return {
		style: {
			cursor: "pointer",
		},
		onclick: () => {
			if (rowData.commitStatus === ExecutionStatus.RUNNING) {
				window.$message.info("由于 HimuOJ 正在运行您的提交，暂时无法查看详情");
				return;				
			}
			router.push(`/commits/${rowData.commitId}`);
		},
	};
}

function handlePageChange(currentPage: number) {
	state.filter.page = currentPage;
	state.pagination.page = currentPage;
    console.log("filter", state.filter);
	emit("updateData", state.filter);
}

async function handleFilterOk() {
	state.showFilterModal = false;
	if (state.filter !== props.filter) {
		state.usingFilter = true;
		emit("updateData", state.filter);
	} else {
		state.usingFilter = false;
	}
}

onMounted(() => {
	state.pagination.page = props.filter.page;
	state.pagination.itemCount = props.commitListData?.totalCount ?? 0;
	state.pagination.pageSize = props.filter.pageSize;
});
</script>

<template>
	<n-space vertical>
		<n-alert
			type="warning"
			v-show="state.usingFilter"
			show-icon
			closable
			title="注意"
		>
			您正在使用筛选功能，数据可能并不完整
		</n-alert>
		<n-data-table
			remote
			striped
			:data="props.commitListData?.data ?? []"
			:pagination="state.pagination"
			:columns="tableColumns"
			:row-key="
				(row) => {
					return row.commitId + row.commitStatus;
				}
			"
			:row-props="handleRowProps"
			:on-update:page="handlePageChange"
		></n-data-table>
	</n-space>

	<n-modal
		style="width: 60%"
		v-model:show="state.showFilterModal"
		preset="card"
		title="筛选提交"
		size="huge"
	>
		<n-form :model="state.filter" label-placement="left" label-width="auto">
			<n-form-item label="题目名称" path="problemName">
				<n-input
					v-model:value="state.filter.problemName"
					placeholder="全部题目"
				/>
			</n-form-item>
			<n-form-item label="提交状态" path="commitStatus">
				<n-select
					v-model:value="state.filter.commitStatus"
					:options="commitStatusOptions"
					placeholder="全部状态"
				>
				</n-select>
			</n-form-item>
			<n-form-item label="语言" path="language">
				<n-select
					:options="languageSelectOptions"
					v-model:value="state.filter.language"
					placeholder="全部语言"
				/>
			</n-form-item>
			<n-form-item label="提交于此时间之前" path="commitDateEnd">
				<n-date-picker
					v-model:value="state.filter.commitDateEnd"
					placeholder="选择日期范围"
				/>
			</n-form-item>
		</n-form>
		<!--suppress VueUnrecognizedSlot -->
		<template #action>
			<n-space style="justify-content: flex-end">
				<n-button type="primary" @click="handleFilterOk"> 确定</n-button>
				<n-button @click="() => (state.showFilterModal = false)">
					取消</n-button
				>
			</n-space>
		</template>
	</n-modal>
</template>
