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
        SET_PASSWORD = 0,
        FORTGOT_PASSWORD = 1
    }

    public enum QueueJobTypeEnum
    {
        SEND_MAIL = 0
    }

    public enum ActionEnum
    {
        LOGIN = 0,
        REGISTER_ACCOUNT = 1,
        SEND_RESET_PASSWORD = 2,
        SUBMIT_RESET_PASSWORD = 3,
        SAVE = 4,
        INSERT = 5,
        UPDATE = 6,
    }

    public enum ActiveStatusEnum
    {
        ACTIVE = 0,
        INACTIVE = 1
    }

    public enum BookingStatusEnum
    {
        SUCCESS = 0,
        CANCEL = 1,
        PENDING = 2
    }

    public enum FunctionTypeEnum
    {
        ADD_PITCH = 0,
        EDIT_PITCH = 1,
        VIEW_LIST_PITCH = 2,

        ADD_BOOKING = 3,
        EDIT_BOOKING = 4,
        VIEW_LIST_BOOKING = 5,

        ADD_USER = 6,
        EDIT_USER = 7,
        VIEW_LIST_USER = 8,

        VIEW_LIST_BOOKING_DETAIL = 9
    }

    public enum NotificationTypeEnum
    {
        ADD_PITCH = 0,
        EDIT_PITCH = 1,
        ADD_BOOKING = 2,
        EDIT_BOOKING = 3,
        ADD_USER = 4,
        EDIT_USER = 5
    }

    public enum EntityEnum
    {
        CONFIG = 0,
        PERMISSION = 1,
        PITCH = 2,
        ACCOUNT = 3
    }
}
