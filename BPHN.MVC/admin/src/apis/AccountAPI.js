import axios from "axios"

class AccountAPI {
    constructor() {

    }

    async login(account) {
        try {
            let requestUrl = 'https://localhost:7166/api/accounts/login'
            let requestParam = account
            return await axios.post(requestUrl, requestParam)
        } catch (error) {
            return false   
        }
    }

    async forgot(email) {
        try {
            let requestUrl = `https://localhost:7166/api/accounts/send-reset-password?username=${email}`
            return await axios.get(requestUrl)
        } catch (error) {
            return false   
        }
    }

    async validateToken(token) {
        try {
            let requestUrl = `https://localhost:7166/api/accounts/validate-token?token=${token}`
            return await axios.get(requestUrl)
        } catch (error) {
            return false
        }
    }
}

export default new AccountAPI()