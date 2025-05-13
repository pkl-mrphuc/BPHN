<script setup>
import { defineProps } from 'vue';

const props = defineProps({
  title: String,
  description: String
});
</script>
<template>
    <div class="collapse-item pointer">
        <div class="collapse-item__container d-flex flex-row align-items-center justify-content-between">
            <div class="collapse-item__content">
                <p class="collapse-item__title m-0 fw-normal mb-1">{{ props.title }}</p>
                <p class="collapse-item__description m-0 fw-normal">{{ props.description }}</p>
            </div>
            <el-button class="collapse-item__button">Booking Now</el-button>
        </div>
    </div>
</template>
<style scoped>
.collapse-item {
    background: #F2F2F2;
    border-bottom: 1px solid #9D9D9D;
    border-radius: 12px;
    padding: 8px 16px;
    margin-bottom: 10px;
}

.collapse-item__title {
    font-size: 16px;
}

.collapse-item__description {
    font-size: 12px;
}

.collapse-item__button, 
.collapse-item__button:hover {
    background: #093D67;
    color: #ffffff;
}
</style>