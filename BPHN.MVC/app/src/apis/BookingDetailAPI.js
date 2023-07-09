import axios from "axios";

class BookingDetailAPI {
    constructor() {

    }

    async getByDate(data) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/bookingdetails?startDate=${data.startDate}&endDate=${data.endDate}&pitchId=${data.pitchId}&nameDetail=${data.nameDetail}`;
            return axios.get(requestUrl);
        } catch (error) {
            console.log(error);
            return false;
        }
    }
}

export default new BookingDetailAPI();