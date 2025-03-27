<script setup>
import { District_Wards, Districts, Province_Districts, Provinces, Wards } from "@/const";
import { useI18n } from "vue-i18n";
import { ref, defineEmits } from "vue";
import useToggleModal from "@/register-components/actionDialog";

const { t } = useI18n();
const emits = defineEmits(["callback"]);
const { toggleModel } = useToggleModal();

const provinceId = ref(null);
const districtId = ref(null);
const wardId = ref(null);
const address = ref(null);
const lstProvinces = ref(Object.keys(Provinces).map(x => { return { id: parseInt(x), name: Provinces[parseInt(x)] } }));
const lstDistricts = ref([]);
const lstWards = ref([]);

const handleSelectProvince = () => {
    lstDistricts.value = Province_Districts[provinceId.value].map(x => { return { id: x, name: Districts[x] } });
    districtId.value = null;
    wardId.value = null;
};

const handleSelectDistrict = () => {
    lstWards.value = District_Wards[districtId.value].map(x => { return { id: x, name: Wards[x] } });
    wardId.value = null;
};

const save = () => {
    emits("callback", `${address.value},${Wards[wardId.value]},${Districts[districtId.value]},${Provinces[provinceId.value]}`);
    toggleModel();
};
</script>

<template>
    <Dialog :title="t('LocationForm')" :className="'w-xl-25'">
        <template #body>
            <div class="container">
                <div class="row">
                    <div class="col-12 col-sm-12 col-md-12">
                        <div class="row d-flex flex-row align-items-center">
                            <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">{{ t("Province") }}<span class="text-danger">(*)</span></div>
                            <div class="mb-2 col-12 col-sm-12 col-md-8">
                                <el-select v-model="provinceId" @change="handleSelectProvince" :placeholder="t('Province')" :no-data-text="t('NoData')" class="w-100">
                                    <el-option v-for="item in lstProvinces" :key="item.id" :label="item.name" :value="item.id" />
                                </el-select>
                            </div>
                        </div>
                        <div class="row d-flex flex-row align-items-center">
                            <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">{{ t("District") }}<span class="text-danger">(*)</span></div>
                            <div class="mb-2 col-12 col-sm-12 col-md-8">
                                <el-select v-model="districtId" @change="handleSelectDistrict" :placeholder="t('District')" :no-data-text="t('NoData')" class="w-100">
                                    <el-option v-for="item in lstDistricts" :key="item.id" :label="item.name" :value="item.id" />
                                </el-select>
                            </div>
                        </div>
                        <div class="row d-flex flex-row align-items-center">
                            <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">{{ t("Ward") }}<span class="text-danger">(*)</span></div>
                            <div class="mb-2 col-12 col-sm-12 col-md-8">
                                <el-select v-model="wardId" @change="handleSelectWard" :placeholder="t('Ward')" :no-data-text="t('NoData')" class="w-100">
                                    <el-option v-for="item in lstWards" :key="item.id" :label="item.name" :value="item.id" />
                                </el-select>
                            </div>
                        </div>
                        <div class="row d-flex flex-row align-items-center">
                            <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">{{ t("Address") }}<span class="text-danger">(*)</span></div>
                            <div class="mb-2 col-12 col-sm-12 col-md-8">
                                <el-input v-model="address" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </template>
        <template #foot>
            <div class="d-flex flex-row-reverse">
                <el-button type="primary" @click="save" class="ml-2">{{ t("Save") }}</el-button>
                <el-button @click="toggleModel">{{ t("Close") }}</el-button>
            </div>
        </template>
    </Dialog>
</template>

<style scoped></style>