import axios from "axios";

class PermissionAPI {
    constructor() {

    }

    async getPermissions(accountId) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/permissions/${accountId}`;
            return await axios.get(requestUrl);
        } catch (error) {
            console.log(error);
            return false;
        }
    }

    async save(data) {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/permissions/save/${data.accountId}`;
            let requestParam = data.permissions;
            return await axios.post(requestUrl, requestParam);
        } catch (error) {
            console.log(error);
            return false;
        }
    }
}

export default new PermissionAPI();