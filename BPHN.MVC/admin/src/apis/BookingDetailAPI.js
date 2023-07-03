import axios from "axios";

class BookingDetailAPI {
    constructor() {

    }

    async cancel(id) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/bookingdetails/cancel/${id}`;
            return axios.post(requestUrl);
        } catch (error) {
            console.log(error);
            return false;
        }
    }

    async getByDate(date) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/bookingdetails/${date}`;
            return axios.get(requestUrl);
        } catch (error) {
            console.log(error);
            return false;
        }
    }

    async updateMatch(data) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/bookingdetails/update-match`;
            let requestParam = data;
            return axios.post(requestUrl, requestParam);
        } catch (error) {
            console.log(error);
            return false;
        }
    }
}

export default new BookingDetailAPI();