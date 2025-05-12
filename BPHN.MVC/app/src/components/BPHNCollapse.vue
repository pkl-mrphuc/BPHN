<script setup>
import { ref } from 'vue';
import { CaretRight } from '@element-plus/icons-vue';
import BphnCollapseItem from '@/components/BPHNCollapseItem.vue';
const isOpen = ref(false);

const toggleCollapse = () => {
  isOpen.value = !isOpen.value;
};
</script>

<template>
    <div>
        <div class="collapse d-flex flex-row align-items-center pointer" @click="toggleCollapse">
            <el-icon size="24" :class="{ 'rotate-90': isOpen }">
                <CaretRight />
            </el-icon>
            <p class="m-0 ml-2 fs-4 fw-bold">Cơ sở đầm hồng</p>
        </div>
        <div class="collapse-items" v-show="isOpen">
            <bphn-collapse-item></bphn-collapse-item>
            <bphn-collapse-item></bphn-collapse-item>
            <bphn-collapse-item></bphn-collapse-item>
        </div>
    </div>
</template>

<style scoped>
.collapse {
    box-shadow: 0px 4px 4px 0px #0000004D;
    background: #EAEAEA;
    padding: 16px 8px;
}

.collapse-items {
    background: #FFFFFFF2;
    border: 1px solid #EFEFEF;
    min-height: 100px;
    margin-top: 8px;
    padding: 10px 20px;
}

.rotate-90 {
  transform: rotate(90deg);
  transition: transform 0.3s ease;
}
</style>