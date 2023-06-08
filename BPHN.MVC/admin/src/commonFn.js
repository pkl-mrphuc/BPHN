export default function useCommonFn() {
    const dateNow = () => {
        let now = new Date();
        return date(now);
    }

    const date = (date) => {
        if(typeof date == "string") {
            date = new Date(date);
        }
        date.setHours(0);
        date.setMinutes(0);
        date.setSeconds(0);
        return date;
    }

    const sameDate = (date1, date2) => {
        let date1Str = `${date1.getFullYear()}-${date1.getMonth()}-${date1.getDate()}`;
        let date2Str = `${date2.getFullYear()}-${date2.getMonth()}-${date2.getDate()}`;
        return date1Str == date2Str ? true : false;
    }

    const newDate = (date, miliseconds) => {
        return new Date(date.getTime() + miliseconds);
    }

    const yearEndDay = (date) => {
        return new Date(date.getFullYear(), 11, 31, 0, 0, 0);
    }

    const time = (date) => {
        if(typeof date == "string") {
            date = new Date(date);
        }
        let hours = date.getHours() < 10 ? `0${date.getHours()}` : date.getHours();
        let mintues = date.getMinutes() < 10 ? `0${date.getMinutes()}` : date.getMinutes();
        return `${hours}:${mintues}`;
    }

    const dateToString = (date) => {
        if(typeof date == "string") {
            date = new Date(date);
        }
        let fullYear = date.getFullYear();
        let month = date.getMonth() + 1 < 10 ? `0${(date.getMonth() + 1)}` : (date.getMonth() + 1);
        let day = date.getDate() < 10 ? `0${date.getDate()}` : date.getDate();
        return `${fullYear}-${month}-${day}`;
    }

    return {
        dateNow,
        sameDate,
        newDate,
        date,
        yearEndDay,
        time,
        dateToString
    }
}