<script setup>
import {ref} from 'vue'
import { useStore } from 'vuex'

const store = useStore()
const username = ref('')
const password = ref('')

function login() {
  if(!username.value) {
    alert('Tên đăng nhập không được để trống.')
    return
  }
  if(!password.value) {
    alert('Mật khẩu không được để trống.')
    return
  }
  store.dispatch('account/login', { 
    username: username.value,
    password: password.value
  })
}

function goToForgot() {
  window.location = '/forgot'
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
        <h2>Login Form</h2>
        <el-form class="box_right__form">
          <el-form-item>
            <el-input v-model="username" placeholder="Username" />
          </el-form-item>
          <el-form-item>
            <el-input v-model="password" show-password placeholder="Password" type="password" />
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="login">Submit</el-button>
            <a class="forgot-btn" href="javascript:void(0)" @click="goToForgot()">Forgot Password</a>
          </el-form-item>
        </el-form>
      </div>
    </div>
  </section>
</template>

<style scoped>
@import "@/assets/css/BPHNLogin.css";
</style>