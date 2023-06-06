<script setup>
import { useI18n } from "vue-i18n";
import { Refresh } from "@element-plus/icons-vue";
import useToggleModal from "@/register-components/actionDialog";

const { t } = useI18n();
const { openModal, hasRole } = useToggleModal();

const addNew = () => {
  openModal("BookingDialog");
};

const bmData = []
</script>

<template>
  <section class="pbhn-screen pbhn-bm">
    <div class="container">
      <div class="head">
        <h3 class="head_title">{{ t("BookingManager") }}</h3>
        <div class="head_toolbar">
          <el-button>
            <el-icon><Refresh /></el-icon>
          </el-button>
          <el-button type="primary" @click="addNew">{{
            t("AddNew")
          }}</el-button>
        </div>
      </div>
      <div class="body">
        <el-table :data="bmData" style="width: 100%;">
          <el-table-column label="Trạng thái">
          </el-table-column>
          <el-table-column label="Ngày đặt">
          </el-table-column>
          <el-table-column label="Người đặt">
          </el-table-column>
          <el-table-column label="Khung giờ">
          </el-table-column>
          <el-table-column label="Từ ngày - Đến ngày">
          </el-table-column>
          <el-table-column label="Sân">
          </el-table-column>
        </el-table>
      </div>
    </div>
  </section>
  <el-empty :description="t('NoData')" v-if="false" />
  <BookingDialog 
  v-if="hasRole('BookingDialog')"
  ></BookingDialog>
</template>

