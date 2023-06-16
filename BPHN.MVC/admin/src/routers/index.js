import { createRouter, createWebHistory } from "vue-router"
import Layout from "@/layouts/BPHNLayout.vue"
import Calendar from "@/components/BPHNCalendar.vue"
import LoginPage from "@/pages/BPHNLogin.vue"
import ForgotPage from "@/pages/BPHNForgot.vue"
import ResetPasswordPage from "@/pages/BPHNResetPassword.vue"
import MyFootballFields from "@/components/BPHNMyFootballFields.vue"
import Configurations from "@/components/BPHNConfigurations.vue"
import BookingManager from "@/components/BPHNBookingManager.vue"

const routes = [
  {
    path: "/",
    component: Layout,
    children: [
      {
        path: "/calendar",
        component: Calendar,
        meta: {
          title: "CalendarTitle"
        }
      },
      {
        path: "/bm",
        component: BookingManager,
        meta: {
          title: "BMTitle"
        }
      },
      {
        path: "/my-football-fields",
        component: MyFootballFields,
        meta: {
          title: "FootballFieldTitle"
        }
      },
      {
        path: "/configuartions",
        component: Configurations,
        meta: {
          title: "ConfigurationsTitle"
        }
      }
    ]
  },
  {
    path: "/login",
    name: "login",
    component: LoginPage,
    meta: {
      title: "LoginForm"
    }
  },
  {
    path: "/forgot",
    name: "forgot",
    component: ForgotPage,
    meta: {
      title: "ForgotPasswordTitle"
    }
  },
  {
    path: "/reset-password",
    name: "reset-password",
    component: ResetPasswordPage,
    meta: {
      title: "ResetPasswordTitle"
    }
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

router.beforeEach(async (to) => {

  const publicPages = ["/login", "/forgot", "/reset-password"]

  const authRequired = !publicPages.includes(to.path)

  if (!authRequired) {
    localStorage.clear()
  }

  const authKey = JSON.parse(localStorage.getItem("admin-auth-key"))
  if (authRequired && !authKey?.account?.context?.token) {
    return "/login"
  }

});

export default router;