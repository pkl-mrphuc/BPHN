import axios from "axios"

class ConfigAPI {
    constructor() {

    }

    async getConfigs(keys) {
        let requestUrl = `https://localhost:7166/api/configs?key=${keys}`
        return await axios.get(requestUrl)
    }
}

export default new ConfigAPI()