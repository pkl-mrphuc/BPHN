import axios from "axios";

class InvoiceAPI {
    constructor() {

    }

    async getAll(data) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/invoices?txtSearch=${data.txtSearch}&status=${data.status}&customerType=${data.customerType}&date=${data.date}&paymentType=${data.paymentType}`;
        return await axios.get(requestUrl);
    }

    async getInstance(id) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/invoices/get-instance?id=${id}`;
        return await axios.get(requestUrl);
    }

    async getByBooking(bookingDetailId) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/invoices/get/${bookingDetailId}`;
        return await axios.get(requestUrl);
    }

    async insert(data) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/invoices/insert`;
        let requestParam = data;
        return await axios.post(requestUrl, requestParam);
    }

    async update(data) {
        let requestUrl = `${process.env.VUE_APP_API_URL}/invoices/update`;
        let requestParam = data;
        return await axios.post(requestUrl, requestParam);
    }
}

export default new InvoiceAPI();