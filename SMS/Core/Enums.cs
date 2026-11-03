using System;

namespace CodeflexSystems.SMS.Core
{
    [Flags]
    [Serializable]
    public enum SaveMode : byte
    {
        SaveChanges = 0x01,
        PrintConsole = 0x02,
        PrintUI = 0x04,
        AcceptChanges = 0x08
    }

    [Serializable]
    public enum SearchValueType : byte
    {
        NoneType,
        Boolean,
        DateTime,
        Decimal,
        Integral,
        Object,
        Percentage,
        Text,
        Time
    }

    [Serializable]
    public enum AddressSearchOption : byte
    {
        NoUsage,
        Line1,
        Line2,
        ZipCode,
        City,
        Township,
        State,
        Country,
        CreatedDate,
        CreatedBy,
        ModifiedDate,
        ModifiedBy,
    }

    [Serializable]
    public enum GuardianSearchOption : byte
    {
        NoUsage,
        Title,
        GuardianName,
        Nationality,
        NIRC,
        Education,
        Employment,
        Designation,
        Passport,
        Visa,
        BirthDate,
        BirthCountry,
        Religion,
        CreatedDate,
        CreatedBy,
        ModifiedDate,
        ModifiedBy,
    }

    [Serializable]
    public enum ReceiptSearchOption : byte
    {
        NoUsage,
        ReceiptDate,
        ReceiptCode,
        ReceiptNo,
        TotalAmount,
        TaxAmount,
        TaxPercentage,
        NetAmount,
        CreatedDate,
        CreatedBy,
        ModifiedDate,
        ModifiedBy,
        DetailAmount,
        DetailDiscountAmount,
        DetailNetAmount,
        Student
    }

    [Serializable]
    public enum SectionSearchOption : byte
    {
        NoUsage,
        SectionName,
        Room,
        OpenDay,
        StartTime,
        EndTime,
        CommencingDate,
        CompletionDate,
        Fee,
        Monthly,
        CreatedDate,
        CreatedBy,
        ModifiedDate,
        ModifiedBy
    }

    [Serializable]
    public enum StudentDiscountSearchOption : byte
    {
        NoUsage,
        DiscountAmount,
        DiscountPercentage,
        CreatedDate,
        CreatedBy,
        ModifiedDate,
        ModifiedBy,
        Student
    }

    [Serializable]
    public enum StudentFerryFeeSearchOption : byte
    {
        NoUsage,
        Student,
        Fee,
        Monthly,
        CreatedDate,
        CreatedBy,
        ModifiedDate,
        ModifiedBy
    }

    [Serializable]
    public enum StudentSearchOption : byte
    {
        NoUsage,
        Id,
        Title,
        StudentName,
        OtherName,
        StudentFullName,
        Nationality,
        NIRC,
        Passport,
        Visa,
        BirthDate,
        BirthCountry,
        Religion,
        CreatedDate,
        CreatedBy,
        ModifiedDate,
        ModifiedBy,
        RegisteredDate,
        ResignedDate,
        Active
    }

    [Serializable]
    public enum UserSearchOption : byte
    {
        NoUsage,
        UserName,
        FullName,
        CreatedDate,
        CreatedBy,
        ModifiedDate,
        ModifiedBy,
        Disabled,
        DisabledDate,
        LogonDate,
        PasswordModifiedDate,
        PasswordFailures,
        PasswordFailureDate
    }

    [Serializable]
    public enum ReportLevel : sbyte
    {
        None = 0,
        School,
        AcademicYear,
        Course,
        Class,
        Section,
        StudentSection
    }

    [Serializable]
    public enum ReportType
    {
        StudentList,
        StudentListDetails,
        StudentListAge,
        StudentListInfo,
        StudentListInRoom,
        StudentListAddress,
        StudentListAddress2,
        StudentListEmail,
        StudentListEmail2,
        StudentListPhone,
        StudentListPhone2,
        StudentListGuardian,
        StudentListGuardian2,
        StudentCard,
        School = 10000,
        AcademicYear,
        Course,
        Class,
        Section,
        AddressType = 11000,
        EmailType,
        PhoneType,
        GuardianType,
        Country,
        Religion,
        Currency,
        FeeType,
        StudentDiscount = 20000,
        StudentFerryFee,
        Reminder,
        Reminder2,
        DailyIncome,
        DailyIncomeDetails,
        DailyIncomeDetails2,
        MonthlyIncome,
        YearlyIncome
    }
}
