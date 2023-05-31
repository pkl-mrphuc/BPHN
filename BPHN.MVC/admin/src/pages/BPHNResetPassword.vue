<script setup>
import {computed, ref} from 'vue'
import { useStore } from 'vuex'
import { useI18n } from 'vue-i18n'
import { useRoute } from 'vue-router'

const { t } = useI18n()
const store = useStore()
const route = useRoute()

const password = ref('')
const passwordAgain = ref('')

const userName = computed(() => {
    return getQueryStringByKey('userName')
})

const getQueryStringByKey = ((key) => {
    return route.query[key]
})

const goToLogin = (() => {
  window.location = '/login'
})

const submit = (() => {
  if(!password.value || !passwordAgain.value) {
    alert(t('PasswordEmptyMesg'))
    return
  }
  if(password.value != passwordAgain.value) {
    alert(t('NoMatchPasswordMesg'))
    return
  }

  let data = {
    userName: userName.value,
    code: getQueryStringByKey('code'),
    password: password.value
  }
  store.dispatch('account/resetPassword', data)
})
</script>

<template>
  <section class="pbhn-login">
    <div class="box">
      <div class="box_left">
        <h1 class="box_left__title">{{ t('BookingPitchHaNoi') }}</h1>
        <img class="img img-anime" src=".././assets/images/fb-anime.jpg" />
      </div>

      <div class="box_right">
        <h2 style="margin-bottom: 0">{{ t('ResetPasswordTitle') }}</h2>
        <h3>
            <i>{{ userName }}</i>
        </h3>
        <el-form class="box_right__form">
          <el-form-item>
            <el-input v-model="password" show-password :placeholder="t('Password')" type="password" />
          </el-form-item>
          <el-form-item>
            <el-input v-model="passwordAgain" show-password :placeholder="t('PasswordAgain')" type="password" />
          </el-form-item>
          <el-form-item>
            <el-button style="width: 100%" type="primary" @click="submit()">{{ t('Submit') }}</el-button>
          </el-form-item>
        </el-form>
        <a class="back-login-btn" @click="goToLogin()" href="javascript:void(0)">{{ t('BackToLogin') }}</a>
      </div>
    </div>
  </section>
</template>

<style scoped>
@import "@/assets/css/BPHNLogin.css";
</style>