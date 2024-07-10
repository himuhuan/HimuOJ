<script lang="ts" setup>
////////////////////////////// imports //////////////////////////////
import {UserDetailInfo} from "@/models/User";
import {UserServices} from "@/services/UserServices.ts";
import {onMounted, PropType, reactive} from "vue";
import {NCard, NAlert, NInput, NList, NListItem, NThing, NTag, NSpace, NButton} from "naive-ui";

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

onMounted(() => {
    state.hasPermissionToPublishProblem = UserServices.hasContestDistributorPermission(
        props.userInfo.permission
    );
});

</script>

<template>
    <n-card title="问题创建，编辑与管理" style="margin: 5px; width: auto; min-height: calc(100vh - 50px)">
        <n-alert type="error" v-if="!state.hasPermissionToPublishProblem" title="需要申请权限">
            您目前没有取得对任何测试发布的权限，请联系网站或测试的管理员申请权限。
        </n-alert>
        <n-card style="margin: 5px auto;" v-else>
            <n-space style="margin: 5px auto;">
                <n-input placeholder="搜索问题" />
                <n-space>
                    <n-button type="primary">搜索</n-button>
                    <router-link to="creator/problem_guide">
                        <n-button>创建问题</n-button>
                    </router-link>
                </n-space>
            </n-space>
            <n-alert type="info" style="margin: 5px auto" title="问题管理">
                您目前还没有发布任何问题，点击右上角的按钮开始创建问题。
            </n-alert>
            <n-list hoverable clickable style="margin: 5px auto">
                <n-list-item>
                    <n-thing title="题目" content-style="margin-top: 10px;">
                        <template #description>
                            <n-space size="small" style="margin-top: 4px">
                                <n-tag :bordered="false" type="primary" size="small">
                                    暑夜
                                </n-tag>
                                <n-tag :bordered="false" type="primary" size="small">
                                    晚春
                                </n-tag>
                            </n-space>
                        </template>
                    </n-thing>
                </n-list-item>
            </n-list>
        </n-card>
    </n-card>
</template>

<style scoped>
.card-cover {
    width: 100%;
    height: 250px;
    object-fit: cover;
}
</style>
