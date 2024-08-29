<script setup>
import useCommonFn from "@/commonFn";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useStore } from "vuex";
import { ConfigKeyEnum, RoleEnum } from "@/const";
import { ElNotification } from "element-plus";

const store = useStore();
const { t } = useI18n();
const darkMode = ref(store.getters["config/getDarkMode"]);
const language = ref(store.getters["config/getLanguage"]);
const formatDate = ref(store.getters["config/getFormatDate"]);
const multiUser = ref(store.getters["config/getMultiUser"]);
const role = ref(store.getters["account/getRole"]);
const running = ref(0);
const { equals } = useCommonFn();

const lstConfig = computed(() => {
  if (
    multiUser.value &&
    (equals(role, RoleEnum.ADMIN) || equals(role, RoleEnum.TENANT))
  ) {
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

const useMultiUser = () => {
  if (multiUser.value) {
    ElNotification({
      title: t("Notification"),
      message: t("ContactSupplier"),
      type: "info",
    });
    multiUser.value = false;
  }
};

const save = () => {
  if (running.value > 0) return;
  ++running.value;
  let configs = [
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
  ];
  store.dispatch("config/save", configs);

  setTimeout(() => {
    running.value = 0;
  }, 1000);
};
</script>

<template>
  <section>
    <div class="container">
      <div class="d-flex flex-row align-items-center justify-content-between">
        <h3 class="fs-3">{{ t("Configurations") }}</h3>
        <div>
          <el-button type="primary" @click="save">{{ t("Save") }}</el-button>
        </div>
      </div>
      <div>
        <el-table :data="lstConfig" style="height: calc(100vh - 190px)">
          <el-table-column :label="t('Title')" width="200">
            <template #default="scope">
              <span>{{ t(scope.row.name) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="" min-width="300">
            <template #default="scope">
              <el-switch v-if="equals(scope.row.key, ConfigKeyEnum.DARKMODE)" v-model="darkMode" />
              <el-select v-if="equals(scope.row.key, ConfigKeyEnum.LANGUAGE)" v-model="language">
                <el-option value="vi" label="Vietnamese" />
                <el-option value="en" label="English" />
              </el-select>
              <el-select v-if="equals(scope.row.key, ConfigKeyEnum.FORMATDATE)" v-model="formatDate">
                <el-option value="yyyy-MM-dd" label="yyyy-MM-dd" />
                <el-option value="dd/MM/yyyy" label="dd/MM/yyyy" />
                <el-option value="dd-MM-yyyy" label="dd-MM-yyyy" />
              </el-select>
              <el-switch v-if="equals(scope.row.key, ConfigKeyEnum.MULTIUSER)" v-model="multiUser" disabled
                @change="useMultiUser" />
            </template>
          </el-table-column>
        </el-table>
      </div>
    </div>
  </section>
</template>
