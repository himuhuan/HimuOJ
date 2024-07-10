<script setup lang="ts">
import type {MenuOption} from "naive-ui";
// ui
import {NAvatar, NIcon, NLayout, NLayoutContent, NLayoutSider, NMenu, NThing,} from "naive-ui";
// services & models
import {UserDetailInfo} from "@/models/User";
// vue
import {Component, h, PropType} from "vue";
// icons
import {
    ChecklistRound as ListIcon,
    CommitRound as CommitIcon,
    FavoriteBorderRound as FavoriteIcon,
    HomeRound as HomeIcon,
} from "@vicons/material";
import {UserFriends as UserFriendIcon} from "@vicons/fa";
import {CreateOutline as CreatorIcon} from "@vicons/ionicons5";
import {usedVariables} from "@/utils/HimuTools.ts";

////////////////////////////// functions //////////////////////////////

function renderIcon(icon: Component) {
    return () => h(NIcon, null, {default: () => h(icon)});
}

function onUpdateMenuSelection(key: string, item: MenuOption) {
    usedVariables(item);
    emit("menuUpdate", {key});
}

////////////////////////////// variables //////////////////////////////

const emit = defineEmits(["menuUpdate"]);

const props = defineProps({
    userInfo: {
        type: Object as PropType<UserDetailInfo>,
        required: true,
    },
    value: {
        type: String,
        required: false,
    },
});

const menuOptions: MenuOption[] = [
    {
        label: () => "个人信息",
        key: "info",
        icon: renderIcon(HomeIcon),
    },
    {
        label: () => "提交记录",
        key: "commits",
        icon: renderIcon(CommitIcon),
    },
    {
        label: () => "我的题单",
        key: "problem-list",
        icon: renderIcon(ListIcon),
    },
    {
        label: () => "好友",
        key: "friends",
        icon: renderIcon(UserFriendIcon),
    },
    {
        label: () => "创建 & 发布",
        key: "creator-dashboard",
        icon: renderIcon(CreatorIcon),
    },
    {
        label: () => "收藏",
        key: "collection",
        icon: renderIcon(FavoriteIcon),
    },
];
</script>

<template>
    <n-layout
        has-sider
        sider-placement="left"
        style="background: transparent; height: calc(100vh - 50px);
        overflow: hidden; position: relative;"
    >
        <n-layout-sider
            bordered
            content-style="padding: 12px;"
            style="position: sticky;"
        >
            <n-thing>
                <template #avatar>
                    <n-avatar
                        size="large"
                        :src="props.userInfo.avatarUri"
                        :style="{
							cursor: 'pointer',
							margin: '0 auto',
						}"
                    ></n-avatar>
                </template>
                <template #header>
                    {{ props.userInfo.userName }}
                </template>
                <template #description>
                    {{ props.userInfo.email }}
                </template>
            </n-thing>
            <n-menu
                :options="menuOptions"
                :indent="16"
                :value="props.value"
                default-value="info"
                :on-update:value="onUpdateMenuSelection"
            ></n-menu>
        </n-layout-sider>
        <n-layout-content style="background-color: transparent" :native-scrollbar="false">
            <slot></slot>
        </n-layout-content>
    </n-layout>
</template>
