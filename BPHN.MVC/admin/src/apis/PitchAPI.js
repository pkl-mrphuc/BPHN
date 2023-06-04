import axios from 'axios'

class PitchAPI {
    constructor() {

    }

    async getInstance(id) {
        try {
            let requestUrl = `https://localhost:7166/api/pitchs/get-instance?id=${id}`
            return await axios.get(requestUrl)
        } catch(error) {
            console.log(error)
            return false
        }
    }

    async insert(data) {
        try {
            let requestUrl = `https://localhost:7166/api/pitchs/insert` 
            let requestParam = data
            return await axios.post(requestUrl, requestParam)           
        } catch (error) {
            console.log(error)
            return false
        }
    }

    async getPaging(accountId) {
        try {
            let requestUrl = `https://localhost:7166/api/pitchs/paging?pageIndex=1&pageSize=1&txtSearch=&accountId=${accountId}` 
            return await axios.get(requestUrl) 
        } catch (error) {
            console.log(error)
            return false
        }
    }
}

export default new PitchAPI()