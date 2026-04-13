namespace SharedKernel.Enums;

public enum UserType
{
    [Display(Name = "System Admin")]
    SystemAdmin = 1,

    [Display(Name = "Oragnization Admin")]
    Admin = 2,

    [Display(Name = "HR Manager")]
    HRManager = 3,

    [Display(Name = "Employee")]
    Employee = 4,

    [Display(Name = "Manager")]
    Manager = 5
}

public enum HolidayType
{
    [Display(Name = "Public Holiday")]
    Public = 1,

    [Display(Name = "Restricted Holiday")]
    Restricted = 2,
}

public enum EmploymentStatus
{
    [Display(Name = "Active")]
    Active = 1,

    [Display(Name = "On Leave")]
    OnLeave = 2,

    [Display(Name = "Terminated")]
    Terminated = 3,

    [Display(Name = "Resigned")]
    Resigned = 4,

    [Display(Name = "Probation")]
    Probation = 5
}

public enum EmploymentType
{
    [Display(Name = "Full Time")]
    FullTime = 1,

    [Display(Name = "Part Time")]
    PartTime = 2,

    [Display(Name = "Contract")]
    Contract = 3,

    [Display(Name = "Intern")]
    Intern = 4,

    [Display(Name = "Temporary")]
    Temporary = 5
}

public enum LeaveType
{
    [Display(Name = "Annual Leave")]
    Annual = 1,

    [Display(Name = "Sick Leave")]
    Sick = 2,

    [Display(Name = "Personal Leave")]
    Personal = 3,

    [Display(Name = "Maternity Leave")]
    Maternity = 4,

    [Display(Name = "Paternity Leave")]
    Paternity = 5,

    [Display(Name = "Unpaid Leave")]
    Unpaid = 6,

    [Display(Name = "Bereavement Leave")]
    Bereavement = 7,

    [Display(Name = "Compensatory Leave")]
    Compensatory = 8
}

public enum LeaveStatus
{
    [Display(Name = "Pending")]
    Pending = 1,

    [Display(Name = "Approved")]
    Approved = 2,

    [Display(Name = "Rejected")]
    Rejected = 3,

    [Display(Name = "Cancelled")]
    Cancelled = 4
}

public enum AttendanceStatus
{
    [Display(Name = "Present")]
    Present = 1,

    [Display(Name = "Absent")]
    Absent = 2,

    [Display(Name = "Late")]
    Late = 3,

    [Display(Name = "Half Day")]
    HalfDay = 4,

    [Display(Name = "On Leave")]
    OnLeave = 5,

    [Display(Name = "Work From Home")]
    WorkFromHome = 6
}

public enum PayrollStatus
{
    [Display(Name = "Draft")]
    Draft = 1,

    [Display(Name = "Pending Approval")]
    PendingApproval = 2,

    [Display(Name = "Approved")]
    Approved = 3,

    [Display(Name = "Processed")]
    Processed = 4,

    [Display(Name = "Paid")]
    Paid = 5
}

public enum Gender
{
    [Display(Name = "Male")]
    Male = 1,

    [Display(Name = "Female")]
    Female = 2,

    [Display(Name = "Other")]
    Other = 3,

    [Display(Name = "Prefer Not to Say")]
    PreferNotToSay = 4
}

public enum MaritalStatus
{
    [Display(Name = "Single")]
    Single = 1,

    [Display(Name = "Married")]
    Married = 2,

    [Display(Name = "Divorced")]
    Divorced = 3,

    [Display(Name = "Widowed")]
    Widowed = 4
}
//Enum for Months
public enum Month
{
    January = 1,
    February = 2,
    March = 3,
    April = 4,
    May = 5,
    June = 6,
    July = 7,
    August = 8,
    September = 9,
    October = 10,
    November = 11,
    December = 12
}
public enum LeaveRule
{
    [Display(Name = "General")]
    Standard = 1,
    //[Display(Name = "Standard")]
    //Standard = 1,

    //[Display(Name = "Accrual Based")]
    //AccrualBased = 2,

    //[Display(Name = "Unlimited")]
    //Unlimited = 3,

    //[Display(Name = "No Leave")]
    //NoLeave = 4
}

public enum MarriedStatus
{
    [Display(Name = "Single")]
    Single = 1,

    [Display(Name = "Married")]
    Married = 2,

    [Display(Name = "Divorced")]
    Divorced = 3,

    [Display(Name = "Widowed")]
    Widowed = 4
}

public enum ProjectStatus
{
    [Display(Name = "Active")]
    Active = 1,

    [Display(Name = "On Hold")]
    OnHold = 2,

    [Display(Name = "Completed")]
    Completed = 3,

    [Display(Name = "Cancelled")]
    Cancelled = 4
}

public enum EmailTemplateName
{
    [Display(Name = "Account Activation")]
    AccountActivation = 1,

    [Display(Name = "Reset Password")]
    ResetPassword = 2,

    [Display(Name = "User Invitation")]
    UserInvitation = 3,

    [Display(Name = "Leave Request")]
    LeaveRequest = 4,

    [Display(Name = "Leave Approval")]
    LeaveApproval = 5,

    [Display(Name = "Payroll Notification")]
    PayrollNotification = 6,

    [Display(Name = "Employee Onboarding")]
    EmployeeOnboarding = 7,

    [Display(Name = "Employee Offboarding")]
    EmployeeOffboarding = 8
}
public enum Year
{
    [Display(Name = "2010")]
    Y2010 = 2010,

    [Display(Name = "2011")]
    Y2011 = 2011,

    [Display(Name = "2012")]
    Y2012 = 2012,

    [Display(Name = "2013")]
    Y2013 = 2013,

    [Display(Name = "2014")]
    Y2014 = 2014,

    [Display(Name = "2015")]
    Y2015 = 2015,

    [Display(Name = "2016")]
    Y2016 = 2016,

    [Display(Name = "2017")]
    Y2017 = 2017,

    [Display(Name = "2018")]
    Y2018 = 2018,

    [Display(Name = "2019")]
    Y2019 = 2019,

    [Display(Name = "2020")]
    Y2020 = 2020,

    [Display(Name = "2021")]
    Y2021 = 2021,

    [Display(Name = "2022")]
    Y2022 = 2022,

    [Display(Name = "2023")]
    Y2023 = 2023,

    [Display(Name = "2024")]
    Y2024 = 2024,

    [Display(Name = "2025")]
    Y2025 = 2025,

    [Display(Name = "2026")]
    Y2026 = 2026,

    [Display(Name = "2027")]
    Y2027 = 2027,

    [Display(Name = "2028")]
    Y2028 = 2028,

    [Display(Name = "2029")]
    Y2029 = 2029,

    [Display(Name = "2030")]
    Y2030 = 2030
}
