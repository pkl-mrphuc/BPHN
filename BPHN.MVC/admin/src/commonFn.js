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
        return `${hours}:${mintues}:00`;
    }

    const numberToTime = (number) => {
        let hours = Math.floor(number / 60);
        let minutes = number % 60;

        hours = hours < 10 ? `0${hours}` : hours;
        minutes = minutes < 10 ? `0${minutes}` : minutes;
        return `${hours}:${minutes}:00`;
    }

    const dateToString = (date, formatDate, hasTime = false) => {
        if (typeof date == "string") {
            date = new Date(date);
        }
        let fullYear = date.getFullYear();
        let month = date.getMonth() + 1 < 10 ? `0${(date.getMonth() + 1)}` : (date.getMonth() + 1);
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
            let minutes = date.getMinutes() < 10 ? `0${date.getMinutes()}` : date.getMinutes();
            let seconds = date.getSeconds() < 10 ? `0${date.getSeconds()}` : date.getSeconds();
            return `${result} ${hours}:${minutes}:${seconds}`;
        }
        return result;
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

    const equals = (string1, string2) => {
        return string1 === string2;
    }

    const isEmail = (string) => {
        return /^[^@]+@\w+(\.\w+)+\w$/.test(string);
    }

    const padToFive = (number) => {
        if (number <= 99999) { number = ("0000" + number).slice(-5); }
        return number;
    }

    const fakeNumber = (number, numberDecimal = 0, separateThousand = ".", prefix = "", suffixes = "", separateDecimal = ",") => {
        let fake = `${number}`;
        let thousand = ``;
        let decimal = ``;
        fake.split(".").forEach((value, index) => {
            if (index == 0) thousand = value;
            else decimal = value;
        });

        let increament = 0;
        if (!decimal) {
            for (let i = 0; i < numberDecimal; i++) {
                decimal += "0";
            }
        } else {
            if (decimal.length <= numberDecimal) {
                decimal = (decimal - "0") * Math.pow(10, numberDecimal - decimal.length);
            } else {
                let hasIncreament = decimal.slice(numberDecimal, numberDecimal + 1) - "0" >= 5;
                decimal = decimal.slice(0, numberDecimal);
                if (hasIncreament) {
                    increament = 1;
                    for (let i = decimal.length - 1; i >= 0; i--) {
                        let digit = decimal[i] - "0";
                        digit = digit + increament;
                        if (digit == 10) {
                            increament = 1;
                            digit = 0;
                        } else {
                            increament = 0;
                        }
                        decimal = decimal.slice(0, i) + (digit + "") + decimal.slice(i + 1);
                    }
                }
            }
        }
        thousand = thousand - "0" + increament + "";
        thousand = thousand.split("").reverse().join("");
        thousand = thousand
            .match(/.{1,3}/g)
            .join(separateThousand)
            .split("")
            .reverse()
            .join("");

        if (numberDecimal === 0) {
            if (thousand === "0") return "";
            return `${prefix}${thousand}${suffixes}`;
        }
        return `${prefix}${thousand}${separateDecimal}${decimal}${suffixes}`;
    }

    const quarter = (date) => {
        date = date ?? new Date();
        var m = Math.floor(date.getMonth() / 3) + 2;
        return m > 4 ? m - 4 : m;
    }

    return {
        sameDate,
        newDate,
        yearEndDay,
        time,
        dateToString,
        getWeekdays,
        ticks,
        equals,
        isEmail,
        padToFive,
        fakeNumber,
        numberToTime,
        quarter
    }
}