import axios from "axios"

class StadiumAPI {
    constructor() {

    }

    async fetchStadium(key) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/pitchs/paging?pageIndex=1&pageSize=100&txtSearch=${key}&hasDetail=false&hasInactive=false`
            return await axios.get(requestUrl)
        } catch (error) {
            console.log(error)
            return false
        }
    }
}

export default new StadiumAPI()