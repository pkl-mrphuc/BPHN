<script setup>
import { useI18n } from "vue-i18n";

const { t } = useI18n();
</script>


<template>
  <section>
    <div class="container">
      <div
        class="row mb-3 d-flex flex-row align-items-center justify-content-between"
      >
        <h3 class="fs-3 col-12 col-sm-12 col-md-12 col-lg-8">
          {{ t("Services") }}
        </h3>
        <div class="col-12 col-sm-12 col-md-12 col-lg-4 d-flex flex-row">
        </div>
      </div>
    </div>
  </section>
</template>