<template>
    {{ JSON.stringify(data) }}
</template>

<script lang="ts" setup>
import { CommitsServices } from "@/services/CommitsServices"
import {ref, onMounted} from "vue";

const data = ref<any>(null);

onMounted(async () => {
    data.value = await CommitsServices.getResult('493128096251973');
})

</script>