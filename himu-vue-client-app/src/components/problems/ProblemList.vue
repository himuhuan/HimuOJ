<script setup lang="ts">
import ProblemList from "@/models/ProblemList.ts";
import {onMounted, reactive, ref} from "vue";
import {DataTableColumn, NAlert, NCard, NDataTable} from "naive-ui";
import {usedVariables} from "@/utils/HimuTools.ts";
import router from "@/routers";
import ProblemListItem from "@/models/ProblemListItem.ts";
import {ProblemsServices} from "@/services/ProblemsServices.ts";
import ProblemListFilter from "@/models/ProblemListFilter.ts";


////////////////////////// variable //////////////////////////

interface ProblemListProps extends ProblemListFilter {
    isDefaultSet: boolean;
    showProblemReviewStatus: boolean;
}

const props = withDefaults(defineProps<ProblemListProps>(), {
    isDefaultSet: false,
    showProblemReviewStatus: false,
});

const state = reactive({
    loading: true,
    data: undefined as ProblemList | undefined,
});

const pagination = ref({
    page: 1,
    pageCount: 1,
    pageSize: 10,
    itemCount: 0,
});

const tableColumns: DataTableColumn[] = [
    {
        key: "problemTitle",
        title: "题目"
    },
    {
        key: "contestTitle",
        title: "所属题目集/比赛"
    },
    {
        key: "totalCommitCount",
        title: "总提交数"
    },
    {
        key: "acceptedCount",
        title: "通过数"
    },
    {
        key: "acceptedRate",
        title: "通过率"
    }
];

////////////////////////// function //////////////////////////

async function fetchProblemList(page: number, pageSize: number = 10) {
    state.loading = true;
    let data: ProblemList | null = null;
    if (props.isDefaultSet) {
        data = await ProblemsServices.getDefaultProblemList(page, pageSize);
    } else {
        data = await ProblemsServices.getProblemList(page, pageSize, props);
    }
    if (!data) {
        window.$message.error("获取题目列表失败");
    } else {
        state.data = data;
        pagination.value = {
            page: page,
            pageCount: data.pageCount,
            pageSize: pageSize,
            itemCount: data.totalCount
        };
    }
    state.loading = false;
}

function handleRowProps(rowData: ProblemListItem, rowIndex: number) {
    usedVariables(rowIndex);
    return {
        style: {
            cursor: "pointer",
        },
        onclick: () => {
            router.push(`/contests/${rowData.contestCode}/problems/${rowData.problemCode}`);
        },
    };
}

function handlePageChange(currentPage: number) {
    fetchProblemList(currentPage, pagination.value.pageSize);
}

////////////////////////// lifecycle //////////////////////////

onMounted(async () => {
    await fetchProblemList(1);
});

</script>

<template>
    <n-card style="margin: 5px 0">
        <n-alert
            type="info"
            show-icon
            closable
            v-if="props.isDefaultSet"
            title="注意"
            style="margin: 5px 0"
        >
            您正在查看 HimuOJ 默认题目集的题目列表, 部分由第三方提供的题目不会显示在此列表中。
        </n-alert>
        <n-data-table
            remote
            :data="state.data?.problems"
            :pagination="pagination"
            :columns="tableColumns"
            :loading="state.loading"
            :row-props="handleRowProps"
            :row-key="row => row.problemCode"
            :on-update:page="handlePageChange"
        ></n-data-table>
    </n-card>
</template>

<style scoped>

</style>