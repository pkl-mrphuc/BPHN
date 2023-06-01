<script setup>
import { useI18n } from "vue-i18n"
import FootballFieldCard from '@/components/FootballFieldCard.vue'
import useActionModal from '@/register-components/actionDialog'
import MyFootballFieldDialog from '@/components/dialogs/BPHNFootballFieldDialog.vue'

const { t } = useI18n()
const { openModal } = useActionModal()

const addNew = (() => {
    debugger; // eslint-disable-line no-debugger
    openModal(MyFootballFieldDialog)
})
</script>

<template>
  <section class="pbhn-football-fields">
    <div class="container">
      <div class="head">
        <h3 class="head_title">{{ t("MyFootballFields") }}</h3>
        <div class="head_toolbar">
          <el-button type="primary" @click="addNew">{{ t("AddNew") }}</el-button>
        </div>
      </div>
      <div class="body">
        <el-row>
          <el-col
            v-for="(o, index) in 2"
            :key="o"
            :span="5"
            :offset="index > 0 ? 1 : 0"
            >
            <football-field-card
                :name="'Đầm hồng'"
                :status="'INACTIVE'"
            ></football-field-card>
          </el-col>
        </el-row>
      </div>
    </div>
  </section>
</template>

<style scoped>
@import "@/assets/css/BPHNMyFootballFields.css";
</style>
