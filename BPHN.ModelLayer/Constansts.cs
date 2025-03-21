﻿namespace BPHN.ModelLayer
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
        public const string ACCOUNT__GET_RELATION_IDS = @"select distinct * 
                                                          from 
                                                          (
                                                              select Id from accounts where ParentId = @id
                                                              union                                    
                                                              select Id from accounts where Id = @id
                                                              union                                    
                                                              select ParentId as Id from accounts where Id = @id
                                                              union                                    
                                                              (select b.Id as Id from accounts a inner join accounts b on a.ParentId = b.ParentId where a.Id = @id)
                                                          ) ids 
                                                          where Id is not null";

        public const string BOOKING__GET_BY_ID = "select * from bookings where Id = @id";

        public const string BOOKING_DETAIL__UPDATE_STATUS = "update booking_details set Status = @status where Id = @id";
        public const string BOOKING_DETAIL__GET_BY_BOOKING_ID = "select * from booking_details where BookingId = @bookingId";
        public const string BOOKING_DETAIL__GET_BY_ID = "select * from booking_details where Id = @id";
        public const string BOOKING_DETAIL__UPDATE_MATCH = "update booking_details set TeamA = @teamA, TeamB = @teamB, Note = @note, Deposit = @deposit where Id = @id";

        public const string CONFIG__GET_ALL = "select c.Key, c.Value from configs c where c.AccountId = @accountId";
        public const string CONFIG__GET_BY_KEY = "select c.Key, c.Value from configs c where c.AccountId = @accountId and c.Key = @key";
        public const string CONFIG__GET_VALUE_BY_KEY = "select c.Value from configs c where c.AccountId = @accountId and c.Key = @key";

        public const string HISTORY_LOG__GET_DESCRIPTION = "select ModelId, OldData, NewData from history_log_descriptions where Id = @id";

        public const string PERMISSION__GET_ALL = "select * from permissions where AccountId = @accountId order by FunctionType";

        public const string PITCH__GET_BY_ID = "select Id, Name, Address, MinutesPerMatch, Quantity, TimeSlotPerDay, Status, NameDetails from pitchs where id = @id";
        public const string PITCH__GET_ALL = "select * from pitchs where ManagerId = @accountId and Status = @status";

        public const string TIME_FRAME__GET_BY_PITCH_ID = "select Id, SortOrder, Name, TimeBegin, TimeEnd, Price from time_frame_infos where PitchId = @pitchId order by SortOrder";
        public const string TIME_FRAME__GET_BY_LIST_PITCH_ID = "select Id, SortOrder, Name, TimeBegin, TimeEnd, Price, PitchId from time_frame_infos where PitchId in @pitchId order by SortOrder";
        public const string TIME_FRAME__GET_BY_ID = "select Id, SortOrder, Name, TimeBegin, TimeEnd, Price, PitchId from time_frame_infos where Id = @id";

        public const string ITEM__GET_ALL = "select * from items where AccountId = @accountId";
        public const string ITEM__GET_MANY = "select * from items";
        public const string ITEM__GET_BY_ID = "select Id, Code, Name, Status, Quantity, SalePrice, PurchasePrice, Unit from items where id = @id";
        public const string ITEM__UPDATE_BY_ID = "update items set Unit = @unit, Status = @status, Code = @code, Name = @name, Quantity = @quantity, SalePrice = @salePrice, PurchasePrice = @purchasePrice, ModifiedBy = @modifiedBy, ModifiedDate = @modifiedDate where Id = @id";
        public const string ITEM__INSERT = "insert into items(Id, AccountId, Unit, Status, Code, Name, Quantity, SalePrice, PurchasePrice, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy) values (@id, @accountId, @unit, @status, @code, @name, @quantity, @salePrice, @purchasePrice, @createdDate, @createdBy, @modifiedDate, @modifiedBy)";

        public const string INVOICE__GET_ALL = "select Id, Status, Date, CustomerPhone, CustomerName, Total, PaymentType, CustomerType from invoices where AccountId = @accountId order by Date desc";
        public const string INVOICE__GET_MANY = "select Id, Status, Date, CustomerPhone, CustomerName, Total, PaymentType, CustomerType from invoices";
        public const string INVOICE__GET_BY_ID = "select * from invoices where Id = @id";
        public const string INVOICE__GET_BY_BOOKING = "select i.* from invoice_bookingdetail ibd join invoices i on ibd.InvoiceId = i.Id where ibd.BookingDetailId = @id";
        public const string INVOICE__INSERT = "insert into invoices(Id, AccountId, CustomerType, CustomerName, CustomerPhone, PaymentType, Total, Detail, Date, Status, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy) values (@id, @accountId, @customerType, @customerName, @customerPhone, @paymentType, @total, @detail, @date, @status, @createdDate, @createdBy, @modifiedDate, @modifiedBy)";
        public const string INVOICE__UPDATE = "update invoices set CustomerType = @customerType, CustomerName = @customerName, CustomerPhone = @customerPhone, PaymentType = @paymentType, Total = @total, Detail = @detail, Date = @date, Status = @status, ModifiedDate = @modifiedDate, ModifiedBy = @modifiedBy where id = @id";

        public const string INVOICE_BOOKING_DETAIL__INSERT = "insert into invoice_bookingdetail(Id, BookingDetailId, InvoiceId, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy) values(@id, @bookingDetailId, @invoiceId, @createdDate, @createdBy, @modifiedDate, @modifiedBy)";
    }
}
