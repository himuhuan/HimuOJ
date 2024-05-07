<script setup lang="ts">
import {onMounted, reactive, ref} from "vue";
import {
    FormInst,
    FormRules,
    NA,
    NAlert,
    NButton,
    NCard,
    NForm,
    NFormItem,
    NInput,
    NInputNumber,
    NResult,
    NSpace,
    NStep,
    NSteps,
    NSwitch,
    useLoadingBar,
} from "naive-ui";
import {MdEditor} from "md-editor-v3";
import 'md-editor-v3/lib/style.css'

// services
import {ContestsServices} from "@/services/ContestsServices.ts";
import {useUserState} from "@/services/UserStateServices.ts";
import router from "@/routers";
import {CodeTemplateInstance} from "@/utils/CodeTemplate.ts";
import {usedVariables} from "@/utils/HimuTools.ts";
import {ProblemLimits} from "@/models/ModelLimits.ts";
import ProblemDetailPreview from "@/components/problems/ProblemDetailPreview.vue";
import {ProblemDetail} from "@/models/ProblemDetail.ts";

const props = defineProps({
    // The contest code to publish
    contestCode: {
        type: String,
        required: true,
    },
});

const userState = useUserState();

const briefInfoFormRef = ref<FormInst | null>(null);
const limitFormRef = ref<FormInst | null>(null);

const state = reactive({
    contestId: undefined as string | undefined, // The current step
    currentStep: 1,
    currentStepName: "ProblemIntro", // step 2: title, code, brief info for the problem
    briefInfo: {
        showProblemCodeAlert: false
    },
    detail: {
        title: "",
        code: "",
        content: CodeTemplateInstance.getTemplate("_new_problem"),
        maxMemoryLimitByte: 125 * 1000 * 1000,
        maxExecuteTimeLimit: 1000,
        allowDownloadInput: false,
        allowDownloadAnswer: false
    },
    nextStepLoading: false,
    showResult: false,
});

const loadingBar = useLoadingBar();

/////////////////////// functions ////////////////////////

async function doBriefInfoValidation(): Promise<boolean>
{
    let res = !!(await briefInfoFormRef.value?.validate());
    if (!res)
    {
        state.briefInfo.showProblemCodeAlert = true;
        return false;
    }

    let ok = await ContestsServices.checkProblemCode(state.contestId!, state.detail.code);
    state.briefInfo.showProblemCodeAlert = !ok;
    return ok;
}

function doDetailValidation(): boolean
{
    return state.detail.content.length <= ProblemLimits.MaxContentLength;
}

async function doProblemLimitValidation(): Promise<boolean>
{
    return !!(await limitFormRef.value?.validate());
}

async function doPublishProblem(): Promise<boolean>
{
    state.nextStepLoading = true;
    loadingBar.start();
    let ok = await ContestsServices.postProblem(state.contestId!, state.detail);
    if (!ok)
    {
        loadingBar.error();
        window.$message.error("发布失败，请稍后重试");
    }
    else
    {
        loadingBar.finish();
        window.$message.success("发布成功");
    }
    state.showResult = ok;
    state.nextStepLoading = false;
    return ok;
}

async function handleNextStep(e: MouseEvent)
{
    e.preventDefault();
    let ok = true;
    if (!state.contestId) return;

    switch (state.currentStep)
    {
    case 2:
        ok = await doBriefInfoValidation();
        break;
    case 3:
        ok = doDetailValidation();
        break;
    case 4:
        ok = await doProblemLimitValidation();
        break;
    case 5:
        ok = await doPublishProblem();
        break;
    }

    if (ok)
    {
        if (state.currentStep <= 5)
        {
            state.currentStep += 1;
        }
    }
}

/////////////////////// validation ///////////////////////

