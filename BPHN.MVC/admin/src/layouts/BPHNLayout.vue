<script setup>
import AccountAPI from '@/apis/AccountAPI'
import { loadConfig } from '@/loader'
import { useStore } from 'vuex'
import { useI18n } from 'vue-i18n'

const t = useI18n()
const store = useStore()

let validateResult = await AccountAPI.validateToken(store.getters['account/getToken'])
if (validateResult?.data?.success) {
  loadConfig()
  t.locale.value = window["Language"]
}
else {
  window.location = '/login'
}

</script>

<template>
  <div class="common-layout">
    <el-container>
      <el-header>
        <bphn-header></bphn-header>
      </el-header>
      <el-container>
        <el-aside width="200px">
          <bphn-menu></bphn-menu>
        </el-aside>
        <el-main>
          <router-view />
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script>
import BphnHeader from '@/layouts/BPHNHeader.vue'
import BphnMenu from '@/layouts/BPHNMenu.vue'
export default {
  name: "BPHNLayout",
  components: { BphnHeader, BphnMenu },
};
</script>