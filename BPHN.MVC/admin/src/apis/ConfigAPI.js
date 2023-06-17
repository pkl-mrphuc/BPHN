import axios from "axios";

class ConfigAPI {
    constructor() {

    }

    async getConfigs(keys) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/configs?key=${keys}`;
            return await axios.get(requestUrl);
        } catch (error) {
            console.log(error);
            return false;
        }
    }

    async save(configs) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/configs/save`;
            let requestParam = configs;
            return await axios.post(requestUrl, requestParam);
        } catch (error) {
            console.log(error);
            return false;
        }
    }
}

export default new ConfigAPI();