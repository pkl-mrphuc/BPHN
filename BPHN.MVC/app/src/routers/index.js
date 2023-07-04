import { createRouter, createWebHistory } from "vue-router";
import Home from "@/pages/BPHNHome.vue";
import Booking from "@/pages/BPHNBooking.vue";
import PartnerService from "@/pages/BPHNPartnerService.vue";
import ContactMe from "@/pages/BPHNContactMe.vue";

const routes = [
    {
        path: "/",
        component: Home
    },
    {
        path: "/booking",
        component: Booking
    },
    {
        path: "/partner-service",
        component: PartnerService
    },
    {
        path: "/contact-me",
        component: ContactMe
    }
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;