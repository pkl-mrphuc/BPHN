<!-- Footer.vue -->
<template>
    <section>
        <footer class="footer m-0">
            <div class="container">
                <div class="footer-content">
                    <!-- Cột 1: Logo và mô tả -->
                    <div class="footer-column">
                        <div class="logo">
                            <img src="../assets/images/logo.png" alt="Bóng đá Icon" />
                            <h3>BPHN</h3>
                        </div>
                        <p>Đặt sân trực tuyến 1 chạm <br>Thỏa đam mê bóng đá</p>
                    </div>

                    <!-- Cột 2: Giới thiệu -->
                    <div class="footer-column">
                        <h4>Giới thiệu</h4>
                        <ul>
                            <li>Chính sách bảo mật</li>
                            <li>Điều khoản và điều kiện</li>
                            <li>Ngân hàng liên kết</li>
                            <li>Sự kiện</li>
                            <li>Về chúng tôi</li>
                        </ul>
                    </div>

                    <!-- Cột 3: Liên hệ -->
                    <div class="footer-column">
                        <h4>Liên hệ</h4>
                        <ul>
                            <li>Facebook: fb/bphn</li>
                            <li>Instagram: ig_bphn</li>
                            <li>HOTLINE: 0988 686 686</li>
                            <li>Email: bphn@gmail.com</li>
                        </ul>
                    </div>

                    <!-- Cột 4: Hỗ trợ -->
                    <div class="footer-column">
                        <h4>Hỗ trợ</h4>
                        <ul>
                            <li>Hướng dẫn đặt sân kỹ thuật số</li>
                            <li>Hướng dẫn nạp tiền</li>
                            <li>Câu hỏi thường gặp</li>
                        </ul>
                    </div>
                </div>
            </div>
        </footer>
    </section>
</template>

<script>
export default {
    name: 'BPHNFooter',
};
</script>

<style scoped>
.footer {
    background-color: #2f2f2f;
    color: white;
    padding: 40px 0;
    font-family: Arial, sans-serif;    
}

.container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 20px;
}

.footer-content {
    display: flex;
    justify-content: space-between;
    flex-wrap: wrap;
    gap: 20px;
}

.footer-column {
    flex: 1;
    min-width: 200px;
}

.logo {
    display: flex;
    align-items: center;
    gap: 10px;
    margin-bottom: 10px;
}

.logo img {
    width: 30px;
    height: 30px;
}

.logo h3 {
    font-size: 24px;
    margin: 0;
}

.footer-column p {
    font-size: 14px;
    line-height: 1.5;
}

.footer-column h4 {
    font-size: 18px;
    margin-bottom: 15px;
}

.footer-column ul {
    list-style: none;
    padding: 0;
}

.footer-column ul li {
    font-size: 14px;
    margin-bottom: 10px;
    line-height: 1.5;
}

@media (max-width: 768px) {
    .footer-content {
        flex-direction: column;
        text-align: center;
    }

    .logo {
        justify-content: center;
    }
}
</style>