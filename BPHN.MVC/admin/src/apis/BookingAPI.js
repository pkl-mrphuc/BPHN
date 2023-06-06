import axios from "axios";

class BookingAPI {
    constructor() {

    }

    async getInstance(id) {
        let requestUrl = `https://localhost:7166/api/bookings/get-instance?id=${id}`;
        return await axios.get(requestUrl);
    }
}

export default new BookingAPI();