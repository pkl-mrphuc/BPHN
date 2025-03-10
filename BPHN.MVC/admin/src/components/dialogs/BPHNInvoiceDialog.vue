<script setup>
import useToggleModal from "@/register-components/actionDialog";
import { defineProps, ref, onMounted, defineEmits } from "vue";
import { useI18n } from "vue-i18n";
import { Delete } from "@element-plus/icons-vue";
import { useStore } from "vuex";

const { t } = useI18n();
const { toggleModel } = useToggleModal();
const store = useStore();
const emit = defineEmits(["callback"]);
const props = defineProps({
  data: Array
});

const running = ref(0);
const lstItem = ref([]);
const lstInvoiceItem = ref(props.data?.items ?? []);
lstInvoiceItem.value.push({

});

const querySearch = (queryString, cb) => {
  const results = queryString ? lstItem.value.filter(createFilter(queryString)) : lstItem.value;
  cb(results);
};

const createFilter = (queryString) => {
  return (item) => {
    return item.name.toLowerCase().indexOf(queryString.toLowerCase()) === 0;
  };
};

const loadAll = () => {
  if (running.value > 0) {
    return;
  }
  ++running.value;
  store.dispatch("item/getAll", null).then((res) => {
    if (res?.data?.data) {
      lstItem.value = res.data.data;
    }
    setTimeout(() => {
      running.value = 0;
    }, 1000);
  });
};

const handleSelect = (item) => {
  console.log(item);
};

const save = () => {
  emit("callback", []);
};

onMounted(() => {
  loadAll();
})
</script>

<template>
  <Dialog :title="t('Invoices')">
    <template #body>
      <el-table :data="lstInvoiceItem" style="height: 100%" :empty-text="t('NoData')">
        <el-table-column :label="t('ItemName')" min-width="150">
          <template #default="scope">
            <el-autocomplete
              v-model="scope.row.itemName"
              :fetch-suggestions="querySearch"
              clearable
              class="inline-input w-100"
              @select="handleSelect"
            />
          </template>
        </el-table-column>
        <el-table-column :label="t('Quantity')" width="200">
          <template #default="scope">
            <el-input-number v-model="scope.row.quantity" :min="0" :max="1000" />
          </template>
        </el-table-column>
        <el-table-column :label="t('Unit')" width="100">
          <template #default="scope">{{ scope.row.unit }}</template>
        </el-table-column>
        <el-table-column :label="t('SalePrice')" width="150">
          <template #default="scope">{{ scope.row.salePrice }}</template>
        </el-table-column>
        <el-table-column :label="t('Total')" width="150">
          <template #default="scope">{{ scope.row.total }}</template>
        </el-table-column>
        <el-table-column label="" width="70" fixed="right">
          <template #default="scope">
            <div class="d-flex flex-row-reverse">
              <el-button circle :icon="Delete" size="small" class="mr-2" @click="remove(scope.row.id)" type="danger"></el-button>
            </div>
          </template>
        </el-table-column>
      </el-table>
    </template>
    <template #foot>
      <div class="d-flex flex-row-reverse">
        <el-button type="primary" @click="save" class="ml-2">{{t("Save")}}</el-button>
        <el-button @click="toggleModel">{{ t("Close") }}</el-button>
      </div>
    </template>
  </Dialog>
</template>