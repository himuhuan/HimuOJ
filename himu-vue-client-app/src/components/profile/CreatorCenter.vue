<script lang="ts" setup>
////////////////////////////// imports //////////////////////////////
import {UserDetailInfo} from "@/models/User";
import {UserServices} from "@/services/UserServices.ts";
import {onMounted, PropType, reactive} from "vue";
import {NAvatar, NCard, NIcon, NList, NListItem, NThing, useThemeVars} from "naive-ui";
import {
    NotebookQuestionMark24Regular as ProblemManageIcon,
    ChevronRight12Filled as RightArrowIcon,
} from "@vicons/fluent";
////////////////////////////// props //////////////////////////////
const props = defineProps({
    userInfo: {
        type: Object as PropType<UserDetailInfo>,
        required: true,
    },
});

////////////////////////////// state //////////////////////////////
const state = reactive({
    hasPermissionToPublishProblem: false,
});

const themeVars = useThemeVars();

onMounted(() => {
    state.hasPermissionToPublishProblem = UserServices.hasContestDistributorPermission(
        props.userInfo.permission
    );
});

</script>

<template>
    <n-card title="创作者中心" style="margin: 5px; width: auto; min-height: calc(100vh - 50px)">
        <n-list hoverable clickable style="margin: 5px auto">
            <n-list-item>
                <n-thing title="题目" content-style="margin-top: 10px;">
                    <template #avatar>
                        <n-avatar :color="themeVars.bodyColor">
                            <n-icon :color="themeVars.textColor1">
                                <ProblemManageIcon/>
                            </n-icon>
                        </n-avatar>
                    </template>
                    <template #description>
                        <n-text> 创建问题并发布到指定测试。管理问题测试点。</n-text>
                    </template>
                </n-thing>
                <template #suffix>
                    <n-icon>
                        <RightArrowIcon/>
                    </n-icon>
                </template>
            </n-list-item>
        </n-list>
    </n-card>
</template>

<style scoped>
.card-cover {
    width: 100%;
    height: 250px;
    object-fit: cover;
}
</style>
