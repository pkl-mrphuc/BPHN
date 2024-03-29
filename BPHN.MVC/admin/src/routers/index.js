import { createRouter, createWebHistory } from "vue-router";
import Layout from "@/layouts/BPHNLayout.vue";
import Calendar from "@/components/BPHNCalendar.vue";
import LoginPage from "@/pages/BPHNLogin.vue";
import ForgotPage from "@/pages/BPHNForgot.vue";
import SetPasswordPage from "@/pages/BPHNSetPassword.vue";
import MyFootballFields from "@/components/BPHNMyFootballFields.vue";
import Configurations from "@/components/BPHNConfigurations.vue";
import BookingManager from "@/components/BPHNBookingManager.vue";
import HistoryLogs from "@/components/BPHNHistoryLogs.vue";
import Tenants from "@/components/BPHNTenants.vue";

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
      },
      {
        path: "/history-logs",
        component: HistoryLogs,
        meta: {
          title: "HistoryLogsTitle"
        }
      },
      {
        path: "/tenants",
        component: Tenants,
        meta: {
          title: "AccountsTitle"
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
    path: "/set-password",
    name: "set-password",
    component: SetPasswordPage,
    meta: {
      title: "SetPasswordTitle"
    }
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

router.beforeEach(async (to) => {

  const publicPages = ["/login", "/forgot", "/set-password"]

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