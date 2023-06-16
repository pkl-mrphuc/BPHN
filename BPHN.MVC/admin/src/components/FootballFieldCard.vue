<script setup>
import { computed, defineProps, ref, defineEmits } from "vue";
import StatusDot from "@/components/StatusDot.vue";
import { useI18n } from "vue-i18n";
import { useStore } from "vuex";

const { t } = useI18n();
const store = useStore();
const emit = defineEmits(["edit"]);
const props = defineProps({
  name: String,
  status: String,
  avatarUrl: String,
  id: String,
});

const hdfFile = ref(null);
const imgAvatar = ref(null);

const status = computed(() => {
  switch (props.status) {
    case "ACTIVE":
      return "Đang hoạt động";
    case "INACTIVE":
      return "Ngừng hoạt động";
  }
  return "Ngừng hoạt động";
});

const edit = () => {
  emit("edit", props.id);
};

const upload = () => {
  hdfFile.value.click();
};

const changeHdfFile = (event) => {
  let files = event.target.files;
  if (files?.length > 0) {
    hdfFile.value = files[0];
    readImageFile(files[0]);
    store.dispatch("file/upload", {
      file: files[0],
      id: props.id,
    });
  }
};

const readImageFile = (file) => {
  let reader = new FileReader();
  reader.onload = (e) => {
    imgAvatar.value.src = e.target.result;
  };
  reader.readAsDataURL(file);
};
</script>


<template>
  <el-card :body-style="{ padding: '0px' }">
    <img
      v-if="props.avatarUrl"
      ref="imgAvatar"
      :src="props.avatarUrl"
      height="300"
      class="image"
    />
    <img
      v-else
      height="300"
      ref="imgAvatar"
      src="../assets/images/football-field.png"
      class="image"
    />

    <div style="padding: 14px">
      <h3 style="margin: 5px 0px">{{ props.name }}</h3>
      <div class="bottom">
        <status-dot :status="props.status" :message="status"></status-dot>
        <div>
          <el-button class="button" @click="upload">{{
            t("UploadImage")
          }}</el-button>
          <el-button class="button" type="primary" @click="edit">{{
            t("Edit")
          }}</el-button>
        </div>
        <!-- hdf = hidden field -->
        <input
          type="file"
          hidden
          accept="image/*"
          @change="changeHdfFile"
          name="hdfFile"
          ref="hdfFile"
        />
      </div>
    </div>
  </el-card>
</template>