const briefInfoRules: FormRules = {
    title: {
        required: true,
        message: "请输入题目标题",
        trigger: 'blur',
    },
    code: {
        required: true,
        validator: (rule, value) =>
        {
            usedVariables(rule);
            if (value === "")
            {
                return new Error("请输入题目代号");
            }
            if (value.match(/[^a-z0-9_-]/))
            {
                return new Error("题目代号只能包含小写字母、数字、下划线和短横线");
            }
            return true;
        },
        trigger: 'blur',
    },
};

const limitRules: FormRules = {
    maxMemoryLimitByte: {
        type: 'number',
        required: true,
        trigger: ['blur', 'change'],
        validator: (rule, value: number) =>
        {
            usedVariables(rule);
            if (value < 0) return new Error("内存限制必须大于 0");
            if (value >= ProblemLimits.MaxMemoryLimit) return new Error("内存不得高于 1GB");
            return true;
        }
    },
    maxExecuteTimeLimit: {
        type: 'number',
        required: true,
        trigger: ['blur', 'change'],
        validator: (rule, value: number) =>
        {
            usedVariables(rule);
            if (value < 0) return new Error("时间限制必须大于 0");
            if (value >= ProblemLimits.MaxTimeMSLimit) return new Error("时间不得高于 10s");
            return true;
        }
    }
};

onMounted(async () =>
{
    if (!props.contestCode
        || !(state.contestId =
            await ContestsServices.getContestId(props.contestCode))
        || !await ContestsServices.checkUserPermission(state.contestId, userState.id))
    {
        console.log("Illegal access");
        window.$message.error("非法访问");
        await router.push({name: "not-found"});
    }
    document.title = "发布题目 - HimuOJ";
});
</script>

