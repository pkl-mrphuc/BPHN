import axios from "axios";

class FileAPI {
    constructor() {

    }

    async upload(data) {
        try {
            if(!data) return false;
            let requestUrl = `${process.env.VUE_APP_API_URL}/files/upload/${data.id}`;
            let formData = new FormData();
            formData.append("file", data.file);
            return await axios.post(requestUrl, formData);
        } catch (error) {
            console.log(error);
            return false;
        }

    }
}

export default new FileAPI();