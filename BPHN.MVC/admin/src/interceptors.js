import axios from "axios"

export const jwtInterceptor = (() => {
    axios.interceptors.request.use((request) => {

        if (request.url &&
            (
                request.url.includes("/login") ||
                request.url.includes("/forgot") ||
                request.url.includes("/send-reset-password") ||
                request.url.includes("/submit-set-password")
            )) {
            return request
        }

        const authKey = JSON.parse(localStorage.getItem("admin-auth-key"))
        if (authKey?.account?.context?.token) {
            request.headers["Authorization"] = `Bearer ${authKey?.account?.context?.token}`
            return request
        }
    })
})
