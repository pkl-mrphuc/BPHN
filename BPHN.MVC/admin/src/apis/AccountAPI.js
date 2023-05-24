import axios from "axios"

class AccountAPI {
    constructor() {

    }

    async login(account) {
        let requestUrl = 'https://localhost:7166/api/accounts/login'
        let requestParam = account
        return await axios.post(requestUrl, requestParam)
    }

    async forgot(email) {
        let requestUrl = `https://localhost:7166/api/accounts/send-reset-password?username=${email}`
        return await axios.get(requestUrl)
        
    }
}

export default new AccountAPI()