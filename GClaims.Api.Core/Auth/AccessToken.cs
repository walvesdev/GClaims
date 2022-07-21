namespace GClaims.Core.Auth
{
    public class AccessToken
    {
        public bool Authenticated { get; set; }
        public string Created { get; set; }
        public string Expiration { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public Guid UserId { get; set; }
    }
}
