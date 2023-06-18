import axios from "axios";

class HistoryLogAPI {
    constructor() {

    }

    async getPaging(data) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/historylogs/paging?pageIndex=${data.pageIndex}&pageSize=${data.pageSize}&txtSearch=${data.txtSearch}`;
        return await axios.get(requestUrl);
    }

    async getCountPaging(data) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/historylogs/count-paging?pageIndex=${data.pageIndex}&pageSize=${data.pageSize}&txtSearch=${data.txtSearch}`;
        return await axios.get(requestUrl);
    }
}

export default new HistoryLogAPI();