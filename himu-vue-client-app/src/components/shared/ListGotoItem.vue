<script setup lang="ts">
import { NListItem, NSpace, NIcon } from "naive-ui";
import { ArrowForwardIosRound as ArrowRightIcon } from "@vicons/material";
</script>

<template>
	<n-list-item>
		<n-space align="center" justify="space-between">
			<span class="info-list-item">
				<slot name="icon"></slot>
				&nbsp;
				<slot></slot>
			</span>
			<n-icon size="small">
				<arrow-right-icon />
			</n-icon>
		</n-space>
	</n-list-item>
</template>

<style scoped>
.info-list-item {
	padding: 5px 0;
	display: flex;
	align-items: center;
}
</style>
