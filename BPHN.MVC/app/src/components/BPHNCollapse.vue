<script setup>
import { ref, defineProps } from 'vue';
import { CaretRight } from '@element-plus/icons-vue';
import BphnCollapseItem from '@/components/BPHNCollapseItem.vue';

const props = defineProps({
  title: String,
  items: Array
});

const isOpen = ref(false);

const toggleCollapse = () => {
  isOpen.value = !isOpen.value;
};
</script>

<template>
    <div class="collapse-panel">
        <div class="collapse-panel__header d-flex flex-row align-items-center pointer" @click="toggleCollapse">
            <el-icon size="24" :class="{ 'rotate-90': isOpen }">
                <CaretRight />
            </el-icon>
            <p class="collapse-panel__title m-0 ml-2 fs-4 fw-bold">{{ props.title }}</p>
        </div>
        <div class="collapse-panel__content" v-show="isOpen">
            <bphn-collapse-item class="collapse-panel__item" v-for="item in props.items" :key="item" :title="item.title" :description="item.description"></bphn-collapse-item>
        </div>
    </div>
</template>

<style scoped>
.collapse-panel__header {
    box-shadow: 0px 4px 4px 0px #0000004D;
    background: #EAEAEA;
    padding: 16px 8px;
}

.collapse-panel__content {
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