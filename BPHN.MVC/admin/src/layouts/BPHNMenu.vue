<template>
    <section class="bphn-menu">
        <div class="menus">
            <div class="menu-item">
                <router-link class="menu-item_link" title="Booking manager" to="/bm">BM</router-link>
            </div>
            <div class="menu-item">
                <router-link class="menu-item_link" title="Calendar" to="/calendar">Calendar</router-link>
            </div>
            <div class="menu-item">
                <router-link class="menu-item_link" title="My grounds" to="/my-grounds">My Grounds</router-link>
            </div>
            <div class="menu-item">
                <router-link class="menu-item_link" title="Configurations" to="/configuartions">Configurations</router-link>
            </div>
        </div>
    </section>
</template>

<script>
export default {
    name: 'BPHNMenu'
}
</script>

<style scoped>
@import '@/assets/css/BPHNMenu.css';
</style>