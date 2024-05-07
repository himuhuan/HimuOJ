<script lang="ts" setup>

import {NIcon, NScrollbar, NSpin, NSplit, NText, useThemeVars} from "naive-ui";
import {onMounted, onUpdated, reactive} from "vue";
import {SwapHorizontal as SwapHorizontalIcon} from '@vicons/ionicons5';
import ProblemDetail from "@/models/ProblemDetail";
import {ProblemsServices} from "@/services/ProblemsServices";
import router from "@/routers";
import ProblemDetailPreview from "@/components/problems/ProblemDetailPreview.vue";
import ProblemCommitTabs from "@/components/problems/ProblemCommitTabs.vue";

/////////////////////////// variable //////////////////////////
const props = defineProps({
    contestCode: {
        type: String,
        required: false,
    },
    problemCode: {
        type: String,
        required: false,
    },
    problemId: {
        type: String,
        required: false,
    },
});

const state = reactive({
    loading: true,
    detail: null as ProblemDetail | null,
    problemId: "",
});

const themeVars = useThemeVars();

onMounted(async () => {
    state.loading = true;
    let error = false;
    if (props.problemId) {
        state.problemId = props.problemId;
        state.detail = await ProblemsServices.getProblemDetailById(state.problemId);
    } else {
        if (props.problemCode && props.contestCode) {
            console.log(props.contestCode, props.problemCode);
            const problemId = await ProblemsServices.getProblemId(
                props.contestCode,
                props.problemCode
            );
            if (problemId) {
                state.problemId = problemId;
                state.detail = await ProblemsServices.getProblemDetailById(problemId);
            } else {
                error = true;
            }
        }
    }
    if (error) {
        window.$message.error("非法访问");
        await router.push({name: "not-found"});
    } else {
        state.loading = false;
    }
});

onUpdated(() => {
    if (state.detail) {
        document.title = state.detail.title;
        const el = document.querySelector(".problem-view-container .n-split-pane-1");
        el?.setAttribute('style', 'width: 45%; overflow: auto;');
    }
});

</script>

<template>
    <transition name="fade">
        <div class="spining-container" v-if="state.loading">
            <n-spin></n-spin>
            <n-text> Loading...</n-text>
        </div>
    </transition>
    <div v-if="!state.loading" class="problem-view-container">
        <n-split
            :resize-trigger-size="12"
            direction="horizontal"
            class="problem-view-container">
            <template #1>
                <n-scrollbar style="height: calc(100vh - 45px)">
                    <problem-detail-preview :detail="state.detail!"/>
                </n-scrollbar>
            </template>
            <template #2>
                <n-scrollbar style="height: calc(100vh - 45px)">
                <problem-commit-tabs  :problem-id="state.problemId"/>
                </n-scrollbar>
            </template>
            <template #resize-trigger>
                <div class="resize-trigger">
                    <n-icon :color="themeVars.textColor1" :size="12">
                        <swap-horizontal-icon/>
                    </n-icon>
                </div>
            </template>
        </n-split>
    </div>
</template>

<style scoped>

.spining-container {
    width: 100%;
    height: 100%;
    background: v-bind("themeVars.bodyColor");
    position: fixed;
    top: 0;
    left: 0;
    z-index: 100;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;
}

.problem-view-container {
    background-color: v-bind("themeVars.bodyColor");
    height: calc(100vh - 45px);
}

.resize-trigger {
    border: 1px solid v-bind("themeVars.borderColor");
    height: 100%;
    background-color: v-bind("themeVars.tableColorHover");
    display: flex;
    justify-content: center;
    align-items: center;
}

.resize-trigger:hover {
    background-color: v-bind("themeVars.tableColor");
    transition: background-color 0.3s;
}

</style>