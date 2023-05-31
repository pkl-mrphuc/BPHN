<script setup>
import { computed, defineProps } from 'vue'
import StatusDot from '@/components/StatusDot.vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const props = defineProps({
    name: String,
    status: Number,
    avatarUrl: String
})

const status = computed(() => {
    switch(props.status) {
        case 'ACTIVE':
            return 'Đang hoạt động'
        case 'INACTIVE':
            return 'Ngừng hoạt động'
    }
    return 'Ngừng hoạt động'
})

const edit = () => {
    alert('edit')
}

</script>


<template>
  <el-card :body-style="{ padding: '0px' }">
    <img
      :src="props.avatarUrl"
      class="image"
    />
    <div style="padding: 14px">
      <span>{{ props.name }}</span>
      <div class="bottom">
        <status-dot 
            :status="props.status"
            :message="status"
        ></status-dot>
        <el-button text class="button" @click="edit">{{ t('Edit') }}</el-button>
      </div>
    </div>
  </el-card>
</template>