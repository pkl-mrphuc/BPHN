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
        public const int INVALID_DATA = 107;
        public const int INVALID_LICENSE = 108;
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
        public const string INVALIDLICENSE = "INVALIDLICENSE";
    }

    public static class Query
    {
        public const string ACCOUNT__CHECK_EXIST_USERNAME = "select count(*) from accounts where UserName = @userName";
        public const string ACCOUNT__GET_BY_USERNAME = "select * from accounts where UserName = @userName";
        public const string ACCOUNT__GET_BY_ID = "select UserName, FullName, Gender, PhoneNumber, Email, Id, Role, Status, ParentId from accounts where Id = @id";
        public const string ACCOUNT__UPDATE_PASSWORD = "update accounts set Password = @password where Id = @id";
        public const string ACCOUNT__UPDATE = "update accounts set FullName = @fullName, Gender = @gender, PhoneNumber = @phoneNumber, Status = @status where Id = @id";
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
        public const string BOOKING_DETAIL__GET_CALENDAR_EVENTS = @"select bd.*, b.PitchId, tfi.TimeBegin as Start, tfi.TimeEnd as End, b.NameDetail as Stadium, b.PhoneNumber as PhoneNumber  from booking_details bd 
                                                                        inner join bookings b on b.Id = bd.BookingId
                                                                        inner join time_frame_infos tfi on b.TimeFrameInfoId = tfi.Id
                                                                        inner join pitchs p on p.Id = b.PitchId and p.Id = @pitchId
                                                                    where b.NameDetail = @nameDetail and bd.MatchDate between @startDate and @endDate";

        public const string CONFIG__GET_ALL = "select c.Key, c.Value from configs c where c.AccountId = @accountId";
        public const string CONFIG__GET_BY_KEY = "select c.Key, c.Value from configs c where c.AccountId = @accountId and c.Key = @key";
        public const string CONFIG__GET_VALUE_BY_KEY = "select c.Value from configs c where c.AccountId = @accountId and c.Key = @key";

        public const string HISTORY_LOG__GET_DESCRIPTION = "select ModelId, OldData, NewData from history_log_descriptions where Id = @id";

        public const string PERMISSION__GET_ALL = "select * from permissions where AccountId = @accountId order by FunctionType";
        public const string PERMISSION__DELETE = "delete from permissions where AccountId = @accountId";

        public const string PITCH__GET_BY_ID = "select Id, Name, Address, MinutesPerMatch, Quantity, TimeSlotPerDay, Status, NameDetails from pitchs where id = @id";
        public const string PITCH__GET_ALL = "select * from pitchs";

        public const string TIME_FRAME__GET_BY_PITCH_ID = "select Id, SortOrder, Name, TimeBegin, TimeEnd, Price from time_frame_infos where PitchId = @pitchId order by SortOrder";
        public const string TIME_FRAME__GET_BY_LIST_PITCH_ID = "select Id, SortOrder, Name, TimeBegin, TimeEnd, Price, PitchId from time_frame_infos where PitchId in @pitchId order by SortOrder";
        public const string TIME_FRAME__GET_BY_ID = "select Id, SortOrder, Name, TimeBegin, TimeEnd, Price, PitchId from time_frame_infos where Id = @id";

        public const string ITEM__GET_ALL = "select * from items where AccountId = @accountId";
        public const string ITEM__GET_MANY = "select * from items";
        public const string ITEM__GET_BY_ID = "select Id, Code, Name, Status, Quantity, SalePrice, PurchasePrice, Unit from items where id = @id";
        public const string ITEM__GET_QUANTITY_BY_ID = "select Quantity from items where id = @id";
        public const string ITEM__UPDATE_BY_ID = "update items set Unit = @unit, Status = @status, Code = @code, Name = @name, Quantity = @quantity, SalePrice = @salePrice, PurchasePrice = @purchasePrice, ModifiedBy = @modifiedBy, ModifiedDate = @modifiedDate where Id = @id";
        public const string ITEM__UPDATE_QUANTITY_BY_ID = "update items set Quantity = Quantity - @quantity where Id = @id";
        public const string ITEM__INSERT = "insert into items(Id, AccountId, Unit, Status, Code, Name, Quantity, SalePrice, PurchasePrice, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy) values (@id, @accountId, @unit, @status, @code, @name, @quantity, @salePrice, @purchasePrice, @createdDate, @createdBy, @modifiedDate, @modifiedBy)";

        public const string INVOICE__GET_MANY = "select Id, Status, Date, CustomerPhone, CustomerName, Total, PaymentType, CustomerType from invoices";
        public const string INVOICE__GET_BY_ID = "select * from invoices where Id = @id";
        public const string INVOICE__GET_BY_BOOKING = "select i.* from invoice_bookingdetail ibd join invoices i on ibd.InvoiceId = i.Id where ibd.BookingDetailId = @id";
        public const string INVOICE__INSERT = "insert into invoices(Id, AccountId, CustomerType, CustomerName, CustomerPhone, PaymentType, Total, Detail, Date, Status, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy) values (@id, @accountId, @customerType, @customerName, @customerPhone, @paymentType, @total, @detail, @date, @status, @createdDate, @createdBy, @modifiedDate, @modifiedBy)";
        public const string INVOICE__UPDATE = "update invoices set CustomerType = @customerType, CustomerName = @customerName, CustomerPhone = @customerPhone, PaymentType = @paymentType, Total = @total, Detail = @detail, Date = @date, Status = @status, ModifiedDate = @modifiedDate, ModifiedBy = @modifiedBy where id = @id";

        public const string INVOICE_BOOKING_DETAIL__INSERT = "insert into invoice_bookingdetail(Id, BookingDetailId, InvoiceId, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy) values(@id, @bookingDetailId, @invoiceId, @createdDate, @createdBy, @modifiedDate, @modifiedBy)";

        public const string LICENSE__GET = "select * from licenses where AccountId = @accountId";
        public const string LICENSE__INSERT = "insert into licenses(Id, AccountId, Type, MaxInvoices, MaxDraftInvoices, ExpireTime, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy) values(@id, @accountId, @type, @maxInvoices, @maxDraftInvoices, @expireTime, @createdDate, @createdBy, @modifiedDate, @modifiedBy)";
        public const string LICENSE__UPDATE = "update licenses set Type = @type, MaxInvoices = @maxInvoices, ExpireTime = @expireTime, ModifiedDate = @modifiedDate, ModifiedBy = @modifiedBy where Id = @id";

        public const string STATISTIC__GET_TOTAL_BOOKING = @"select 
	                                                                sum(case when bd.MatchDate >= @preVal1 and bd.MatchDate <= @preVal2 then 1 else 0 end) as preValue,
                                                                    sum(case when bd.MatchDate >= @val1 and bd.MatchDate <= @val2 then 1 else 0 END) as value,
                                                                    @parameter as parameter
                                                                from booking_details bd 
                                                                join bookings b on b.Id = bd.BookingId
                                                                where bd.MatchDate >= @preVal1 and bd.MatchDate <= @val2 and b.AccountId = @accountId";
        public const string STATISTIC__GET_TOTAL_DETAIL_BOOKING = @"select 
	                                                                        sum(case when bd.Status = 'PENDING' THEN 1 ELSE 0 END) AS pending,
                                                                            sum(case when bd.Status = 'SUCCESS' THEN 1 ELSE 0 END) AS success,
                                                                            sum(case when bd.Status = 'CANCEL' THEN 1 ELSE 0 END) AS cancel,
                                                                            @parameter as parameter
                                                                        from booking_details bd 
                                                                        join bookings b on b.Id = bd.BookingId
                                                                        where bd.MatchDate >= @val1 and bd.MatchDate <= @val2 and b.AccountId = @accountId";
        public const string STATISTIC__GET_REVENUE = @"select  
	                                                        sum(case when Date >= @preVal1 and Date <= @preVal2 then Total else 0 end) as preValue,
                                                            sum(case when Date >= @val1 and Date <= @val2 then Total else 0 end) as value,
                                                            @parameter as parameter
                                                        from invoices 
                                                        where Date >= @preVal1 and Date <= @val2 and AccountId = @accountId";
        public const string STATISTIC__GET_REVENUE_SERVICE_YEAR = @"select 
                                                                        (select sum(Total) 
                                                                            from invoices where Date >= @val1 and Date <= @val2 and AccountId = @accountId) as total,
                                                                        (select SUM(tfi.Price) 
                                                                            from invoice_bookingdetail ibd
                                                                            join invoices i on i.Id = ibd.InvoiceId
                                                                            join booking_details bd on bd.Id = ibd.BookingDetailId
                                                                            join bookings b on b.Id = bd.BookingId
                                                                            join time_frame_infos tfi on tfi.Id = b.TimeFrameInfoId
                                                                            where i.Date >= @val1 and i.Date <= @val2 and b.AccountId = @accountId) as detail1,
                                                                        @parameter as parameter";
        public const string STATISTIC__GET_TOTAL_INVOICE = @"select
                                                                count(case when status = @status1 then 1 end) as draft,
                                                                count(case when status = @status2 then 1 end) as published
                                                             from invoices where AccountId = @accountId";
    }
}
