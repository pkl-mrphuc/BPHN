<script setup>
import useToggleModal from "@/register-components/actionDialog";
import { ColdDrink } from "@element-plus/icons-vue";
import { useI18n } from "vue-i18n";
import { ref, defineProps } from "vue";
import { StatusEnum } from "@/const";
import MaskNumberInput from "@/components/MaskNumberInput.vue";

const props = defineProps({
    data: Object,
    mode: String,
});
const { toggleModel } = useToggleModal();
const { t } = useI18n();
const name = ref(props.data?.name);
const code = ref(props.data?.code);
const purchasePrice = ref(props.data?.purchasePrice ?? 0);
const salePrice = ref(props.data?.salePrice ?? 0);
const quantity = ref(props.data?.quantity ?? 0);
const status = ref(props.data?.status ?? StatusEnum.ACTIVE);

const save = () => {
    console.log("x");
};
</script>

<template>
    <Dialog :title="t('ServiceForm')">
        <template #body>
            <div class="container">
                <div class="row">
                    <div class="d-flex flex-row justify-content-center mb-3 col-12 col-sm-12 col-md-4">
                        <el-avatar :size="120"><el-icon :size="50"><ColdDrink /></el-icon></el-avatar>
                    </div>
                    <div class="col-12 col-sm-12 col-md-8">
                        <div class="row">
                            <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">{{ t("Status") }}</div>
                            <div class="col-12 col-sm-12 col-md-8">
                                <el-select v-model="status" class="w-100 mb-2">
                                    <el-option :label="t('Active')" :value="StatusEnum.ACTIVE" />
                                    <el-option :label="t('Inactive')" :value="StatusEnum.INACTIVE" />
                                </el-select>
                            </div>
                        </div>
                        <div class="row">
                            <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">{{ t("Code") }}
                                <span class="text-danger">(*)</span>
                            </div>
                            <div class="mb-2 col-12 col-sm-12 col-md-8">
                                <el-input v-model="code" maxlength="255" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">{{ t("Name") }}
                                <span class="text-danger">(*)</span>
                            </div>
                            <div class="mb-2 col-12 col-sm-12 col-md-8">
                                <el-input v-model="name" maxlength="255" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">{{ t("PurchasePrice") }}</div>
                            <div class="mb-2 col-12 col-sm-12 col-md-8">
                                <mask-number-input :value="purchasePrice" @value="(value) => { purchasePrice = value; }"></mask-number-input>
                            </div>
                        </div>
                        <div class="row">
                            <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">{{ t("SalePrice") }}</div>
                            <div class="mb-2 col-12 col-sm-12 col-md-8">
                                <mask-number-input :value="salePrice" @value="(value) => { salePrice = value; }"></mask-number-input>
                            </div>
                        </div>
                        <div class="row">
                            <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">{{ t("Quantity") }}</div>
                            <div class="mb-2 col-12 col-sm-12 col-md-8">
                                <el-input-number class="w-100" v-model="quantity" :min="0" :max="1000"/>
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
