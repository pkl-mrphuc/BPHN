<script setup>

import { useI18n } from "vue-i18n";
import { ref } from 'vue';
import { Provinces } from "@/const";
import router from "@/routers";
import { useStore } from "vuex";
const { t } = useI18n();
const store = useStore();
// const selectedArea = ref('');
const isMobile = ref(store.getters["config/isMobile"]);
const provinceId = ref(null);
const lstProvinces = ref(Object.keys(Provinces).map(x => { return { id: parseInt(x), name: Provinces[parseInt(x)] } }));

const goTo = (link) => {
  router.push(link);
}
</script>
<template>
  <div class="container">
    <section :class="isMobile ? '' : 'p-4'" class="banner">
      <div class="w-100">
        <img src="../assets/images/football-field.png" alt="Football Field" class="banner-img" :height="500" />
      </div>
      <div :class="isMobile ? 'booking-box mobile' : 'booking-box'">
        <h1 class="fs-3 fw-bolder text-center m-0">Đặt sân nhanh chóng
          với vài bước đơn giản</h1>
        <p class="text-center">Hơn 100 sân bóng đang chờ đón những
          chiến binh sân cỏ</p>
        <div class="row" :style="isMobile?'':'padding: 0 50px'">
          <div class="col-12 col-sm-12 col-md-12 col-lg-9">
            <div class="mx-2">
              <el-select v-model="provinceId" :placeholder="t('Province')" :no-data-text="t('NoData')" size="large"
                class="w-100 mb-2">
                <el-option v-for="item in lstProvinces" :key="item.id" :label="item.name" :value="item.id" />
              </el-select>
            </div>
          </div>
          <div class="col-12 col-sm-12 col-md-12 col-lg-3">
            <div class="mx-2">
              <el-button type="primary" size="large" @click="goTo('/booking')" class="w-100 mb-2">Đặt sân ngay
              </el-button>
            </div>

          </div>
        </div>

      </div>
    </section>

    <section class="features">
      <h2 class="text-center bg-info p-2 text-white">Bạn là chủ sân bóng?</h2>
      <p class="m-0 fs-5 p-3">Tham gia ngay BPHN để tối đa hóa lợi nhuận, tiết kiệm thời gian quản lý!</p>
      <div class="features-list row">
        <div class="col-12 col-sm-12 col-md-12 col-lg-4">
          <div class="feature-item" :class="isMobile?'mx-2 mb-5':'mx-5'">
            <img src="../assets/images/table-icon.png" alt="Calendar"/>
            <h3 class="m-0 fs-4 text-center mb-3">Cho phép mọi người đặt sân</h3>
            <p class="m-0 fs-5 ">Khách hàng có thể đặt sân nhanh chóng, không lo trống giờ.</p>
          </div>
        </div>
        <div class="col-12 col-sm-12 col-md-12 col-lg-4">
          <div class="feature-item" :class="isMobile?'mx-2 mb-5':'mx-5'">
            <img src="../assets/images/pitch-icon.png" alt="Management" />
            <h3 class="m-0 fs-4 text-center mb-3">Quản lý sân hiệu quả hơn</h3>
            <p class="m-0 fs-5">Dễ dàng theo dõi lịch sân, doanh thu, khách hàng.</p>
          </div>
        </div>
        <div class="col-12 col-sm-12 col-md-12 col-lg-4">
          <div class="feature-item" :class="isMobile?'mx-2 mb-5':'mx-5'">
            <img src="../assets/images/invoice-icon.png" alt="Order" />
            <h3 class="m-0 fs-4 text-center mb-3">Tích hợp order thêm đồ uống</h3>
            <p class="m-0 fs-5">Tăng thêm lợi nhuận mà không cần tốn công quản lý.</p>
          </div>
        </div>
      </div>
      <div class="d-flex flex-row justify-content-center">
        <el-button type="primary" size="large" @click="goTo('/')">Đăng ký tài khoản chủ sân ngay</el-button>
      </div>
    </section>

    <section class="testimonials">
      <h2>Phản hồi từ cộng đồng về BPHN</h2>
      <div class="testimonial-list">
        <div class="testimonial">
          <p>"Từ ngày biết đến BPHN, mình không còn phải loay hoay tìm sân trống nữa!"</p>
          <strong>Lê Khắc Phúc - Đội trưởng CLB</strong>
        </div>
        <div class="testimonial">
          <p>"Hệ thống giúp mình quản lý sân dễ dàng hơn bao giờ hết!"</p>
          <strong>Nguyễn Lương Khang - Chủ sân bóng</strong>
        </div>
      </div>
    </section>

    <footer class="footer">
      <div class="footer-logo">BPHN</div>
      <p>Đặt sân trong 1 chạm - Thỏa đam mê bóng đá!</p>
      <div class="footer-links">
        <a href="#">Chính sách bảo mật</a>
        <a href="#">Điều khoản</a>
        <a href="#">Liên hệ</a>
      </div>
    </footer>
  </div>
</template>

<style scoped>
.banner {
  position: relative;
}

.banner-img {
  width: 100%;
  border-radius: 20px;
  object-fit: cover;
}

.booking-box {
  width: 75%;
  background: #ffffff;
  padding: 60px 60px 20px 60px;
  border-radius: 30px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  position: absolute;
  bottom: -150px;
  left: 50%;
  transform: translate(-50%, -50%);
}

.booking-box.mobile {
  width: 90%;
}

.features {
  margin-top: 125px;
}

.feature-item {
  background-color: #f2f2f2;
  border-radius: 25px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  padding: 30px;
}

.feature-item img {
  width: 75%;
  margin-top: -30px;
}

.features-list{
  /* padding: 50px; */
}
</style>
