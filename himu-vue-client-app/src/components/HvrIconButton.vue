<template>
	<n-button type="primary" :class="props.animation" block secondary strong>
		<n-space>
			<n-icon :component="props.component" class="hvr-icon">
			</n-icon>
			<span>{{ props.text }}</span>
		</n-space>
	</n-button>
</template>

<script lang="ts" setup>
import { NButton, NIcon, NSpace } from "naive-ui";
import { ChevronForward } from "@vicons/ionicons5";

const props = defineProps({
	animation: {
		type: String,
		default: "hvr-icon-forward",
	},
	text: {
		type: String,
		default: "Button",
	},
	component: {
		default: ChevronForward,
	},
});
</script>
