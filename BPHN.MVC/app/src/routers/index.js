import { createRouter, createWebHistory } from "vue-router";
import Home from "@/pages/BPHNHome.vue";
import Search from "@/pages/BPHNSearch.vue";
import RegisterService from "@/pages/BPHNRegisterService.vue";
import Contact from "@/pages/BPHNContact.vue";

const routes = [
    {
        path: "/",
        component: Home
    },
    {
        path: "/search",
        component: Search
    },
    {
        path: "/register-service",
        component: RegisterService
    },
    {
        path: "/contact",
        component: Contact
    }
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;