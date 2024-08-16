namespace Portfolio.Core.Types.Enums.Users
{
    public enum UserStatusEnum
    {
        [Description("Active")]
        Active,
        [Description("LoggedOut")]
        LoggedOut,
        [Description("Blocked")]
        Blocked,
        [Description("ForgotPassword")]
        ForgotPassword,
        [Description("Inactive")]
        Inactive,
        [Description("Pending")]
        Pending,
        [Description("DEFAULT")]
        DEFAULT
    }
}