export default function useCommonFn() {

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
        if (typeof date == "string") {
            date = new Date(date);
        }
        let hours = date.getHours() < 10 ? `0${date.getHours()}` : date.getHours();
        let mintues = date.getMinutes() < 10 ? `0${date.getMinutes()}` : date.getMinutes();
        return `${hours}:${mintues}`;
    }

    const dateToString = (date, hasTime = false) => {
        if (typeof date == "string") {
            date = new Date(date);
        }
        let fullYear = date.getFullYear();
        let month = date.getMonth() + 1 < 10 ? `0${(date.getMonth() + 1)}` : (date.getMonth() + 1);
        let day = date.getDate() < 10 ? `0${date.getDate()}` : date.getDate();

        if(hasTime) {
            let hours = date.getHours() < 10 ? `0${date.getHours()}` : date.getHours();
            let minutes = date.getMinutes() < 10 ? `0${date.getMinutes()}` : date.getMinutes();
            let seconds = date.getSeconds() < 10 ? `0${date.getSeconds()}` : date.getSeconds();
            return `${fullYear}-${month}-${day} ${hours}:${minutes}:${seconds}`;
        }
        return `${fullYear}-${month}-${day}`;
    }

    const getWeekdays = (value) => {
        switch (value) {
            case 0:
                return "Sunday";
            case 1:
                return "Monday";
            case 2:
                return "Tuesday";
            case 3:
                return "Wednesday";
            case 4:
                return "Thursday";
            case 5:
                return "Friday";
            case 6:
                return "Saturday";
        }
    }

    const ticks = (date) => {
        return (date.getTime() * 10000 + 621355968000000000) - (date.getTimezoneOffset() * 600000000);
    }

    return {
        sameDate,
        newDate,
        yearEndDay,
        time,
        dateToString,
        getWeekdays,
        ticks
    }
}