<script setup>
import useToggleModal from "@/register-components/actionDialog";
import { ColdDrink } from "@element-plus/icons-vue";
import { useI18n } from "vue-i18n";
import { ref, defineProps, inject, defineEmits } from "vue";
import { StatusEnum } from "@/const";
import MaskNumberInput from "@/components/MaskNumberInput.vue";
import { ElLoading, ElNotification } from "element-plus";
import { useStore } from "vuex";

const props = defineProps({
    data: Object,
    mode: String,
});
const emit = defineEmits(["callback"]);
const { toggleModel } = useToggleModal();
const { t } = useI18n();
const store = useStore();
const loadingOptions = inject("loadingOptions");
const name = ref(props.data?.name);
const code = ref(props.data?.code);
const purchasePrice = ref(props.data?.purchasePrice ?? 0);
const salePrice = ref(props.data?.salePrice ?? 0);
const quantity = ref(props.data?.quantity ?? 0);
const unit = ref(props.data?.unit ?? "");
const status = ref(props.data?.status ?? StatusEnum.ACTIVE);
const running = ref(0);

const save = () => {
    if (running.value > 0) return;
    ++running.value;

    setTimeout(() => {
        running.value = 0;
    }, 1000);

    if (!code.value) {
        ElNotification({ title: t("Notification"), message: t("CodeItemEmptyMesg"), type: "warning" });
        return;
    }
    if (!name.value) {
        ElNotification({ title: t("Notification"), message: t("NameItemEmptyMesg"), type: "warning" });
        return;
    }

    const loading = ElLoading.service(loadingOptions);
    let actionPath = "item/insert";
    if (props.mode == "edit") actionPath = "item/update";
    store.dispatch(actionPath, 
    {
        id: props.data?.id,
        name: name.value,
        code: code.value,
        quantity: quantity.value,
        status: status.value,
        salePrice: salePrice.value,
        purchasePrice: purchasePrice.value,
        unit: unit.value
    })
    .then((res) => {
        loading.close();
        if (res?.data?.success) {
            emit("callback", res);
            toggleModel();
            ElNotification({ title: t("Notification"), message: t("SaveSuccess"), type: "success" });
        } else {
            ElNotification({ title: t("Notification"), message: t("ErrorMesg"), type: "error" });
        }
    });
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
                                <el-input v-model="code" maxlength="36" />
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
                            <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">{{ t("Unit") }}</div>
                            <div class="mb-2 col-12 col-sm-12 col-md-8">
                                <el-input v-model="unit" maxlength="255" />
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
