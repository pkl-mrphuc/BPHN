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
}

export default new BookingAPI();