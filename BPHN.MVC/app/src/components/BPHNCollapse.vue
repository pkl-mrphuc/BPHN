<script setup>
import { CaretRight } from '@element-plus/icons-vue';
import BphnCollapseItem from '@/components/BPHNCollapseItem.vue';
</script>

<template>
    <div>
        <div class="collapse d-flex flex-row align-items-center pointer">
            <el-icon size="24">
                <CaretRight />
            </el-icon>
            <p class="m-0 ml-2 fs-4 fw-bold">Cơ sở đầm hồng</p>
        </div>
        <div class="collapse-items">
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
</style>