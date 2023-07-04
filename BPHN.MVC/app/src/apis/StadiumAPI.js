import axios from "axios"

class StadiumAPI {
    constructor() {

    }

    async getPaging(data) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/pitchs/paging?pageIndex=${data.pageIndex}&pageSize=100&txtSearch=${data.txtSearch}&hasDetail=true&hasInactive=false`
            return await axios.get(requestUrl)
        } catch (error) {
            console.log(error)
            return false
        }
    }

    async getCountPaging(data) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/pitchs/count-paging?pageIndex=${data.pageIndex}&pageSize=100&txtSearch=${data.txtSearch}&hasInactive=false`;
        return await axios.get(requestUrl);
    }
}

export default new StadiumAPI()