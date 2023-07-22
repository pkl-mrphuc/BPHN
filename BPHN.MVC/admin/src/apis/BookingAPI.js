import axios from "axios";

class BookingAPI {
    constructor() {

    }

    async getInstance(id) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/bookings/get-instance?id=${id}`;
        return await axios.get(requestUrl);
    }

    async insert(data) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/bookings/insert`;
        let requestParam = data;
        return await axios.post(requestUrl, requestParam);
    }

    async checkTimeFrame(data) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/bookings/check-time-frame`;
        let requestParam = data;
        return await axios.post(requestUrl, requestParam);
    }

    async getPaging(data) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/bookings/paging?pageIndex=${data.pageIndex}&pageSize=${data.pageSize}&txtSearch=${data.txtSearch}&hasBookingDetail=${data.hasBookingDetail}`;
        return await axios.get(requestUrl);
    }

    async getCountPaging(data) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/bookings/count-paging?pageIndex=${data.pageIndex}&pageSize=${data.pageSize}&txtSearch=${data.txtSearch}`;
        return await axios.get(requestUrl);
    }

    async findBlank(data) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/bookings/find-blank`;
        let requestParam = data;
        return axios.post(requestUrl, requestParam);
    }

    async approval(id) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/bookings/approval/${id}`;
        return axios.post(requestUrl);
    }

    async decline(id) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/bookings/decline/${id}`;
        return axios.post(requestUrl);
    }
}

export default new BookingAPI();