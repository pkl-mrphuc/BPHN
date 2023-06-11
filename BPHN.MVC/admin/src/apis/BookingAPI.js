import axios from "axios";

class BookingAPI {
    constructor() {

    }

    async getInstance(id) {
        let requestUrl = `https://localhost:7166/api/bookings/get-instance?id=${id}`;
        return await axios.get(requestUrl);
    }

    async insert(data) {
        let requestUrl = `https://localhost:7166/api/bookings/insert`;
        let requestParam = data;
        return await axios.post(requestUrl, requestParam);
    }

    async checkTimeFrame(data) {
        let requestUrl = `https://localhost:7166/api/bookings/check-time-frame`;
        let requestParam = data;
        return await axios.post(requestUrl, requestParam);
    }

    async getPaging(data) {
        let requestUrl = `https://localhost:7166/api/bookings/paging?pageIndex=${data.pageIndex}&pageSize=${data.pageSize}&txtSearch=${data.txtSearch}&hasBookingDetail=${data.hasBookingDetail}`;
        return await axios.get(requestUrl);
    }

    async getCountPaging(data) {
        let requestUrl = `https://localhost:7166/api/bookings/count-paging?pageIndex=${data.pageIndex}&pageSize=${data.pageSize}&txtSearch=${data.txtSearch}`;
        return await axios.get(requestUrl);
    }

    async find(data) {
        let requestUrl = `https://localhost:7166/api/bookings/find`;
        let requestParam = data;
        return axios.post(requestUrl, requestParam);
    }
}

export default new BookingAPI();