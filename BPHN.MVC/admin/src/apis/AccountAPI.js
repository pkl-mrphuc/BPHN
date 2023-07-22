import axios from "axios";

class AccountAPI {
    constructor() {

    }

    async login(account) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/accounts/login`;
            let requestParam = account;
            return await axios.post(requestUrl, requestParam);
        } catch (error) {
            console.log(error);
            return false;
        }
    }

    async forgot(email) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/accounts/send-reset-password?username=${email}`;
            return await axios.get(requestUrl);
        } catch (error) {
            console.log(error);
            return false;
        }
    }

    async validateToken(token) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/accounts/validate-token?token=${token}`;
            return await axios.get(requestUrl);
        } catch (error) {
            console.log(error);
            return false;
        }
    }

    async setPassword(data) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/accounts/submit-set-password`;
            let requestParam = data;
            return await axios.post(requestUrl, requestParam);
        } catch (error) {
            console.log(error);
            return false;
        }
    }

    async changePassword(data) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/accounts/change-password`;
            let requestParam = data;
            return await axios.post(requestUrl, requestParam);
        } catch (error) {
            console.log(error);
            return false;
        }
    }

    async getPaging(data) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/accounts/paging?pageIndex=${data.pageIndex}&pageSize=${data.pageSize}&txtSearch=${data.txtSearch}`;
            return await axios.get(requestUrl);
        } catch (error) {
            console.log(error);
            return false;
        }

    }

    async getCountPaging(data) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/accounts/count-paging?pageIndex=${data.pageIndex}&pageSize=${data.pageSize}&txtSearch=${data.txtSearch}`;
            return await axios.get(requestUrl);
        } catch (error) {
            console.log(error);
            return false;
        }

    }

    async register(data) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/accounts/register`;
            let requestParam = data;
            return await axios.post(requestUrl, requestParam);
        } catch (error) {
            console.log(error);
            return false;
        }

    }

    async getInstance(id) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/accounts/get-instance?id=${id}`;
        return await axios.get(requestUrl);
    }

    async refresh() {
        let requestUrl = `${process.env.VUE_APP_API_URL}/accounts/refresh`;
        return await axios.get(requestUrl);
    }
}

export default new AccountAPI();