import { createRouter, createWebHistory } from "vue-router";
import Home from "@/components/BPHNHome.vue";
import HomeMobile from "@/components/mobile/BPHNHome.vue";
import AboutUs from "@/components/BPHNAboutUs.vue";
import Booking from "@/components/BPHNBooking.vue";
import BookingMobile from "@/components/mobile/BPHNBooking.vue";
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
                path: "/mobile/",
                component: HomeMobile
            },
            {
                path: "/booking",
                component: Booking
            },
            {
                path: "/mobile/booking",
                component: BookingMobile
            },
            {
                path: "/about-us",
                component: AboutUs
            }
        ]
    }
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

router.beforeEach((to, from, next) => {
    const isMobile = window.innerWidth <= 768;
    if (isMobile && !to.path.startsWith("/mobile/")) {
        const mobilePath = `/mobile${to.path}`;
        next(mobilePath);
    } else if (!isMobile && to.path.startsWith("/mobile")) {
        const webPath = to.path.replace("/mobile/", "/");
        next(webPath);
    } else {
        next();
    }
});

export default router;