import axios from "axios";

class ItemAPI {
    constructor() {

    }

    async getAll() {
        let requestUrl = `${process.env.VUE_APP_API_URL}/items`;
        return await axios.get(requestUrl);
    }

    async getInstance(id) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/items/get-instance?id=${id}`;
        return await axios.get(requestUrl);
    }

    async insert(data) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/items/insert`;
        let requestParam = data;
        return await axios.post(requestUrl, requestParam);
    }

    async update(data) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/items/update`;
        let requestParam = data;
        return await axios.post(requestUrl, requestParam);
    }
}

export default new ItemAPI();