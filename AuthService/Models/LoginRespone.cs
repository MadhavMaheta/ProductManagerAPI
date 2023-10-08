namespace AuthService.Models
{
    public class LoginRespone
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
        public string UserWiseToken { get; set; }
        public DateTime Expiration { get; set; }
        public string EMail { get; set; }
        public string Permissions { get; set; }
        public int StatusCode { get; set; }
        public long UserId { get; set; }
        //public long RoleId { get; set; }
        public string RoleName { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
    }
}
