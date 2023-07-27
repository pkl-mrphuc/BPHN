import axios from "axios";

class PitchAPI {
    constructor() {

    }

    async getInstance(id) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/pitchs/get-instance?id=${id}`;
            return await axios.get(requestUrl);
        } catch (error) {
            console.log(error);
            return false;
        }
    }

    async insert(data) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/pitchs/insert`;
            let requestParam = data;
            return await axios.post(requestUrl, requestParam);
        } catch (error) {
            console.log(error);
            return false;
        }
    }

    async update(data) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/pitchs/update`;
            let requestParam = data;
            return await axios.post(requestUrl, requestParam);
        } catch (error) {
            console.log(error);
            return false;
        }
    }

    async getPaging(data) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/pitchs/paging?pageIndex=${data.pageIndex}&pageSize=${data.pageSize}&txtSearch=&accountId=${data.accountId}&hasDetail=${data.hasDetail}&hasInactive=${data.hasInactive}`;
            return await axios.get(requestUrl);
        } catch (error) {
            console.log(error);
            return false;
        }
    }

    async getCountPaging(data) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/pitchs/count-paging?pageIndex=${data.pageIndex}&pageSize=${data.pageSize}&txtSearch=&accountId=${data.accountId}&hasInactive=${data.hasInactive}`;
            return await axios.get(requestUrl);
        } catch (error) {
            console.log(error);
            return false;
        }
    }
}

export default new PitchAPI();