namespace Api.Controllers.User.Requests
{
    public class GetUserTokenRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
