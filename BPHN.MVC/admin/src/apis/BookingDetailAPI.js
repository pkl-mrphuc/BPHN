import axios from "axios";

class BookingDetailAPI {
    constructor() {

    }

    async cancel(id) {
        try {
            let requestUrl = `https://localhost:7166/api/bookingdetails/cancel/${id}`;
            return axios.post(requestUrl);
        } catch (error) {
            console.log(error);
            return false;
        }
    }

    async getByDate(date) {
        try {
            let requestUrl = `https://localhost:7166/api/bookingdetails/${date}`;
            return axios.get(requestUrl);
        } catch (error) {
            console.log(error);
            return false;
        }
    }
}

export default new BookingDetailAPI();