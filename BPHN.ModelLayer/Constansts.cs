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
}
