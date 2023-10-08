namespace AuthService.DTO
{
    public class UserSetPassword
    {
        public int UserId { get; set; }
        public string? Password { get; set; }
        public string? ConfrimPassword { get; set; }
    }
}
