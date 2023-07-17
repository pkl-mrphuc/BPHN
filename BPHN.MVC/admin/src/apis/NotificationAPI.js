import axios from "axios";

class NotificationAPI {
    constructor() {

    }

    async getNotifications() {
        try {
            let requestUrl = `${process.env.VUE_APP_API_URL}/notifications/top-5`;
            return await axios.get(requestUrl);
        } catch (error) {
            console.log(error);
            return false;
        }
    }
}

export default new NotificationAPI();