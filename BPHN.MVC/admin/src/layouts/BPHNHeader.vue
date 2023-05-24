<script setup>
import { computed } from 'vue'
import { useStore } from 'vuex'

const store = useStore()
const fullname = computed(() => {
    return store.getters['account/getFullName']
})

function logout() {
    localStorage.clear()
    window.location = '/login'
}

function goToHome() {
    window.location = '/'
}
</script>

<template>
    <section class="pbhn-header">
        <div class="header_left">
            <h1 class="header_left__logo" @click="goToHome()">BPHN</h1>
        </div>
        <div class="header_right">
            <p class="header_right__fullname">Xin chào, <span class="header_right__fullname--underline">{{ fullname }}</span></p>
            <div class="header_right__logout" @click="logout()">
                <div class="icon36 icon-logout"></div>
            </div>
        </div>
    </section>
</template>

<style scoped>
@import '@/assets/css/BPHNHeader.css';
</style>