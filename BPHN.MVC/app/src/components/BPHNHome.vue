<script setup>
import { ref } from 'vue'; 
import { useI18n } from "vue-i18n";
import { Provinces } from "@/const";
import router from "@/routers";

const { t } = useI18n();
const provinceId = ref(null);
const lstProvinces = ref(
  Object.keys(Provinces).map(x => ({
    id: parseInt(x),
    name: Provinces[parseInt(x)]
  }))
);

const goTo = (link) => {
  router.push(link);
};

const showLeftBanner = ref(true);
const showRightBanner = ref(true);
</script>

<template>
  <div class="container limit">
    <div class="ad-banner left" v-show="showLeftBanner">
      <button class="close-btn" @click="showLeftBanner = false">X</button>
      <img src="../assets/images/ad-banner.gif" alt="Left Ad" />
    </div>

    <div class="ad-banner right" v-show="showRightBanner">
      <button class="close-btn" @click="showRightBanner = false">X</button>
      <img src="../assets/images/ad-banner.gif" alt="Right Ad" loading="lazy" />
    </div>

    <section class="p-4 banner">
      <div class="w-100">
        <img src="../assets/images/football-field.png" alt="Football Field" class="banner-img" height="500"/>
      </div>
      <div class="booking-box">
        <h1 class="fs-3 fw-bolder text-center m-0">Đặt sân nhanh chóng
          với vài bước đơn giản</h1>
        <p class="text-center">Hơn 100 sân bóng đang chờ đón những
          chiến binh sân cỏ</p>
        <div class="row" style="padding: 0 50px">
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
      <p class="m-0 fs-4" style="padding:1em;">Bạn là chủ sân bóng và muốn
        tối
        đa hóa lợi nhuận, tiết kiệm thời gian
        quản lý?</p>
      <div class="features-list row p-5">
        <div class="col-12 col-sm-12 col-md-12 col-lg-4">
          <div class="feature-item mx-5" data-aos="flip-right"
            data-aos-duration="200">
            <img src="../assets/images/table-icon.png" alt="Calendar" />
            <h3 class="m-0 fs-4 text-center mb-3">Cho phép mọi người đặt sân</h3>
            <p class="m-0 fs-5 ">Khách hàng đặt sân nhanh chóng, không lo trống giờ.</p>
          </div>
        </div>
        <div class="col-12 col-sm-12 col-md-12 col-lg-4">
          <div class="feature-item mx-5" data-aos="flip-left"
            data-aos-duration="600">
            <img src="../assets/images/pitch-icon.png" alt="Management" />
            <h3 class="m-0 fs-4 text-center mb-3">Quản lý sân bóng hiệu quả hơn</h3>
            <p class="m-0 fs-5">Dễ dàng theo dõi lịch sân, doanh thu, khách hàng.</p>
          </div>
        </div>
        <div class="col-12 col-sm-12 col-md-12 col-lg-4">
          <div class="feature-item mx-5" data-aos="flip-right"
            data-aos-duration="1400">
            <img src="../assets/images/invoice-icon.png" alt="Order" />
            <h3 class="m-0 fs-4 text-center mb-3">Tích hợp order thêm đồ uống</h3>
            <p class="m-0 fs-5">Tăng thêm lợi nhuận mà không cần tốn công quản lý.</p>
          </div>
        </div>
      </div>
      <div class="d-flex flex-row justify-content-center mt-4">
        <el-button type="primary" size="large" @click="goTo('/')">Đăng ký tài khoản chủ sân ngay</el-button>
      </div>
    </section>

    <section class="testimonials">
      <h2 class="text-center bg-info p-2 text-white">Phản hồi từ cộng đồng về BPHN</h2>
      <div class="testimonial-list row p-1">
        <div class="col-12 col-sm-12 col-md-12 col-lg-4">
          <div class="testimonial" data-aos="fade-up" data-aos-duration="200">
            <div>
              <p style="font-family: 'Times New Roman'; font-size: 5em; font-weight: bold; margin: -15px; padding: 0;">
                &ldquo;
              </p>
              <p class="fs-5">Từ ngày biết đến BPHN, mình không còn phải loay hoay tìm sân trống nữa. Đặt sân nhanh
                chóng, có thể xem
                lịch trống theo thời gian thực, cực kỳ tiện lợi!</p>
            </div>
            <div>
              <strong class="m-0 p-0">Nguyễn Minh Khoa</strong>
              <p class="m-0 p-0">Đội trưởng CLB</p>
            </div>
          </div>
        </div>
        <div class="col-12 col-sm-12 col-md-12 col-lg-4">
          <div class="testimonial" data-aos="fade-up" data-aos-duration="600">
            <div>
              <p style="font-family: 'Times New Roman'; font-size: 5em; font-weight: bold; margin: -15px; padding: 0;">
                &ldquo;
              </p>
              <p class="fs-5">Hệ thống giúp mình quản lý sân dễ dàng hơn bao giờ hết! Không còn tình trạng chồng lịch
                hay bỏ lỡ
                khách.</p>
            </div>
            <div>
              <strong class="m-0 p-0">Trần Anh Dũng</strong>
              <p class="m-0 p-0">Chủ sân bóng</p>
            </div>
          </div>
        </div>
        <div class="col-12 col-sm-12 col-md-12 col-lg-4">
          <div class="testimonial" data-aos="fade-up" data-aos-duration="1200">
            <div>
              <p style="font-family: 'Times New Roman'; font-size: 5em; font-weight: bold; margin: -15px; padding: 0;">
                &ldquo;
              </p>
              <p class="fs-5">Mình xin chia sẻ thật lòng là hệ thống này khá chất lượng! Tìm sân nhanh, đặt sân dễ nói
                chung là cũng
                chim ưng</p>
            </div>
            <div>
              <strong class="m-0 p-0">Lê Hoàng Phúc</strong>
              <p class="m-0 p-0">Chủ sân bóng</p>
            </div>
          </div>
        </div>
      </div>
    </section>
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

.testimonial {
  background-color: #f2f2f2;
  border-radius: 25px;
  padding: 25px;
  min-height: 250px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  margin: 25px;
}
</style>
