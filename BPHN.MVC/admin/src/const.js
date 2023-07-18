export const RoleEnum = {
    TENANT: 1,
    ADMIN: 0,
    USER: 2
};

export const StatusEnum = {
    ACTIVE: 'ACTIVE',
    INACTIVE: 'INACTIVE'
};

export const GenderEnum = {
    MALE: 'MALE',
    FEMALE: 'FEMALE',
    OTHER: 'OTHER'
};

export const BookingStatusEnum = {
    SUCCESS: 'SUCCESS',
    CANCEL: 'CANCEL',
    PENDING: 'PENDING'
};

export const ConfigKeyEnum = {
    DARKMODE: 'DarkMode',
    LANGUAGE: 'Language',
    FORMATDATE: 'FormatDate',
    MULTIUSER: 'MultiUser'
};

export const FunctionTypeEnum = {
    ADD_PITCH: 0,
    EDIT_PITCH: 1,
    VIEW_LIST_PITCH: 2,
    ADD_BOOKING: 3,
    EDIT_BOOKING: 4,
    VIEW_LIST_BOOKING: 5,
    ADD_USER: 6,
    EDIT_USER: 7,
    VIEW_LIST_USER: 8,
    VIEW_LIST_BOOKING_DETAIL: 9
};

export const NotificationTypeEnum =
{
    ADD_PITCH: 0,
    EDIT_PITCH: 1,
    ADD_BOOKING: 2,
    EDIT_BOOKING: 3,
    ADD_USER: 4,
    EDIT_USER: 5
}