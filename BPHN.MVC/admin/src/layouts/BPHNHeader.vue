<script setup>

function getFullName() {
    return 'Lê Khắc Phúc'
}

function logout() {
    alert('Logout')
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
            <p class="header_right__fullname">Xin chào, <span class="header_right__fullname--underline">{{ getFullName() }}</span></p>
            <div class="header_right__logout" @click="logout()">
                <div class="icon36 icon-logout"></div>
            </div>
        </div>
    </section>
</template>

<style scoped>
@import '@/assets/css/BPHNHeader.css';
</style>