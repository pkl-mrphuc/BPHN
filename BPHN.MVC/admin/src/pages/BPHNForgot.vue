<script setup>
import {ref} from 'vue'

const email = ref('')

function goToLogin() {
  window.location = '/login'
}
</script>

<template>
  <section class="pbhn-login">
    <div class="box">
      <div class="box_left">
        <h1 class="box_left__title">Booking Pitch Ha Noi</h1>
        <img class="img img-anime" src=".././assets/images/fb-anime.jpg" />
      </div>

      <div class="box_right">
        <h2>Forgot Password</h2>
        <el-form class="box_right__form">
          <el-form-item>
            <el-input v-model="email" placeholder="Email" />
          </el-form-item>
          <el-form-item>
            <el-button style="width: 100%" type="primary">Send Request</el-button>
          </el-form-item>
        </el-form>
        <a class="back-login-btn" @click="goToLogin()" href="javascript:void(0)" >Back To Login</a>
      </div>
    </div>
  </section>
</template>

<style scoped>
@import "@/assets/css/BPHNLogin.css";
</style>