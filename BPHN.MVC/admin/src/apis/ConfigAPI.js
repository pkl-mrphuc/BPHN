import axios from "axios"

class ConfigAPI {
    constructor() {

    }

    async getConfigs(keys) {
        try {
            let requestUrl = `https://localhost:7166/api/configs?key=${keys}`
            return await axios.get(requestUrl)
        } catch (error) {
           return false 
        }
    }

    async save(configs) {
        try {
            let requestUrl = `https://localhost:7166/api/configs/save`
            let requestParam =  configs
            return await axios.post(requestUrl, requestParam)            
        } catch (error) {
            return false
        }
    }
}

export default new ConfigAPI()