<script setup>
import useCommonFn from "@/commonFn";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useStore } from "vuex";
import { ConfigKeyEnum, RoleEnum } from "@/const";
import { ElNotification } from "element-plus";
import useToggleModal from "@/register-components/actionDialog";
import {
  Refresh
} from "@element-plus/icons-vue";
import router from "@/routers";

const { openModal, hasRole } = useToggleModal();
const store = useStore();
const { t } = useI18n();
const darkMode = ref(store.getters["config/getDarkMode"]);
const language = ref(store.getters["config/getLanguage"]);
const formatDate = ref(store.getters["config/getFormatDate"]);
const multiUser = ref(store.getters["config/getMultiUser"]);
const email = ref(store.getters["config/getSystemEmail"]);
const role = ref(store.getters["account/getRole"]);
const isMobile = ref(store.getters["config/isMobile"]);
const objConnect = ref(null);
const running = ref(0);
const { equals } = useCommonFn();

const lstConfig = computed(() => {
  if (multiUser.value && (equals(role.value, RoleEnum.ADMIN) || equals(role.value, RoleEnum.TENANT))) {
    return [
      {
        name: t("DarkMode"),
        key: ConfigKeyEnum.DARKMODE,
      },
      {
        name: t("Language"),
        key: ConfigKeyEnum.LANGUAGE,
      },
      {
        name: t("FormatDate"),
        key: ConfigKeyEnum.FORMATDATE,
      },
      {
        name: t("MultiUser"),
        key: ConfigKeyEnum.MULTIUSER,
      },
      {
        name: t("SystemEmail"),
        key: ConfigKeyEnum.SYSTEMEMAIL,
      }
    ];
  } else {
    return [
      {
        name: t("DarkMode"),
        key: ConfigKeyEnum.DARKMODE,
      },
      {
        name: t("Language"),
        key: ConfigKeyEnum.LANGUAGE,
      },
      {
        name: t("FormatDate"),
        key: ConfigKeyEnum.FORMATDATE,
      },
      {
        name: t("SystemEmail"),
        key: ConfigKeyEnum.SYSTEMEMAIL,
      }
    ];
  }
});

const getDarkMode = computed(() => {
  return store.getters["config/getDarkMode"];
});

const getLanguage = computed(() => {
  return store.getters["config/getLanguage"];
});

const getFormatDate = computed(() => {
  return store.getters["config/getFormatDate"];
});

const getMultiUser = computed(() => {
  return store.getters["config/getMultiUser"];
});

const getSystemEmail = computed(() => {
  return store.getters["config/getSystemEmail"];
});

watch(getDarkMode, (newValue) => {
  darkMode.value = newValue;
});

watch(getLanguage, (newValue) => {
  language.value = newValue;
});

watch(getFormatDate, (newValue) => {
  formatDate.value = newValue;
});

watch(getMultiUser, (newValue) => {
  multiUser.value = newValue;
});

watch(getSystemEmail, (newValue) => {
  email.value = newValue;
});

const useMultiUser = () => {
  if (multiUser.value) {
    ElNotification({ title: t("Notification"), message: t("ContactSupplier"), type: "info", });
    multiUser.value = false;
  }
};

const save = () => {
  if (running.value > 0) return;
  ++running.value;
  setTimeout(() => {
    running.value = 0;
  }, 1000);

  store.dispatch("config/save", 
  [
    {
      Key: ConfigKeyEnum.DARKMODE,
      Value: `${darkMode.value}`,
    },
    {
      Key: ConfigKeyEnum.LANGUAGE,
      Value: language.value,
    },
    {
      Key: ConfigKeyEnum.FORMATDATE,
      Value: formatDate.value,
    },
    {
      Key: ConfigKeyEnum.MULTIUSER,
      Value: `${multiUser.value}`,
    },
    {
      Key: ConfigKeyEnum.SYSTEMEMAIL,
      Value: `${email.value}`,
    }
  ]);
};

const connect = () => {
  openModal("ConfigDialog");
  objConnect.value = email.value;
};

const onBack = () => {
  router.push("/");
};
</script>

<template>
  <section>
    <div class="container">
      <el-page-header icon="" v-if="isMobile" class="mb-3" @back="onBack">
        <template #content>
          <span class="text-large font-600 mr-3">{{ t("Configurations") }}</span>
        </template>
        <template #extra>
          <div class="d-flex flex-row">
            <el-button type="primary" @click="save">{{ t("Save") }}</el-button>
          </div>
        </template>
      </el-page-header>
      <div v-else class="row mb-3 d-flex flex-row align-items-center justify-content-between">
        <h3 class="col-12 col-sm-12 col-md-12 col-lg-8 fs-3 mt-1 mb-1">{{ t("Configurations") }}</h3>
        <div>
          <el-button type="primary" @click="save">{{ t("Save") }}</el-button>
        </div>
      </div>
      <div>
        <el-table :data="lstConfig" border style="height: calc(100vh - 156px)">
          <el-table-column :label="t('Title')" width="200">
            <template #default="scope">
              <span>{{ t(scope.row.name) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="" min-width="300">
            <template #default="scope">
              <div class="col-8 col-sm-8 col-md-6 col-lg-2">
                <el-switch v-if="equals(scope.row.key, ConfigKeyEnum.DARKMODE)" v-model="darkMode" />
              </div>

              <div class="col-8 col-sm-8 col-md-6 col-lg-2">
                <el-select v-if="equals(scope.row.key, ConfigKeyEnum.LANGUAGE)" v-model="language" class="w-100">
                  <el-option value="vi" label="Vietnamese" />
                  <el-option value="en" label="English" />
                </el-select>
              </div>

              <div class="col-8 col-sm-8 col-md-6 col-lg-2">
                <el-select v-if="equals(scope.row.key, ConfigKeyEnum.FORMATDATE)" v-model="formatDate" class="w-100">
                  <el-option value="yyyy-MM-dd" label="yyyy-MM-dd" />
                  <el-option value="dd/MM/yyyy" label="dd/MM/yyyy" />
                  <el-option value="dd-MM-yyyy" label="dd-MM-yyyy" />
                </el-select>
              </div>

              <div class="col-8 col-sm-8 col-md-6 col-lg-2">
                <el-switch v-if="equals(scope.row.key, ConfigKeyEnum.MULTIUSER)" v-model="multiUser" disabled
                  @change="useMultiUser" />
              </div>

              <div class="col-8 col-sm-8 col-md-6 col-lg-2">
                <el-input v-if="equals(scope.row.key, ConfigKeyEnum.SYSTEMEMAIL)" v-model="email" maxlength="225"
                  class="w-100">
                  <template #append>
                    <el-button :icon="Refresh" @click="connect" />
                  </template>
                </el-input>
              </div>

            </template>
          </el-table-column>
        </el-table>
      </div>
    </div>
  </section>
  <ConnectSystemEmailDialog v-if="hasRole('ConfigDialog')" :data="objConnect"></ConnectSystemEmailDialog>
</template>
