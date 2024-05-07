<script setup lang="ts">

import {onMounted, reactive, toRaw} from "vue";
import * as monaco from "monaco-editor";
import {
    NAlert,
    NButton,
    NCard,
    NIcon,
    NP,
    NSelect,
    NSpace,
    NTabPane,
    NTabs,
    NText,
    NUpload,
    NUploadDragger,
    SelectOption,
    UploadCustomRequestOptions,
    useThemeVars,
} from "naive-ui";

import {CodeTemplateInstance} from "@/utils/CodeTemplate.ts";
import {CommitsServices} from "@/services/CommitsServices.ts";
import {useUserState} from "@/services/UserStateServices.ts";
import {
    SaveOutlined as SaveCodeIcon,
    UploadFileOutlined as SubmitIcon,
    WarningRound as WarningIcon
} from "@vicons/material";
import CommitList from "@/components/commits/CommitList.vue";

const languageOptions: SelectOption[] = [
    {
        label: "C++",
        value: "cpp",
    },
    {
        label: "C",
        value: "c",
    },
    {
        label: "Java",
        value: "java",
    },
    {
        label: "Python",
        value: "python",
    },
];

const props = defineProps({
    problemId: {
        type: String,
        required: true,
    },
});

const state = reactive({
    currentTab: 'code-edit',
    editor: null as monaco.editor.IStandaloneCodeEditor | null,
    fileToUpload: null as File | null,
    selectedLanguage: languageOptions[0],
});

const themeVars = useThemeVars();
const userState = useUserState();

////////////////////////// function //////////////////////////

function changeTabs(tab: string | number) {
    state.currentTab = tab as string;
}

function handleHandleLanguageChange(value: string, option: SelectOption) {
    const editorRaw = toRaw(state.editor)!;
    const model = editorRaw.getModel();
    state.selectedLanguage = option;
    monaco.editor.setModelLanguage(model!, value);
    editorRaw.setValue(CodeTemplateInstance.getTemplate(value));
}

async function uploadSourceFile(options: UploadCustomRequestOptions) {
    if (options.file.file) {
        state.fileToUpload = options.file.file;
        const resp = await CommitsServices.postSourceFile(
            props.problemId,
            options.file.file,
            state.selectedLanguage.value as string
        );
        console.log(`commit ${resp} has submitted`);
    }
}

function removeUploadSourceFile() {
    state.fileToUpload = null;
}

async function handleSubmitCode() {
    if (state.currentTab === "code-edit") {
        const editorRaw = toRaw(state.editor)!;
        const model = editorRaw.getModel();
        const code = model!.getValue();
        await CommitsServices.postSourceCode(
            props.problemId,
            code,
            state.selectedLanguage.value as string
        );
    }
    state.currentTab = "commits";
}

onMounted(async () => {
    state.editor = monaco.editor.create(
        document.getElementById("user-editor")!,
        {
            value: CodeTemplateInstance.getTemplate("cpp"),
            language: "cpp",
            fontSize: 14,
            fontLigatures: true,
            automaticLayout: true,
            lineHeight: 1.5,
            fontFamily: themeVars.value.fontFamilyMono,
            lineNumbers: "on",
            roundedSelection: false,
            scrollBeyondLastLine: false,
            readOnly: false,
            theme: userState.perferTheme === "dark" ? "vs-dark" : "vs",
        }
    );
})

</script>

<template>
    <div class="problem-commit-tabs-container">
        <n-tabs
            default-value="code-edit"
            :on-update:value="changeTabs"
            :value="state.currentTab"
            animated
        >
            <template #suffix>
                <n-space>
                    <n-select
                        filterable
                        default-value="C++"
                        placeholder="选择语言..."
                        :options="languageOptions"
                        style="width: 200px"
                        :on-update:value="handleHandleLanguageChange"
                    ></n-select>
                    <n-button
                        type="primary"
                        :disabled="state.currentTab == 'commits'"
                        @click="handleSubmitCode"
                    >
                        <n-icon size="medium" :component="SubmitIcon"/> &nbsp; 提交
                    </n-button>
                    <n-button
                        type="primary"
                        secondary
                        :disabled="state.currentTab != 'code-edit'"
                    >
                        <n-icon size="medium" :component="SaveCodeIcon"/> &nbsp; 保存
                    </n-button>
                </n-space>
            </template>
            <n-tab-pane
                name="code-edit"
                tab="代码"
                display-directive="show"
                style="height: 90vh; width: 100%"
            >
                <n-card bordered class="monaco-editor" id="user-editor"></n-card>
            </n-tab-pane>
            <n-tab-pane name="source" tab="源文件">
                <n-space vertical>
                    <n-alert title="安全警告" type="error" closable>
                        <template #icon>
                            <n-icon size="larger" :component="WarningIcon"/>
                        </template>
                        请不要试图上传一切可能危害服务器的代码或者文件。<br/>
                        否则，一旦被发现，你的账号将会被永久封禁。
                    </n-alert>
                    <n-alert title="注意" type="info" closable>
                        请注意，你上传的文件必须与你选择的语言(
                        {{ state.selectedLanguage?.label }} )相匹配，否则将会导致编译错误。
                    </n-alert>
                    <n-upload
                        :custom-request="uploadSourceFile"
                        :on-remove="removeUploadSourceFile"
                        :max="1"
                    >
                        <n-upload-dragger>
                            <div style="margin-bottom: 12px">
                                <n-icon size="large" :depth="3">
                                    <submit-icon/>
                                </n-icon>
                                <n-text style="font-size: 16px">
                                    点击或者拖动文件到该区域来上传
                                </n-text>
                                <n-p depth="3" style="margin: 8px 0 0 0">
                                    请仔细阅读上方的安全警告和注意事项。
                                </n-p>
                            </div>
                        </n-upload-dragger>
                    </n-upload>
                    <transition name="fade">
                        <n-alert
                            v-if="state.fileToUpload != null"
                            title="就绪"
                            type="success"
                            closable
                        >
                            你已经准备好了上传文件。请点击上方的上传按钮来上传你的文件。
                        </n-alert>
                    </transition>
                </n-space>
            </n-tab-pane>
            <n-tab-pane name="commits" tab="提交">
                <n-space vertical>
                    <n-alert title="请不要盯着表格发呆" type="info">
                        由于流量和服务器性能限制, 本页面的提交列表不会实时更新,
                        请点击刷新按钮获取最新的提交列表. (也请不要过多点击)
                        <br/>
                        <p class="italic-font">嘿，也许是开发太懒了呢？...</p>
                    </n-alert>
                    <commit-list :user-id="userState.id.toString()"
                                 :problem-id="props.problemId.toString()"
                                 :filter-enabled="false"/>
                </n-space>
            </n-tab-pane>
        </n-tabs>
    </div>
</template>

<style scoped>
.problem-commit-tabs-container {
    padding: 24px;
    height: 100vh;
}

.monaco-editor {
    height: 100%;
    width: 100%;
    resize: both;
    overflow: auto;
    align-items: center;
}
</style>