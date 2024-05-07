<template>
    <center-content-container width="90%">
        <transition name="slide-fade">
            <n-card
                v-if="state.showCreatorProgram"
                @close="state.showCreatorProgram = !state.showCreatorProgram"
                @click="() => router.push({ name: 'creator-apply' })"
                hoverable
                closable
                style="cursor: pointer"
                title="HimuOJ 创作者计划：您的创作对我们至关重要!"
            >
                <p>
                    <n-text>
                        HimuOJ 鼓励所有人在 HimuOJ
                        上解答问题，参加测试，发布题解，并邀请其他人来自由使用本平台的所有资源。
                        此外，HimuOJ
                        对于所有的创作者提供<b>免费</b>的服务器资源，用于发布测试，题解，以及其他的资源。
                        对于提供独创优秀的测试，题解，或者其他的资源的创作者，HimuOJ
                        会给予一定的奖励。
                    </n-text>
                </p>
                <p>
                    <n-text>
                        如果您有兴趣参与 HimuOJ 创作者计划，点击此处了解如何利用 HimuOJ API
                        发布测试或题目。
                    </n-text>
                    <br/>
                    <n-text>
                        注意：HimuOJ 严禁发布未经授权或存在侵权行为的题目, HimuOJ
                        不会对任何发布者的侵权行为做出任何保证.
                    </n-text>
                </p>

                <p>您可以关闭此标签，之后 HimuOJ 不会为您显示.</p>
            </n-card>
        </transition>
        <problem-list :is-default-set="true" />
    </center-content-container>
</template>

<script setup lang="ts">
import ProblemList from "@/components/problems/ProblemList.vue";
import {onMounted, reactive} from "vue";
import CenterContentContainer from "@/components/CenterContentContainer.vue";
import {NCard, NText} from "naive-ui";
import router from "@/routers";

//////////////////////// variables //////////////////////////
const state = reactive({
    showCreatorProgram: true,
});

//////////////////////// lifecycle //////////////////////////
onMounted(() => {
    document.title = "HimuOJ - 题目列表";
});

</script>
