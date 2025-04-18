import { createRouter, createWebHistory } from "vue-router";
import Home from "@/components/BPHNHome.vue";
import AboutUs from "@/components/BPHNAboutUs.vue";
import BookingStep1 from "@/components/BookingStep1.vue";
import Stadiums from "@/components/BPHNStadiums.vue";
import Layout from "@/layouts/BPHNLayout.vue";

const routes = [
    {
        path: "/",
        component: Layout,
        children: [
            {
                path: "/",
                component: Home
            },
            {
                path: "/booking",
                component: BookingStep1
            },
            {
                path: "/about-us",
                component: AboutUs
            },
            {
                path: "/stadiums",
                component: Stadiums
            }
        ]
    }
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;