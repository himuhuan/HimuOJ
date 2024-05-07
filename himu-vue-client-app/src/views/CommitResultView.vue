<template>
  <center-content-container width="95%">
    <commit-result-card :commitDetail="commitDetail"
                        v-if="commitDetail"></commit-result-card>
  </center-content-container>
</template>

<script setup lang="ts">
import {CommitsServices} from "@/services/CommitsServices";
import {ref, onMounted} from "vue";
import router from "@/routers";
import {CommitDetail} from "@/models/CommitDetail";
import CenterContentContainer from "@/components/CenterContentContainer.vue";
import CommitResultCard from "@/components/commits/CommitResultCard.vue";


const commitDetail = ref<null | CommitDetail>(null);

const props = defineProps({
  commitId: {
    type: String,
    required: true,
    default: router.currentRoute.value.params.commitId,
  },
});

onMounted(async () => {
  commitDetail.value = await CommitsServices.getCommitDetail(props.commitId);
});

</script>
