<script setup></script>

<script setup>
import { ref } from 'vue';

const selectedArea = ref('');
</script>
<template>
  <div class="container">
    <section class="banner">
        <div class="w-100">
            <img src="../assets/images/football-field.png" alt="Football Field" class="banner-img" />
        </div>
      <div class="d-flex flex-row w-100 justify-content-center">
            <div class="booking-box">
                <h1 class="fs-1 m-3 fw-bolder">Đặt sân nhanh chóng chỉ với vài bước đơn giản</h1>
                <p class="fs-5 m-3">Hơn 100 sân bóng đang chờ đón những chiến binh sân cỏ</p>
                <div class="d-flex justify-content-center">
                    <el-select v-model="selectedArea" size="large" class="w-50 m-2">
                        <option value="">Chọn khu vực đặt sân</option>
                        <option value="hanoi">Hà Nội</option>
                        <option value="hcm">TP. Hồ Chí Minh</option>
                    </el-select>
                    <el-button type="primary" size="large" class="m-2">Đặt sân ngay</el-button>
                </div>
            </div>
        </div>
    </section>

    <section class="features">
      <h2>Bạn là chủ sân bóng?</h2>
      <p>Tham gia ngay BPHN để tối đa hóa lợi nhuận, tiết kiệm thời gian quản lý!</p>
      <div class="features-list">
        <div class="feature-item">
          <img src="../assets/images/football-field.png" alt="Calendar" />
          <h3>Cho phép mọi người đặt sân</h3>
          <p>Khách hàng có thể đặt sân nhanh chóng, không lo trống giờ.</p>
        </div>
        <div class="feature-item">
          <img src="../assets/images/football-field.png" alt="Management" />
          <h3>Quản lý sân hiệu quả hơn</h3>
          <p>Dễ dàng theo dõi lịch sân, doanh thu, khách hàng.</p>
        </div>
        <div class="feature-item">
          <img src="../assets/images/football-field.png" alt="Order" />
          <h3>Tích hợp order thêm đồ uống</h3>
          <p>Tăng thêm lợi nhuận mà không cần tốn công quản lý.</p>
        </div>
      </div>
      <button class="register-owner">Đăng ký tài khoản chủ sân ngay</button>
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
  text-align: center;
  padding: 50px;
}
.banner-img {
  width: 100%;
  height: 60vh;
  border-radius: 20px;
  object-fit: cover;
}
.booking-box {
    width: 75%;
    background: #ffffff;
    padding: 20px;
    border-radius: 30px;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
    position: relative;
    top: -150px;
}
.features {
  text-align: center;
  padding: 50px;
  background: #f9f9f9;
}
.features-list {
  display: flex;
  justify-content: space-around;
}
.testimonials {
  padding: 50px;
  text-align: center;
}
.footer {
  background: #222;
  color: white;
  text-align: center;
  padding: 20px;
}
</style>
