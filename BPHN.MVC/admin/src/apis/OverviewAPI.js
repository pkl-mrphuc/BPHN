import axios from "axios";

class OverviewAPI {
    constructor() {

    }

    async get(data) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/overview`;
        let requestParam = data;
        return await axios.post(requestUrl, requestParam);
    }
}

export default new OverviewAPI();