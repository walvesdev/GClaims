namespace GClaims.Domain.Models.Auth
{
    public class AccessTokenJwt
    {
        public bool Authenticated { get; set; }
        public string Created { get; set; }
        public string Expiration { get; set; }
        public string Message { get; set; }
        public string AccessToken { get; set; }
    }
}
