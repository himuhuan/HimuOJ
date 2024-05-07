<script setup lang="ts">
///////////////////////// imports /////////////////////////
import CommitListFilter from "@/models/CommitListFilter.ts";
import {h, onMounted, reactive, ref} from "vue";
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
    SelectOption
} from "naive-ui";
import {ExecutionStatus} from "@/models/ResultModels.ts";
import {UserCommitList} from "@/models/UserCommitList.ts";
import {usedVariables} from "@/utils/HimuTools.ts";
import {AutorenewRound as ResetFilterIcon, FilterAltRound, RefreshRound} from "@vicons/material";
import {CommitsServices} from "@/services/CommitsServices.ts";
import CommitListInfo from "@/models/CommitListInfo.ts";
import router from "@/routers";

///////////////////////// variables /////////////////////////

interface CommitListFilterProps extends CommitListFilter {
    filterEnabled: boolean;
}

const props = withDefaults(defineProps<CommitListFilterProps>(), {
    commitDateEnd: Date.now(),
    commitStatus: '',
    language: '',
    problemId: '',
    problemName: '',
    userId: '',
    filterEnabled: true,
});

const commitStatusOptions: SelectOption[] = [
    {label: "全部状态", value: ""},
    {label: "通过", value: ExecutionStatus.ACCEPTED},
    {label: "内存超限", value: ExecutionStatus.MEMORY_LIMIT_EXCEEDED},
    {label: "时间超限", value: ExecutionStatus.TIME_LIMIT_EXCEEDED},
    {label: "运行时错误", value: ExecutionStatus.RUNTIME_ERROR},
    {label: "答案错误", value: ExecutionStatus.WRONG_ANSWER},
    {label: "编译错误", value: ExecutionStatus.COMPILE_ERROR},
    {label: "运行中", value: ExecutionStatus.RUNNING},
    {label: "等待中", value: ExecutionStatus.PENDING},
];

const languageSelectOptions: SelectOption[] = [
    {label: "g++", value: "g++"},
    {label: "Java", value: "Java"},
    {label: "Python", value: "Python"},
    {label: "JavaScript", value: "JavaScript"},
    {label: "TypeScript", value: "TypeScript"},
    {label: "C#", value: "C#"},
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
            switch (row.commitStatus) {
            case ExecutionStatus.ACCEPTED:
                return h(
                    NTag,
                    {
                        type: "success",
                    },
                    () => "通过"
                );
            case ExecutionStatus.MEMORY_LIMIT_EXCEEDED:
                return h(
                    NTag,
                    {
                        type: "error",
                    },
                    () => "内存超限"
                );
            case ExecutionStatus.TIME_LIMIT_EXCEEDED:
                return h(
                    NTag,
                    {
                        type: "error",
                    },
                    () => "时间超限"
                );
            case ExecutionStatus.RUNTIME_ERROR:
                return h(
                    NTag,
                    {
                        type: "error",
                    },
                    () => "运行时错误"
                );
            case ExecutionStatus.WRONG_ANSWER:
                return h(
                    NTag,
                    {
                        type: "error",
                    },
                    () => "答案错误"
                );
            case ExecutionStatus.COMPILE_ERROR:
                return h(
                    NTag,
                    {
                        type: "error",
                    },
                    () => "编译错误"
                );
            case ExecutionStatus.RUNNING:
                return h(
                    NTag,
                    {
                        type: "warning",
                    },
                    () => "运行中"
                );
            case ExecutionStatus.PENDING:
                return h(
                    NTag,
                    {
                        type: "default",
                    },
                    () => "等待中"
                );
            default:
                return h(
                    NTag,
                    {
                        type: "default",
                    },
                    () => row.commitStatus
                );
            }
        },
    },
    {
        key: "compilerName",
        title: "语言",
    },
]);

///////////////////////// states /////////////////////////
const state = reactive({
    loading: true,
    data: null as UserCommitList | null,
    showFilterModal: false,
    filter: {
        ...props
    } as CommitListFilter,
    usingFilter: false,
    pagination: {
        page: 1,
        pageCount: 1,
        pageSize: 10,
        itemCount: 0,
        prefix: paginationPrefix,
    },
});

///////////////////////// functions /////////////////////////

/**
 * Request commit list by page then update the data and pagination
 */
async function requestCommitList(
    page: number,
    pageSize: number,
    filter?: CommitListFilter
) {
    state.loading = true;
    state.data = await CommitsServices.filterCommitList(page, pageSize, filter);
    if (state.data) {
        state.pagination.pageCount = state.data.pageCount;
        state.pagination.itemCount = state.data.totalCount;
        state.pagination.page = page;
    } else {
        window.$message.error(`无法获取${page}页的提交列表`);
        console.error(`cannot get commit list of page ${page}`, filter);
    }
    state.loading = false;
}

function resetFilter() {
    state.filter = props;
    requestCommitList(1, state.pagination.pageSize, props);
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
                        h(NIcon, null, {default: () => h(ResetFilterIcon)}),
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
                        h(NIcon, null, {default: () => h(FilterAltRound)}),
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
                    renderIcon: () =>
                        h(NIcon, null, {default: () => h(RefreshRound)}),
                    onClick: handleRefreshRequest,
                },
                {
                    default: () => "刷新",
                }
            ),
        ],
    });
}

async function handleRefreshRequest() {
    await requestCommitList(
        state.pagination.page,
        state.pagination.pageSize,
        state.filter
    );
}

function handleRowProps(rowData: CommitListInfo, rowIndex: number) {
    usedVariables(rowIndex);
    return {
        style: {
            cursor: "pointer",
        },
        onclick: () => {
            router.push(`/commits/${rowData.commitId}`);
        },
    };
}

async function handlePageChange(currentPage: number) {
    await requestCommitList(currentPage, state.pagination.pageSize, state.filter);
}

async function handleFilterOk() {
    state.showFilterModal = false;
    if (state.filter !== props) {
        state.usingFilter = true;
        await requestCommitList(1, state.pagination.pageSize, state.filter);
    } else {
        state.usingFilter = false;
    }
}

//////////////////// lifecycle ////////////////////
onMounted(async () => {
    await requestCommitList(1, state.pagination.pageSize, props);
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
            :data="state.data?.data ?? []"
            :pagination="state.pagination"
            :columns="tableColumns"
            :loading="state.loading"
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
                <n-input v-model:value="state.filter.problemName" placeholder="全部题目"/>
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
                <n-button @click="() => (state.showFilterModal = false)"> 取消</n-button>
            </n-space>
        </template>
    </n-modal>
</template>