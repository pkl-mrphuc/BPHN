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
    name: 'BPHNLayout',
    components: { BphnHeader, BphnMenu }
}
</script>