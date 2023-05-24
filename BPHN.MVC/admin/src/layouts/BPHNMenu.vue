<script setup>
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
</script>

<template>
    <section class="bphn-menu">
        <div class="menus">
            <div class="menu-item">
                <router-link class="menu-item_link" :title="t('BMTitle')" to="/bm">{{ t('BM') }}</router-link>
            </div>
            <div class="menu-item">
                <router-link class="menu-item_link" :title="t('CalendarTitle')" to="/calendar">{{ t('Calendar') }}</router-link>
            </div>
            <div class="menu-item">
                <router-link class="menu-item_link" :title="t('MyGroundsTitle')" to="/my-grounds">{{ t('MyGrounds') }}</router-link>
            </div>
            <div class="menu-item">
                <router-link class="menu-item_link" :title="t('ConfigurationsTitle')" to="/configuartions">{{ t('Configurations') }}</router-link>
            </div>
        </div>
    </section>
</template>

<style scoped>
@import '@/assets/css/BPHNMenu.css';
</style>