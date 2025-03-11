namespace BPHN.ModelLayer
{
    public enum GenderEnum
    {
        MALE = 0,
        FEMALE = 1,
        OTHER = 2
    }

    public enum RoleEnum
    {
        ADMIN = 0,
        TENANT = 1,
        USER = 2
    }

    public enum MailTypeEnum
    {
        SETPASSWORD = 0,
        FORTGOTPASSWORD = 1,
        APPROVALBOOKING = 2,
        DECLINEBOOKING = 3
    }

    public enum QueueJobTypeEnum
    {
        SENDMAIL = 0,
        WRITELOG = 1
    }

    public enum ActionEnum
    {
        LOGIN = 0,
        REGISTERACCOUNT = 1,
        SENDRESETPASSWORD = 2,
        SUBMITRESETPASSWORD = 3,
        SAVE = 4,
        INSERT = 5,
        UPDATE = 6,
        CANCEL = 7
    }

    public enum ActiveStatusEnum
    {
        ACTIVE = 0,
        INACTIVE = 1
    }

    public enum BookingStatusEnum
    {
        PENDING = 0,
        SUCCESS = 1,
        CANCEL = 2
    }

    public enum FunctionTypeEnum
    {
        ADDPITCH = 0,
        EDITPITCH = 1,
        VIEWLISTPITCH = 2,
        ADDBOOKING = 3,
        EDITBOOKING = 4,
        VIEWLISTBOOKING = 5,
        ADDUSER = 6,
        EDITUSER = 7,
        VIEWLISTUSER = 8,
        VIEWLISTBOOKINGDETAIL = 9,
        ADDINVOICE = 10,
        EDITINVOICE = 11,
        VIEWLISTINVOICE = 12,
        ADDSERVICE = 13,
        EDITSERVICE = 14,
        VIEWLISTSERVICE = 15
    }

    public enum NotificationTypeEnum
    {
        CANCELBOOKINGDETAIL = 0,
        UPDATEMATCH = 1,
        INSERTBOOKING = 2,
        DECLINEBOOKING = 3,
        APPROVALBOOKING = 4,
        CHANGEPERMISSION = 5,
        INSERTPITCH = 6,
        UPDATEPITCH = 7,
        INSERTACCOUNT = 8,
        UPDATEACCOUNT = 9,
        INSERTSERVICIE = 10,
        UPDATESERVICE = 11
    }

    public enum EntityEnum
    {
        CONFIG = 0,
        PERMISSION = 1,
        PITCH = 2,
        ACCOUNT = 3,
        BOOKING = 4,
        BOOKINGDETAIL = 5,
        SERVICE = 6
    }
}
