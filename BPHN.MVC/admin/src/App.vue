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
import BphnHeader from '@/components/BPHNHeader.vue'
import BphnMenu from '@/components/BPHNMenu.vue'
export default {
  name: 'App',
  components: { BphnHeader, BphnMenu }
}
</script>
