namespace Portfolio.Core.Types.DataTypes
{
    public class PasswordValidationType
    {
        public string UserConfirmPassword { get; set; }
        public string RequestConfirmPassword { get; set; }
        public byte[] UserSalt { get; set; }
    }
}