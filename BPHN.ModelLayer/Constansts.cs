namespace BPHN.ModelLayer
{
    public static class Constansts
    {
        public const int EXPIRE_HOUR_REDIS_CACHE = 8;
    }

    public static class ErrorCodes
    {
        public const int INVALID_ROLE = 100;
        public const int EMPTY_INPUT = 101;
        public const int NOT_EXISTS = 102;
        public const int EXISTED = 103;
        public const int OUT_TIME = 104;
        public const int NO_INTEGRITY = 105;
        public const int INACTIVE_DATA = 106;
    }

    public static class  SharedResourceKey
    {
        public const string OUTTIME = "OUTTIME";
        public const string EMPTYINPUT = "EMPTYINPUT";
        public const string NOTEXIST = "NOTEXIST";
        public const string INVALIDROLE = "INVALIDROLE";
        public const string LOGINFAIL = "LOGINFAILD";
        public const string INACTIVESTATUS = "INACTIVESTATUS";
        public const string EXISTED = "EXISTED";
        public const string INVALIDDATA = "INVALIDDATA";
    }

    public static class Query
    {
        public const string ACCOUNT__CHECK_EXIST_USERNAME = "select count(*) from accounts where UserName = @userName";
        public const string ACCOUNT__GET_BY_USERNAME = "select * from accounts where UserName = @userName";
        public const string ACCOUNT__GET_BY_ID = "select UserName, FullName, Gender, PhoneNumber, Email, Id, Role, Status, ParentId from accounts where Id = @id";
        public const string ACCOUNT__UPDATE_PASSWORD = "update accounts set Password = @password where Id = @id";
        public const string ACCOUNT__GET_ALL = "select Id, UserName, Email from accounts";
        public const string ACCOUNT__UPDATE_TOKEN = "update accounts set Token = @token, RefreshToken = @refreshToken where Id = @id";

        public const string BOOKING_DETAIL__UPDATE_STATUS = "update booking_details set Status = @status where Id = @id";
        public const string BOOKING_DETAIL__GET_BY_BOOKING_ID = "select * from booking_details where BookingId = @bookingId";
        public const string BOOKING_DETAIL__GET_BY_ID = "select * from booking_details where Id = @id";
        public const string BOOKING_DETAIL__UPDATE_MATCH = "update booking_details set TeamA = @teamA, TeamB = @teamB, Note = @note, Deposite = @deposite where Id = @id";

        public const string CONFIG_GET_ALL = "select c.Key, c.Value from configs c where c.AccountId = @accountId";
        public const string CONFIG_GET_BY_KEY = "select c.Key, c.Value from configs c where c.AccountId = @accountId and c.Key in (@key)";
    }
}
