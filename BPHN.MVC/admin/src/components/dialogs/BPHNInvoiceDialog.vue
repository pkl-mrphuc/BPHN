<script setup>
import useToggleModal from "@/register-components/actionDialog";
import { defineProps, ref, onMounted, defineEmits } from "vue";
import { useI18n } from "vue-i18n";
import { Delete, Plus } from "@element-plus/icons-vue";
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
const currentRow = ref(null);
const lstRow = ref(props.data?.items ?? [{
  id: 1,
  itemName: "",
  unit: "",
  quantity: "",
  salePrice: "",
  total: ""
}]);

const querySearch = (queryString, cb) => {
  const results = queryString ? lstItem.value.filter(x => x.value.toLowerCase().indexOf(queryString.toLowerCase()) === 0) : lstItem.value;
  cb(results);
};

const loadAll = () => {
  if (running.value > 0) {
    return;
  }
  ++running.value;

  setTimeout(() => {
    running.value = 0;
  }, 1000);

  store.dispatch("item/getAll", null).then((res) => {
    if (res?.data?.data) {
      for (let i = 0; i < res.data.data.length; i++) {
        const element = res.data.data[i];
        lstItem.value.push({
          value: element.name,
          id: element.id,
          unit: element.unit,
          salePrice: element.salePrice
        });
      }
    }
  });
};

const handleSelect = (item) => {
  currentRow.value.itemName = item.value;
  currentRow.value.unit = item.unit;
  currentRow.value.salePrice = item.salePrice;
  currentRow.value.total = item.salePrice * currentRow.value.quantity;
};

const setItem = (row) => {
  currentRow.value = row;
  lstRow[row.id] = currentRow.value;
};

const save = () => {
  emit("callback", []);
};

const handleChange = (row) => {
  row.total = row.salePrice * row.quantity;
};

const add = (row) => {
  console.log(row.id);
  lstRow.value.push(defaultRow());
};

const remove = (row) => {
  lstRow.value = lstRow.value.filter(x => x.id != row.id);
  if (lstRow.value.length == 0) {
    lstRow.value.push(defaultRow());
  }
};

const defaultRow = () => {
  return {
    id: Math.max(...lstRow.value.map(x => x.id)) + 1,
    itemName: "",
    unit: "",
    quantity: "",
    salePrice: "",
    total: ""
  };
}

onMounted(() => {
  loadAll();
});

</script>

<template>
  <Dialog :title="t('Invoices')">
    <template #body>
      <el-table :data="lstRow" style="height: 100%" :empty-text="t('NoData')">
        <el-table-column label="" width="50">
          <template #default="scope">
            <el-button circle :icon="Plus" size="small" @click="add(scope.row)" type="secondary"></el-button>
          </template>
        </el-table-column>
        <el-table-column :label="t('ItemName')" min-width="150">
          <template #default="scope">
            <el-autocomplete v-model="scope.row.itemName" @focus="setItem(scope.row)" @select="handleSelect"
              :fetch-suggestions="querySearch" clearable class="inline-input w-100" />
          </template>
        </el-table-column>
        <el-table-column :label="t('Quantity')" width="200">
          <template #default="scope">
            <el-input-number @change="handleChange(scope.row)" v-model="scope.row.quantity" :min="0" :max="1000" />
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
              <el-button circle :icon="Delete" size="small" class="mr-2" @click="remove(scope.row)"
                type="danger"></el-button>
            </div>
          </template>
        </el-table-column>
      </el-table>
    </template>
    <template #foot>
      <div class="d-flex flex-row-reverse">
        <el-button type="primary" @click="save" class="ml-2">{{ t("Save") }}</el-button>
        <el-button @click="toggleModel">{{ t("Close") }}</el-button>
      </div>
    </template>
  </Dialog>
</template>