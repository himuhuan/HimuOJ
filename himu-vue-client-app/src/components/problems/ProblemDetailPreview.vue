<script lang="ts" setup>

import {PropType} from 'vue';
import ProblemDetail from "@/models/ProblemDetail.ts";

// ui
import {NCard, NH2, NSpace, NTag, NText} from "naive-ui";
import {MdPreview} from 'md-editor-v3';
import 'md-editor-v3/lib/preview.css';

/////////////////// Props ///////////////////
const props = defineProps({
    detail: {
        type: Object as PropType<ProblemDetail>,
        required: true,
    },
});

const id = 'problem-detail-preview';

</script>

<template>
        <n-card bordered>
            <n-space vertical size="large">
                <n-h2>
                    <n-text type="primary"> {{ props.detail?.title }}</n-text>
                </n-h2>
                <n-space>
                    <n-tag type="info">
                        内存限制: {{ props.detail?.maxMemoryLimitByte! / 1000 / 1000 }}MB
                    </n-tag>
                    <n-tag type="warning">
                        时间限制: {{ props.detail?.maxExecuteTimeLimit }}ms
                    </n-tag>
                </n-space>
                <MdPreview :editor-id="id" preview-theme="vuepress"
                           :model-value="props.detail.content"/>
            </n-space>
        </n-card>
</template>