<template>
    <n-card style="min-height: calc(100vh - 50px)">
        <h2 class="creator-header">HimuOJ 问题发布向导</h2>
        <n-steps :current="state.currentStep" size="small">
            <n-step title="发布须知" description="请仔细阅读发布须知"/>
            <n-step title="标题 & 代号" description="方便他人快速找到您的问题"/>
            <n-step title="描述" description="详细描述您的问题"/>
            <n-step title="数据 & 约束" description="输入/输出"/>
            <n-step title="汇总" description="检查您的问题并发布"/>
        </n-steps>
        <template #footer v-if="state.currentStep !== 6">
            <n-space style="float: right">
                <n-button type="primary"
                          :loading="state.nextStepLoading"
                          @click="handleNextStep">
                    下一步
                </n-button>
                <n-button @click="() => (state.currentStep -= 1)"> 上一步
                </n-button>
            </n-space>
        </template>
        <transition name="slide-fade" mode="out-in">
            <div v-if="state.currentStep === 1">
                <p>
                    欢迎使用 HimuOJ 问题发布向导！本向导将指导您在 HimuOJ 发布您的题目。
                </p>
                <p>
                    设计合理且有意义的题目是发布者个人能力的体现。 要想发布一个好题目，
                    您需要考虑题目的难度、题目的描述、题目的输入输出、题目的约束等等。
                    您发布的题目必须通过管理员或者测试
                    {{ props.contestCode }} 所属用户同意 审核后才能被其他用户看到。
                </p>
                <p>
                    更准确的说，您发布的题目会被放入待审核列表，要想要尽快通过审核，
                    您需要遵守
                    <n-a href="#"> HimuOJ 发布公约</n-a>
                    ，以及
                    <n-a href="#"> HimuOJ 题目发布指南</n-a>
                    。
                </p>
                <p>
                    请注意，您发布的题目<b>必须是您自己原创</b>的，不得抄袭他人题目。
                    点击
                    '下一步' 说明您已同意以上约定，并开始发布您的题目。
                </p>
            </div>
            <div
                v-else-if="state.currentStep === 2"
                style="width: 80%; margin: 0 auto"
            >
                <h3>标题 & 代号</h3>
                <n-form
                    :rules="briefInfoRules"
                    ref="briefInfoFormRef"
                    :model="state.detail"
                    label-placement="left"
                >
                    <n-form-item label="发布到测试">
                        <n-input :placeholder="props.contestCode" disabled/>
                    </n-form-item>
                    <n-form-item label="题目标题" path="title">
                        <n-input
                            placeholder="标题将会显示到题目列表中"
                            v-model:value="state.detail.title"
                        />
                    </n-form-item>
                    <n-form-item label="题目代号" path="code">
                        <n-input
                            placeholder="代号方便他人使用 URL 以快速找到您的题目"
                            v-model:value="state.detail.code"
                        />
                    </n-form-item>
                    <transition name="slide-fade">
                        <n-form-item v-if="state.detail.code !== ''">
                            <n-alert type="info" show-icon title="注意"
                                     style="width: 100%">
                                您的题目将在
                                <u> /{{
                                        props.contestCode
                                    }}/{{ state.detail.code }} </u>
                                发布。在下一步开始之前我们将会检查您的代号是否合法。
                            </n-alert>
                        </n-form-item>
                    </transition>
                    <transition name="slide-fade">
                        <n-form-item v-if="state.briefInfo.showProblemCodeAlert">
                            <n-alert type="error" show-icon
                                     title="请重新考虑您的问题代码"
                                     style="width: 100%">
                                我们的服务器报告了一个错误，
                                这有可能是由于您的问题代码不符合要求，或与该测试的其他问题的代码发生了冲突。
                            </n-alert>
                        </n-form-item>
                    </transition>
                </n-form>
            </div>
            <div v-else-if="state.currentStep === 3"
                 style="margin: 10px auto">
                <md-editor style="height: 80vh" preview-theme="vuepress"
                           :toolbars="[]" v-model="state.detail.content"/>
            </div>
            <div v-else-if="state.currentStep === 4" style="width: 80%; margin: 5px auto">
                <n-alert type="warning" title="测试点需要额外添加">
                    问题被创建后，任何提交都无法都无法通过。
                    您需要在你所创建的问题管理页面添加测试点后才可以进行提交 & 测试。
                </n-alert>
                <h3> 为您的问题提供必要的约束。 </h3>
                <n-form :model="state.detail"
                        ref="limitFormRef"
                        label-placement="left"
                        :rules="limitRules"
                >
                    <n-form-item label="内存限制" path="maxMemoryLimitByte">
                        <n-input-number style="width: 100%"
                                        v-model:value="state.detail.maxMemoryLimitByte">
                            <template #suffix> BYTE</template>
                        </n-input-number>
                    </n-form-item>
                    <n-form-item label="时间限制" path="maxExecuteTimeLimit">
                        <n-input-number style="width: 100%"
                                        v-model:value="state.detail.maxExecuteTimeLimit">
                            <template #suffix> MS</template>
                        </n-input-number>
                    </n-form-item>
                    <n-form-item label="允许下载测试点的答案">
                        <n-switch v-model:value="state.detail.allowDownloadAnswer"/>
                    </n-form-item>
                    <n-form-item label="允许下载测试点的输入">
                        <n-switch v-model:value="state.detail.allowDownloadInput"/>
                    </n-form-item>
                </n-form>
            </div>
            <div v-else-if="state.currentStep === 5" style="width: 80%; margin: 5px auto">
                <n-alert type="info" title="就绪" style="margin: 5px auto">
                    您的问题已经准备好发布了。以下是您要发布的问题的预览。
                </n-alert>
                <problem-detail-preview :detail="state.detail as ProblemDetail"/>
            </div>
            <div v-else-if="state.currentStep === 6">
                <n-result status="success" title="发布成功" description="您的问题已发布成功！">
                    <template #footer>
                        <n-button> 管理问题</n-button>
                    </template>
                </n-result>
            </div>
        </transition>
    </n-card>
</template>

<style scoped>
.creator-header {
    font-size: 2em;
    margin-top: 5px;
    margin-bottom: 5px;
    font-weight: 300;
}
</style>
