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
    MULTIUSER: 'MultiUser',
    SYSTEMEMAIL: 'SystemEmail',
    SECRETEMAIL: 'SecretEmail'
};

export const FunctionTypeEnum = {
    ADDPITCH: 0,
    EDITPITCH: 1,
    VIEWLISTPITCH: 2,
    ADDBOOKING: 3,
    EDITBOOKING: 4,
    VIEWLISTBOOKING: 5,
    ADDUSER: 6,
    EDITUSER: 7,
    VIEWLISTUSER: 8,
    VIEWLISTBOOKINGDETAIL: 9,
    ADDINVOICE: 10,
    EDITINVOICE: 11,
    VIEWLISTINVOICE: 12,
    ADDSERVICE: 13,
    EDITSERVICE: 14,
    VIEWLISTSERVICE: 15
};

export const NotificationTypeEnum =
{
    CANCELBOOKINGDETAIL: 0,
    UPDATEMATCH: 1,
    INSERTBOOKING: 2,
    DECLINEBOOKING: 3,
    APPROVALBOOKING: 4,
    CHANGEPERMISSION: 5,
    INSERTPITCH: 6,
    UPDATEPITCH: 7,
    INSERTACCOUNT: 8,
    UPDATEACCOUNT: 9
}