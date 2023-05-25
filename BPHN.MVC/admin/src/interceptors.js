import axios from 'axios'

export function jwtInterceptor() {
    axios.interceptors.request.use((request) => {
        const authKey = JSON.parse(localStorage.getItem('admin-auth-key'))
        if(authKey?.account?.context?.token) {
            request.headers['Authorization'] = `Bearer ${authKey?.account?.context?.token}`
            return request
        }
    })
}
