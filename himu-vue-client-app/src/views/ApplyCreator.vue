<script setup lang="ts">
////////////////////////////// imports //////////////////////////////

import {NAlert, NButton, NCard, NGi, NGrid, NIcon, NText, useLoadingBar, useThemeVars,} from "naive-ui";

import {CheckRound} from "@vicons/material";
import {AuthorizationServices, UserRole,} from "@/services/AuthorizationServices";
import {UserServices} from "@/services/UserServices.ts";
import {useUserState} from "@/services/UserStateServices";
import {onMounted, reactive} from "vue";

///////////////////////////// Global Variables //////////////////////'
const themeVars = useThemeVars();
const loadingBar = useLoadingBar();
const {user} = useUserState();

const state = reactive({
    hasPermission: false,
});

///////////////////////////// functions /////////////////////////////

function handleApplyButton() {
    loadingBar.start();
    AuthorizationServices.applyPermission(
        user!.id,
        UserRole.ContestDistributor
    ).then((ok) => {
        if (ok) {
            loadingBar.finish();
            window.location.reload();
        } else {
            loadingBar.error();
        }
    });
}

/////////////////////////// setup ////////////////////////////

onMounted(async () => {
    if (await UserServices.hasContestDistributorPermissionById(user!.id)) {
        state.hasPermission = true;
    }
});

</script>

<template>
    <img
        src="@/assets/images/himu-creator-plan.jpg"
        alt="background"
        style="
			width: 100%;
			height: 100%;
			object-fit: cover;
			position: absolute;
			top: 0;
			left: 0;
			z-index: -1;
		"
    />
    <n-card style="width: 100%; min-height: 100vh">
        <div style="align-items: center; text-align: center">
            <p style="font-size: 60px">
                <n-text type="primary"> 在 HimuOJ 免费发布题目/测试！</n-text>
            </p>
            <p style="font-size: 30px">
                <n-text> 发布题目供所有人/特定人员测试</n-text>
                <br/>
                <n-text> 完全免费。</n-text>
            </p>
            <n-button
                type="primary"
                style="padding: 0 40px"
                v-if="!state.hasPermission"
                @click="handleApplyButton"
            >
                申请
            </n-button>
            <n-alert type="success" style="width: 50%; margin: 0 auto" v-else
                     title="您无需再申请权限">
                您已经拥有发布题目的权限。
            </n-alert>
        </div>
        <div class="apply-introduction">
            <n-grid cols="1 m:5" item-responsive responsive="screen">
                <n-gi span="1" offset="0 m:1" class="check-list">
                    <h3>你将获得的权限：</h3>
                    <li>
                        <n-icon :color="themeVars.primaryColor">
                            <check-round/>
                        </n-icon>
                        发布或更改测试以及问题
                    </li>
                    <li>
                        <n-icon :color="themeVars.primaryColor">
                            <check-round/>
                        </n-icon>
                        查看测试的所有提交
                    </li>
                    <li>
                        <n-icon :color="themeVars.primaryColor">
                            <check-round/>
                        </n-icon>
                        手动/自动地查看并运行问题下的所有提交
                    </li>
                </n-gi>
                <n-gi span="1" offset="0 m:1" class="check-list">
                    <h3>申请要求：</h3>
                    <li>
                        <n-icon :color="themeVars.primaryColor">
                            <check-round/>
                        </n-icon>
                        没有要求，目前！
                    </li>
                </n-gi>
            </n-grid>
        </div>
    </n-card>
</template>

<style scoped>
.apply-introduction {
    margin: auto;
    padding: 20px;
    font-size: 16px;
    font-family: v-mono, v-sans, other-fallbacks, sans-serif;
}

.check-list li {
    list-style-type: none;
}
</style>
