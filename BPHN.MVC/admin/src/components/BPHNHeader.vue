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

<script>
export default {
    name: 'BphnHeader',
    methods: {
        getFullName() {
            return 'Lê Khắc Phúc'
        },

        logout() {
            alert('Logout')
        },

        goToHome() {
            window.location = '/'
        }
    }
}
</script>

<style scoped>
@import '@/assets/css/BPHNHeader.css';
</style>