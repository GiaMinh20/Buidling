using API.Payloads.Response.BaseResponses;

namespace API.Payloads.Response
{
    public class LoginResponse
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string AvatarUrl { get; set; }
    }
}
