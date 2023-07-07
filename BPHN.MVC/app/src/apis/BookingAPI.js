import axios from "axios";

class BookingAPI {
    constructor() {

    }

    async getInstance(id) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/bookings/get-instance?id=${id}`;
        return await axios.get(requestUrl);
    }

    async insertBookingRequest(data) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/bookings/insert-booking-request`;
        let requestParam = data;
        return await axios.post(requestUrl, requestParam);
    }

}

export default new BookingAPI();