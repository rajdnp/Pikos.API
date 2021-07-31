namespace Pikos.Models.DTOs
{
    public static class AccountDtos
    {
        public class RevokeTokenRequest
        {
            public string Token { get; set; }
        }

        public class RefreshToken
        {
            public string Token { get; set; }
            public string IpAddress { get; set; }
        }

        public class RevokeToken
        {
            public string Token { get; set; }
            public string IpAddress { get; set; }
        }
    }
}
