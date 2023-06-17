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

    async resetPassword(data) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/accounts/submit-reset-password`;
            let requestParam = data;
            return await axios.post(requestUrl, requestParam);
        } catch (error) {
            console.log(error);
            return false;
        }
    }
}

export default new AccountAPI();