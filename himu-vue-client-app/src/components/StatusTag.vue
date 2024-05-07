<template>
	<n-tag
		size="large"
		type="error"
		v-if="props.status === ExecutionStatus.WRONG_ANSWER"
	>
		结果错误
	</n-tag>
	<n-tag
		size="large"
		type="success"
		v-else-if="props.status === ExecutionStatus.ACCEPTED"
	>
		已通过
	</n-tag>
	<n-tag
		size="large"
		type="warning"
		v-else-if="props.status === ExecutionStatus.RUNNING"
	>
		正在运行
	</n-tag>
	<n-tag
		size="large"
		type="default"
		v-else-if="props.status === ExecutionStatus.PENDING"
	>
		等待运行
	</n-tag>
	<n-tag
		size="large"
		type="error"
		v-else-if="props.status === ExecutionStatus.COMPILE_ERROR"
	>
		编译错误
	</n-tag>
	<n-tag
		size="large"
		type="error"
		v-else-if="props.status === ExecutionStatus.SYSTEM_ERROR"
	>
		系统错误
	</n-tag>
	<n-tag
		size="large"
		type="error"
		v-else-if="props.status === ExecutionStatus.TIME_LIMIT_EXCEEDED"
	>
		时间超限
	</n-tag>
    <n-tag
        size="large"
        type="error"
        v-else-if="props.status === ExecutionStatus.MEMORY_LIMIT_EXCEEDED"
    >
        内存超限
    </n-tag>
    <n-tag size="large" type="error" v-else>
        {{ props.status }}
    </n-tag>
</template>

<script setup lang="ts">
import { NTag } from "naive-ui";
import { defineProps } from "vue";
import { ExecutionStatus } from "@/models/ResultModels";

const props = defineProps({
	status: {
		type: String,
		default: "default",
	},
});
</script>
