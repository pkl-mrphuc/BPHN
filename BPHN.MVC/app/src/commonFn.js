export default function useCommonFn() {

    const dateToString = (date, formatDate, hasTime = false, hasDate = true) => {
        if (typeof date == "string") {
            date = new Date(date);
        }
        let fullYear = date.getFullYear();
        let month =
            date.getMonth() + 1 < 10 ? `0${date.getMonth() + 1}` : date.getMonth() + 1;
        let day = date.getDate() < 10 ? `0${date.getDate()}` : date.getDate();

        let result = "";
        switch (formatDate) {
            case "yyyy-MM-dd":
                result = `${fullYear}-${month}-${day}`;
                break;
            case "dd-MM-yyyy":
                result = `${day}-${month}-${fullYear}`;
                break;
            default:
                result = `${day}/${month}/${fullYear}`;
                break;
        }

        if (hasTime) {
            let hours = date.getHours() < 10 ? `0${date.getHours()}` : date.getHours();
            let minutes =
                date.getMinutes() < 10 ? `0${date.getMinutes()}` : date.getMinutes();
            let seconds =
                date.getSeconds() < 10 ? `0${date.getSeconds()}` : date.getSeconds();
            if (hasDate) {
                return `${result} ${hours}:${minutes}:${seconds}`;
            } else {
                return `${hours}:${minutes}:${seconds}`;
            }
        }
        return result;
    };

    const saveLocalStorage = (key, value) => {
        if(localStorage.getItem(key)) {
            localStorage.removeItem(key);
        }
        localStorage.setItem(key, value);
    }

    const getLocalStorage = (key) => {
        return localStorage.getItem(key);
    }

    return {
        dateToString,
        saveLocalStorage,
        getLocalStorage
    }
}